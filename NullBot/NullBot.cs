using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DreamPoeBot.Loki.Coroutine;
using log4net;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Game;
using Message = DreamPoeBot.Loki.Bot.Message;
using UserControl = System.Windows.Controls.UserControl;

namespace NullBot
{
    public class NullBot : IBot
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private NullBotGui _gui;
        private Coroutine _coroutine;
        private bool _forceExilePatherReload = false;
        public const string GetTaskManagerMessage = "GetTaskManager";

        // The TaskManager is a List of ITask that are performed in order. Every time a task return true, the taskmanager restart the execution from the first task, while if the task return false, the execution continue to the next task.
        // The very last task of Every taskManager, should always be a task that return true, and restart the tasks execution.
        // Its responsability of the taskmanager to then Start/Stop the loop with the BotManager.Start/Stop functions.
        private readonly TaskManager _taskManager = new TaskManager();

        /// <summary>
        /// The Message system create a flexible system to send message and recieve answer from and to all the components of the bot.
        /// Once a message is send, every single ITask, IPlugin, IBot, IRoutine will recieve it, be able to read its content (id and eventual Object[] trasported) and eventually add some output (again Object[])
        /// </summary>
        /// <param name="message"> A message containing the Id of the message, its Object[] Input and Object[] Output</param>
        /// <returns>MessageResult usually indicating if someone readed the message or noone.</returns>
        public MessageResult Message(Message message)
        {
            var handled = false;
            var id = message.Id;

            if (id == GetTaskManagerMessage)
            {
                message.AddOutput(this, _taskManager);
                handled = true;
            }

            var res = _taskManager.SendMessage(TaskGroup.Enabled, message);
            if (res == MessageResult.Processed)
                handled = true;

            return handled ? MessageResult.Processed : MessageResult.Unprocessed;
        }

        public void Start()
        {
            ItemEvaluator.Instance = DefaultItemEvaluator.Instance;
            Explorer.CurrentDelegate = user => null;

            // Cache all bound keys.
            LokiPoe.Input.Binding.Update();

            // Reset the default MsBetweenTicks on start.
            BotManager.MsBetweenTicks = 30;
            
            Log.Debug($"[Start] MsBetweenTicks: {BotManager.MsBetweenTicks}.");
            Log.Debug($"[Start] KeyPickup: {LokiPoe.ConfigManager.KeyPickup}.");

            if (NullBotSettings.Instance.ShouldEnableHooks)
            {
                // Since this bot will be performing client actions, we need to enable the process hook manager.
                LokiPoe.ProcessHookManager.Enable();
            }
            

            // The coroutine, is a reference for a second thread that will handle the Primary decision this Bot should take, See the MainCoruotine function for dettails.
            _coroutine = null;

            // Reload the area to make the bot aware of walkable map.
            ExilePather.Reload();

            // Clean the task manager
            _taskManager.Reset();

            // Add tasks to the bot, to perform actions.
            AddTasks();

            // Propagate the Start event in all enabled Plugins, Routine, PlayerMover, and last the task manager(that will then propagate it to all active tasks.).
            PluginManager.Start();
            RoutineManager.Start();
            PlayerMoverManager.Start();
            _taskManager.Start();

            Log.Debug($"[Start] Current PlayerMover: {PlayerMoverManager.Current.Name}.");
            Log.Debug($"[Start] Current Routine {RoutineManager.Current.Name}.");
            foreach (var plugin in PluginManager.EnabledPlugins)
            {
                Log.Debug($"[Start] The plugin {plugin.Name} is enabled.");
            }

            // This is just an example of how to completly block the portion of a map occupied by the TrialOfAscendancy area, to avoid walk there.
            //if (ExilePather.BlockTrialOfAscendancy == FeatureEnum.Unset)
            //{
            //    if (AreasThisBotWalkInCanConteinTrials)
            //    {
            //        ExilePather.BlockTrialOfAscendancy = FeatureEnum.Enabled;
            //    }
            //    else
            //    {
            //        //no need for this, for exemple, map trials are in separate areas, we just need to be sure we dont enter those areas.
            //        ExilePather.BlockTrialOfAscendancy = FeatureEnum.Disabled;
            //    }
                
            //}
        }

        public void Stop()
        {
            // Propagate the Stop Event to the other bot components:
            _taskManager.Stop();
            PluginManager.Stop();
            RoutineManager.Stop();
            PlayerMoverManager.Stop();

            if (NullBotSettings.Instance.ShouldEnableHooks)
            {
                // When the bot is stopped, we want to remove the process hook manager.
                LokiPoe.ProcessHookManager.Disable();
            }

            // Cleanup the coroutine.
            if (_coroutine != null)
            {
                _coroutine.Dispose();
                _coroutine = null;
            }
        }

        public void Tick()
        {
            // Tick function will trigger once every 'BotManager.MsBetweenTicks' default value is 30 and is mainly used to perform non actions logics, like cache Objects around us, or preprocess logics that will be performed.

            // First we ensure the coroutine is not null and associated with the MainCoroutine function.
            if (_coroutine == null)
            {
                _coroutine = new Coroutine(() => MainCoroutine());
            }

            // Execute some logic based on the actual state:
            if (LokiPoe.IsInGame)
            {
                // Reload the ExilePather every time we Enter the game world.
                if (_forceExilePatherReload)
                {
                    ExilePather.Reload(true);
                    _forceExilePatherReload = false;
                }
                else
                    ExilePather.Reload();
            }
            else
            {
                _forceExilePatherReload = true;
            }

            // Propagate the Tick Event to the other bot components:
            _taskManager.Tick();
            PluginManager.Tick();
            RoutineManager.Tick();
            PlayerMoverManager.Tick();


            // Check to see if the coroutine is finished, meaning something stopped the bot (For exemple the Stop button.)
            if (_coroutine.IsFinished)
            {
                Log.Debug($"The bot coroutine has finished in a state of {_coroutine.Status}");
                BotManager.Stop();
                return;
            }

            // Otherwise Resume the coroutine execution.
            try
            {
                _coroutine.Resume();
            }
            catch
            {
                var c = _coroutine;
                _coroutine = null;
                c.Dispose();
                throw;
            }
        }

