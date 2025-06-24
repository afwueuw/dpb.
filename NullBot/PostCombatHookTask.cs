using System.Threading.Tasks;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Common;
using log4net;

namespace NullBot
{
    public class PostCombatHookTask : ITask
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        public const string MessageId = "hook_post_combat";

        public async Task<bool> Run()
        {
            // The purpose of this task is to restart the Taskmanager execution from the first task, if a IPlugin or a IContent has executed Logic with id `hook_post_combat`.
            // If no IPlugin or IContent executed any logic, the return of the message is `LogicResult.Unprovided` and this task return false, so the Taskmanager continue to next task.
            foreach (var plugin in PluginManager.EnabledPlugins)
            {
                if (await plugin.Logic(new Logic(MessageId, this)) == LogicResult.Provided)
                {
                    Log.DebugFormat($"[PostCombatHookTask] \"{plugin.Name}\" returned true.");
                    return true;
                }
            }
            foreach (var content in ContentManager.Contents)
            {
                if (await content.Logic(new Logic(MessageId, this)) == LogicResult.Provided)
                {
                    Log.DebugFormat($"[PostCombatHookTask] \"{content.Name}\" returned true.");
                    return true;
                }
            }
            return false;
        }

        #region Unused interface methods

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public async Task<LogicResult> Logic(Logic logic)
        {
            return LogicResult.Unprovided;
        }

        public void Start()
        {
        }

        public void Tick()
        {
        }

        public void Stop()
        {
        }

        public string Name => "PostCombatHookTask";
        public string Description => "This task provides a coroutine hook for executing user logic after combat has completed.";
        public string Author => "Alcor75";
        public string Version => "1.0";

        #endregion
    }
}
