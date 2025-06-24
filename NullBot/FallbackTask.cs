using System.Threading.Tasks;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Common;
using log4net;

namespace NullBot
{
    public class FallbackTask : ITask
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        public async Task<bool> Run()
        {
            Log.ErrorFormat("[FallbackTask] The Fallback task is executing. The bot does not know what to do.");
            await Coroutines.LatencyWait();
            return true;
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

        public string Name => "FallbackTask";
        public string Description => "This task is the last task executed. It should not execute.";
        public string Author => "Alcor75";
        public string Version => "1.0";

        #endregion
    }
}