        public string Name => "NullBot";
        public string Description => "A Bot that does nothing.";
        public string Author => "Alcor75";
        public string Version => "1.0";
        public override string ToString() => $"{Name}: {Description}";

        public JsonSettings Settings => NullBotSettings.Instance;
        public UserControl Control => _gui ?? (_gui = new NullBotGui());

        public void Deinitialize()
        {
            BotManager.OnBotChanged -= BotManagerOnOnBotChanged;
        }

        public void Initialize()
        {
            BotManager.OnBotChanged += BotManagerOnOnBotChanged;
        }

        public async Task<LogicResult> Logic(Logic logic)
        {
            return await _taskManager.ProvideLogic(TaskGroup.Enabled, RunBehavior.UntilHandled, logic);
        }

        private void BotManagerOnOnBotChanged(object sender, BotChangedEventArgs botChangedEventArgs)
        {
            if (botChangedEventArgs.New == this)
            {
                ItemEvaluator.Instance = DefaultItemEvaluator.Instance;
            }
        }

        /// <summary>
        /// A public function to retrive the current taskmanager instance.
        /// </summary>
        /// <returns>The current TaskManager instance.</returns>
        public TaskManager GetTaskManager()
        {
            return _taskManager;
        }
        /// <summary>
        /// Add tasks to the TaskManager in a specific order.
        /// </summary>
        private void AddTasks()
        {
            // I add 2 CombatTask with different leash range, the first (leash 50) only fight real close monsters (within the screen range), the second (leash -1) fight all monster in combat range.
            // This way we can trigger other important non combat logic, like Post close range combat tasks, loot, chest/monolith interaction etc etc before to move to far away cheasing next monster.
            // This is just an exemple of possible combination, there are few if not not at all limit to the possibility use your fantasy!.

            if (NullBotSettings.Instance.ShouldEnableCombat)
            {
                _taskManager.Add(new CombatTask(50));
                _taskManager.Add(new PostCombatHookTask());
            }

            //_taskManager.Add(new LootItemsTask());
            if (NullBotSettings.Instance.ShouldEnableCombat)
            {
                _taskManager.Add(new CombatTask(-1));
            }
            _taskManager.Add(new ExampleTask());
            _taskManager.Add(new FallbackTask());
        }

        private async Task MainCoroutine()
        {
            // This function is an endless running function that newer return, b/c its created and sestroied by the Start and Stop event functions.
            while (true)
            {
                if (LokiPoe.IsInLoginScreen)
                {
                    // If we want to use a Plugin to handle the login actions, we propagate a Logic event to Trigger the plugin Logic function.
                    // A total Standalone Bot will handle the login actions by imself, with integrate logic and a dedicate UI for login password and char selection.
                    // Offload auto login logic to a plugin, sending it a Logic id that indicate the bot state.
                    var logic = new Logic("hook_login_screen", this);
                    // Propagate the Logic message to trigger each enabled Plugins Logic function with this id.
                    foreach (var plugin in PluginManager.EnabledPlugins)
                    {
                        if (await plugin.Logic(logic) == LogicResult.Provided)
                            break;
                    }
                }
                else if (LokiPoe.IsInCharacterSelectionScreen)
                {
                    // Offload auto login logic to a plugin, sending it a Logic id that indicate the bot state.
                    var logic = new Logic("hook_character_selection", this);
                    // Propagate the Logic message to trigger each enabled Plugins Logic function with this id.
                    foreach (var plugin in PluginManager.EnabledPlugins)
                    {
                        if (await plugin.Logic(logic) == LogicResult.Provided)
                            break;
                    }
                }
                else if (LokiPoe.IsInGame && ExilePather.IsReady)
                {
                    // Trigger Logic once again with 'hook_ingame' id, allow user coroutine logic to preempt the bot base coroutine logic.
                    // This was supported to a degree in 2.6, and in general with our bot bases. Technically, this probably should
                    // be at the top of the while loop in case your bot handle the login/enter game by imself, in case the bot bases offload sets of logic to plugins this way, this
                    // hook is being placed here.
                    var hooked = false;
                    var logic = new Logic("hook_ingame", this);
                    foreach (var plugin in PluginManager.EnabledPlugins)
                    {
                        if (await plugin.Logic(logic) == LogicResult.Provided)
                        {
                            hooked = true;
                            break;
                        }
                    }
                    if (!hooked)
                    {
                        // Wait for game pause
                        if (LokiPoe.InstanceInfo.IsGamePaused)
                        {
                            Log.Debug("Waiting for game pause");
                        }
                        // Resurrect character if it is dead
                        else if (LokiPoe.Me.IsDead)
                        {
                            // A resurrection Function need to be implemented here, should be an async Task.
                            //await ResurrectionLogic.Execute();
                        }
                        // What the bot does now is up to the registered tasks.
                        else
                        {
                            // Propagate the Run event to trigger the taskmanager Run founctions
                            await _taskManager.Run(TaskGroup.Enabled, RunBehavior.UntilHandled);
                        }
                    }
                }
                else
                {
                    // Most likely in a loading screen, which will cause us to block on the executor, 
                    // but just in case we hit something else that would cause us to execute...
                    await Coroutine.Sleep(1000);
                    continue;
                }

                // End of the tick.
                await Coroutine.Yield();
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
