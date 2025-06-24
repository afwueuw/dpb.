using System.Collections.ObjectModel;
using System.ComponentModel;
using DreamPoeBot.Loki;
using DreamPoeBot.Loki.Common;

namespace ExampleMover
{
    public class StringWrapper
    {
        public string Value { get; set; }
    }
    class ExampleMoverSettings : JsonSettings
    {
        private static ExampleMoverSettings _instance;
        /// <summary>The current instance for this class. </summary>
        public static ExampleMoverSettings Instance
        {
            get { return _instance ?? (_instance = new ExampleMoverSettings()); }
        }

        /// <summary>The default ctor. Will use the settings path "QuestPlugin".</summary>
        public ExampleMoverSettings()
            : base(GetSettingsFilePath(Configuration.Instance.Name, string.Format("{0}.json", "ExampleMover")))
        {
            if (_forcedAdjustmentAreas == null)
            {
                _forcedAdjustmentAreas = new ObservableCollection<StringWrapper> {
                    new StringWrapper { Value = "The City of Sarn" },
                    new StringWrapper { Value = "The Slums" },
                    new StringWrapper { Value = "The Quay" },
                    new StringWrapper { Value = "The Toxic Conduits" } };
            }
        }

        private int _pathRefreshRateMs;
        private ObservableCollection<StringWrapper> _forcedAdjustmentAreas;
        private bool _forceAdjustCombatAreas;
        private bool _avoidWallHugging;
        private int _moveRange;
        private int _singleUseDistance;

        /// <summary>
        /// Should the area adjustments be used for all combat areas and not just ForcedAdjustmentAreas?
        /// </summary>
        [DefaultValue(false)]
        public bool ForceAdjustCombatAreas
        {
            get { return _forceAdjustCombatAreas; }
            set
            {
                if (value.Equals(_forceAdjustCombatAreas))
                {
                    return;
                }
                _forceAdjustCombatAreas = value;
                NotifyPropertyChanged(() => ForceAdjustCombatAreas);
            }
        }

        /// <summary>
        /// A list of areas to force movement adjustments on.
        /// </summary>
        public ObservableCollection<StringWrapper> ForcedAdjustmentAreas
        {
            get
            {
                return _forcedAdjustmentAreas;
            }
            set
            {
                if (value.Equals(_forcedAdjustmentAreas))
                {
                    return;
                }
                _forcedAdjustmentAreas = value;
                NotifyPropertyChanged(() => ForcedAdjustmentAreas);
            }
        }

        /// <summary>
        /// The time in ms to refresh a path that was generated.
        /// </summary>
        [DefaultValue(30)]
        public int PathRefreshRateMs
        {
            get { return _pathRefreshRateMs; }
            set
            {
                if (value.Equals(_pathRefreshRateMs))
                {
                    return;
                }
                _pathRefreshRateMs = value;
                NotifyPropertyChanged(() => PathRefreshRateMs);
            }
        }

        [DefaultValue(true)]
        public bool AvoidWallHugging
        {
            get { return _avoidWallHugging; }
            set
            {
                if (value.Equals(_avoidWallHugging))
                {
                    return;
                }
                _avoidWallHugging = value;
                NotifyPropertyChanged(() => AvoidWallHugging);
            }
        }

        [DefaultValue(35)]
        public int MoveRange
        {
            get { return _moveRange; }
            set
            {
                if (value.Equals(_moveRange))
                {
                    return;
                }
                _moveRange = value;
                NotifyPropertyChanged(() => MoveRange);
            }
        }

        [DefaultValue(18)]
        public int SingleUseDistance
        {
            get { return _singleUseDistance; }
            set
            {
                if (value.Equals(_singleUseDistance))
                {
                    return;
                }
                _singleUseDistance = value;
                NotifyPropertyChanged(() => SingleUseDistance);
            }
        }
    }
}
