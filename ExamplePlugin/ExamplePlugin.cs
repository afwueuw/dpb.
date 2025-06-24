using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Common;
using log4net;

namespace ExamplePlugin
{
    public class ExamplePlugin : IPlugin, IStartStopEvents
    {
        public static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private TaskManager _currentManager;

        private ExamplePluginGui _gui;
        public string Author => "Alcor75";
        public string Description => "An Example Plugin that use Life, Mana and Hybrid Flasks, can be used with NullBot, with Disabled Hooks, as a autoflask tool while you hand play.";
        public string Name => "ExamplePlugin";
        public string Version => "1.0";
        public void Deinitialize()
        {
            
        }

        public void Initialize()
        {
            
        }

        public UserControl Control => _gui ?? (_gui = new ExamplePluginGui());
        public JsonSettings Settings => ExamplePluginSettings.Instance;
        public void Disable()
        {
            
        }

        public void Enable()
        {
            
        }

        public async Task<LogicResult> Logic(Logic logic)
        {
            return LogicResult.Unprovided;
        }

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public void Start()
        {
            _currentManager = GetCurrentBotTaskManager();
            if (_currentManager == null)
            {
                Log.ErrorFormat($"[ExamplePlugin] The current TaskManager is null!");
                StopReasonData stopReason = new StopReasonData(id:"666", reason:$"The bot: {BotManager.Current.Name} do not implement GetTaskManager message. ");
                BotManager.Stop(stopReason);
                return;
            }

            _currentManager.AddAtFront(new ExempleTask());
        }

        public void Stop()
        {
            _currentManager.Remove("ExempleTask");
        }

        private static TaskManager GetCurrentBotTaskManager()
        {
            var bot = BotManager.Current;

            var msg = new Message("GetTaskManager");
            bot.Message(msg);
            var taskManager = msg.GetOutput<TaskManager>();

            return taskManager;
        }
    }
}
