using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using DreamPoeBot.Common;
using log4net;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Game;

namespace PatherExplorer
{
    public class PatherExplorer : IPlugin, ITickEvents
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private PatherExplorerGui _instance;
        private PatherExplorerData _data = new PatherExplorerData();

        #region Implementation of IAuthored

        /// <summary> The name of the plugin. </summary>
        public string Name => "PatherExplorer";

        /// <summary>The author of the plugin.</summary>
        public string Author => "Alcor75";

        /// <summary> The description of the plugin. </summary>
        public string Description => "A plugin that help debug and understand the ExilePather and Alcor75RDServer.";

        /// <summary>The version of the plugin.</summary>
        public string Version => "0.0.1.4";

        #endregion

        #region Implementation of IBase

        /// <summary>Initializes this plugin.</summary>
        public void Initialize()
        {
        }

        /// <summary>Deinitializes this object. This is called when the object is being unloaded from the bot.</summary>
        public void Deinitialize()
        {
            _instance?.OnDeinitialize();
        }

        #endregion

        #region Implementation of ITickEvents

        /// <summary> The plugin tick callback. Do any update logic here. </summary>
        public void Tick()
        {
            _data.Update();
        }

        #endregion

        #region Implementation of IConfigurable

        /// <summary>The settings object. This will be registered in the current configuration.</summary>
        public JsonSettings Settings => PatherExplorerSettings.Instance;

        /// <summary> The plugin's settings control. This will be added to the Exilebuddy Settings tab.</summary>
        public UserControl Control => (_instance ?? (_instance = new PatherExplorerGui(() => PluginManager.IsEnabled(this), () => _data)));

		#endregion

		#region Implementation of ILogicHandler

        /// <summary>
        /// Implements the ability to handle a logic passed through the system.
        /// </summary>
        /// <param name="logic">The logic to be processed.</param>
        /// <returns>A LogicResult that describes the result..</returns>
        public async Task<LogicResult> Logic(Logic logic)
        {
            return LogicResult.Unprovided;
        }

        #endregion

        #region Implementation of IMessageHandler

        /// <summary>
        /// Implements logic to handle a message passed through the system.
        /// </summary>
        /// <param name="message">The message to be processed.</param>
        /// <returns>A enum of MessageResult.</returns>
        public MessageResult Message(Message message)
        {
            var id = message.Id;
            if (id == "FindPath_Result")
            {
                try
                {
                    var pathfindingCommand = message.GetInput<PathfindingCommand>();
                    _data.PathfindingCommand = pathfindingCommand;
                }
                catch (Exception e)
                {
                    Log.ErrorFormat($"[{Name}-Message] \"FindPath_Result\" was unable to GetInput.");
                    return MessageResult.Unprocessed;
                }
            }

            if (id == "Next_Selected_Walk_Position")
            {
                try
                {
                    var location = message.GetInput<Vector2i>();
                    _data.NextWalkPosition = location;
                }
                catch (Exception e)
                {
                    Log.ErrorFormat($"[{Name}-Message] \"Next_Selected_Walk_Position\" was unable to GetInput.");
                    return MessageResult.Unprocessed;
                }
            }

            if (id == "Raycast_Result")
            {
                try
                {
                    var origin = message.GetInput<Vector2i>(0);
                    var destination = message.GetInput<Vector2i>(1);
                    var result = message.GetInput<bool>(2);
                    var hitpoint = message.GetInput<Vector2i>(3);
                    _data.ClearRaycastData();
                    _data.AddRaycastData(new RaycastData(origin, destination, result, hitpoint));
                }
                catch (Exception e)
                {
                    Log.ErrorFormat($"[{Name}-Message] \"Raycast_Result\" was unable to GetInput.");
                    return MessageResult.Unprocessed;
                }
            }
            return MessageResult.Unprocessed;
        }

        #endregion

        #region Implementation of IEnableable

        /// <summary> The plugin is being enabled.</summary>
        public void Enable()
        {
            LokiPoe.OnGuiTick += LokiPoeOnOnGuiTick;
        }

        /// <summary> The plugin is being disabled.</summary>
        public void Disable()
        {
            LokiPoe.OnGuiTick -= LokiPoeOnOnGuiTick;
        }

        #endregion

        #region Override of Object

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Name + ": " + Description;
        }

        #endregion

        private void LokiPoeOnOnGuiTick(object sender, GuiTickEventArgs guiTickEventArgs)
        {
            if (!BotManager.IsRunning)
            {
                _data.Update();
            }
        }
	}
}
