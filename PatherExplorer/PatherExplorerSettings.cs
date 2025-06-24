using DreamPoeBot.Loki;
using DreamPoeBot.Loki.Common;

namespace PatherExplorer
{
    public class PatherExplorerSettings : JsonSettings
    {
        private static PatherExplorerSettings _instance;

        /// <summary>The current instance for this class. </summary>
        public static PatherExplorerSettings Instance => _instance ?? (_instance = new PatherExplorerSettings());

        /// <summary>The default ctor. Will use the settings path "AreaVisualizer".</summary>
        public PatherExplorerSettings()
            : base(GetSettingsFilePath(Configuration.Instance.Name, string.Format("{0}.json", "PatherExplorer")))
        {
        }
	}
}
