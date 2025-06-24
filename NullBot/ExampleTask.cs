using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Common;
using log4net;

namespace NullBot
{
    public class ExampleTask: ITask
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        public async Task<bool> Run()
        {
            
            await Coroutines.LatencyWait();
            // Return true make so the taskmanager restart the tasks execution to the first task, if we return false here, the FalbackTask will execute.
            // The last task, before the FallbackTask usually always return false to restart the TaskManager task execution from the first task.
            return true;
        }

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

        public string Name => "ExampleTask";
        public string Description => "This task is a demostration of ITask implementation.";
        public string Author => "Alcor75";
        public string Version => "1.0";

        private TaskManager GetTaskManager()
        {
            var msg = new Message(NullBot.GetTaskManagerMessage);
            var result = this.Message(msg);
            if (result == MessageResult.Processed)
            {
                var taskManager = msg.GetOutput<TaskManager>();

                return taskManager;
            }

            return null;

        }
    }
}
