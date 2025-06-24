using System;
using DreamPoeBot.Loki;
using DreamPoeBot.Loki.Common;

namespace NullBot
{
    public class NullBotSettings : JsonSettings
    {
        private static NullBotSettings _instance;
        public static NullBotSettings Instance => _instance ?? (_instance = new NullBotSettings());

        private NullBotSettings()
            : base(GetSettingsFilePath(Configuration.Instance.Name, "NullBot.json"))
        {

        }

        private bool _shouldEnableHooks;
        private bool _shouldEnableCombat;

        public bool ShouldEnableHooks
        {
            get { return _shouldEnableHooks; }
            set
            {
                _shouldEnableHooks = value;
                NotifyPropertyChanged(() => ShouldEnableHooks);
            }
        }
        public bool ShouldEnableCombat
        {
            get { return _shouldEnableCombat; }
            set
            {
                _shouldEnableCombat = value;
                NotifyPropertyChanged(() => ShouldEnableCombat);
            }
        }
    }
}
