using System.Collections.ObjectModel;
using System.ComponentModel;
using DreamPoeBot.Loki;
using DreamPoeBot.Loki.Common;

namespace SkillPlayerMover
{
    public class StringWrapper
    {
        public string Value { get; set; }
    }
    class SkillPlayerMoverSettings : JsonSettings
    {
        private static SkillPlayerMoverSettings _instance;
        /// <summary>The current instance for this class. </summary>
        public static SkillPlayerMoverSettings Instance
        {
            get { return _instance ?? (_instance = new SkillPlayerMoverSettings()); }
        }

        /// <summary>The default ctor. Will use the settings path "QuestPlugin".</summary>
        public SkillPlayerMoverSettings()
            : base(GetSettingsFilePath(Configuration.Instance.Name, string.Format("{0}.json", "SkillPlayerMover")))
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
        private bool _debugAdjustments;
        
        private bool _debugInputApi;
        private bool _avoidWallHugging;
        private int _moveRange;
        private int _singleUseDistance;

        private int _moveMinMana;

        private bool _ignoreMobs;

        private bool _useBloodMagicValue;

        private bool _enableLwValue;
        private int _lwMinDist;
        private int _lwMaxDist;

        private bool _enablePrValue;
        private int _prMinDist;
        private int _prMaxDist;

        private bool _enableLsValue;
        private int _lsMinDist;
        private int _lsMaxDist;

        private bool _enableFdValue;
        private int _fdMinDist;
        private int _fdMaxDist;

        private bool _enableBaValue;
        private int _baMinDist;
        private int _baMaxDist;

        private bool _enableWbValue;
        private int _wbMinDist;
        private int _wbMaxDist;

        private bool _enableScValue;
        private int _scMinDist;
        private int _scMaxDist;

        private bool _enableFrValue;
        private int _frMinDist;
        private int _frMaxDist;

        private bool _enableWsValue;
        private int _wsMinDist;
        private int _wsMaxDist;

        private bool _enableFbValue;
        private int _fbMinDist;
        private int _fbMaxDist;

        private bool _enableDsValue;
        private int _dsMinDist;
        private int _dsMaxDist;

        private bool _enableBsValue;
        private int _bsMinDist;
        private int _bsMaxDist;

        #region Basic
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
        [DefaultValue(32)]
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

        /// <summary>
        /// Should the area adjustments be used for all combat areas and not just ForcedAdjustmentAreas?
        /// </summary>
        [DefaultValue(false)]
        public bool DebugAdjustments
        {
            get { return _debugAdjustments; }
            set
            {
                if (value.Equals(_debugAdjustments))
                {
                    return;
                }
                _debugAdjustments = value;
                NotifyPropertyChanged(() => ForceAdjustCombatAreas);
            }
        }

        [DefaultValue(false)]
        public bool DebugInputApi
        {
            get { return _debugInputApi; }
            set
            {
                if (value.Equals(_debugInputApi))
                {
                    return;
                }
                _debugInputApi = value;
                NotifyPropertyChanged(() => DebugInputApi);
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

        [DefaultValue(33)]
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
        #endregion
        [DefaultValue(20)]
        public int MoveMinManaValue
        {
            get { return _moveMinMana; }
            set
            {
                if (value.Equals(_moveMinMana))
                {
                    return;
                }
                _moveMinMana = value;
                NotifyPropertyChanged(() => MoveMinManaValue);
            }
        }


        [DefaultValue(false)]
        public bool IgnoreMobsValue
        {
            get { return _ignoreMobs; }
            set
            {
                if (value.Equals(_ignoreMobs))
                {
                    return;
                }
                _ignoreMobs = value;
                NotifyPropertyChanged(() => IgnoreMobsValue);
            }
        }

        [DefaultValue(false)]
        public bool UseBloodMagicValue
        {
            get { return _useBloodMagicValue; }
            set
            {
                if (value.Equals(_useBloodMagicValue))
                {
                    return;
                }
                _useBloodMagicValue = value;
                NotifyPropertyChanged(() => UseBloodMagicValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableLwValue
        {
            get { return _enableLwValue; }
            set
            {
                if (value.Equals(_enableLwValue))
                {
                    return;
                }
                _enableLwValue = value;
                NotifyPropertyChanged(() => EnableLwValue);
            }
        }

        [DefaultValue(20)]
        public int LwMinDistValue
        {
            get { return _lwMinDist; }
            set
            {
                if (value.Equals(_lwMinDist))
                {
                    return;
                }
                _lwMinDist = value;
                NotifyPropertyChanged(() => LwMinDistValue);
            }
        }
        [DefaultValue(65)]
        public int LwMaxDistValue
        {
            get { return _lwMaxDist; }
            set
            {
                if (value.Equals(_lwMaxDist))
                {
                    return;
                }
                _lwMaxDist = value;
                NotifyPropertyChanged(() => LwMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableBaValue
        {
            get { return _enableBaValue; }
            set
            {
                if (value.Equals(_enableBaValue))
                {
                    return;
                }
                _enableBaValue = value;
                NotifyPropertyChanged(() => EnableBaValue);
            }
        }

        [DefaultValue(20)]
        public int BaMinDistValue
        {
            get { return _baMinDist; }
            set
            {
                if (value.Equals(_baMinDist))
                {
                    return;
                }
                _baMinDist = value;
                NotifyPropertyChanged(() => BaMinDistValue);
            }
        }

        [DefaultValue(65)]
        public int BaMaxDistValue
        {
            get { return _baMaxDist; }
            set
            {
                if (value.Equals(_baMaxDist))
                {
                    return;
                }
                _baMaxDist = value;
                NotifyPropertyChanged(() => BaMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableFdValue
        {
            get { return _enableFdValue; }
            set
            {
                if (value.Equals(_enableFdValue))
                {
                    return;
                }
                _enableFdValue = value;
                NotifyPropertyChanged(() => EnableFdValue);
            }
        }

        [DefaultValue(27)]
        public int FdMinDistValue
        {
            get { return _fdMinDist; }
            set
            {
                if (value.Equals(_fdMinDist))
                {
                    return;
                }
                _fdMinDist = value;
                NotifyPropertyChanged(() => FdMinDistValue);
            }
        }

        [DefaultValue(65)]
        public int FdMaxDistValue
        {
            get { return _fdMaxDist; }
            set
            {
                if (value.Equals(_fdMaxDist))
                {
                    return;
                }
                _fdMaxDist = value;
                NotifyPropertyChanged(() => FdMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableLsValue
        {
            get { return _enableLsValue; }
            set
            {
                if (value.Equals(_enableLsValue))
                {
                    return;
                }
                _enableLsValue = value;
                NotifyPropertyChanged(() => EnableLsValue);
            }
        }

        [DefaultValue(25)]
        public int LsMinDistValue
        {
            get { return _lsMinDist; }
            set
            {
                if (value.Equals(_lsMinDist))
                {
                    return;
                }
                _lsMinDist = value;
                NotifyPropertyChanged(() => LsMinDistValue);
            }
        }

        [DefaultValue(65)]
        public int LsMaxDistValue
        {
            get { return _lsMaxDist; }
            set
            {
                if (value.Equals(_lsMaxDist))
                {
                    return;
                }
                _lsMaxDist = value;
                NotifyPropertyChanged(() => LsMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnablePrValue
        {
            get { return _enablePrValue; }
            set
            {
                if (value.Equals(_enablePrValue))
                {
                    return;
                }
                _enablePrValue = value;
                NotifyPropertyChanged(() => EnablePrValue);
            }
        }

        [DefaultValue(1)]
        public int PrMinDistValue
        {
            get { return _prMinDist; }
            set
            {
                if (value.Equals(_prMinDist))
                {
                    return;
                }
                _prMinDist = value;
                NotifyPropertyChanged(() => PrMinDistValue);
            }
        }

        [DefaultValue(120)]
        public int PrMaxDistValue
        {
            get { return _prMaxDist; }
            set
            {
                if (value.Equals(_prMaxDist))
                {
                    return;
                }
                _prMaxDist = value;
                NotifyPropertyChanged(() => PrMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableWbValue
        {
            get { return _enableWbValue; }
            set
            {
                if (value.Equals(_enableWbValue))
                {
                    return;
                }
                _enableWbValue = value;
                NotifyPropertyChanged(() => EnableWbValue);
            }
        }

        [DefaultValue(25)]
        public int WbMinDistValue
        {
            get { return _wbMinDist; }
            set
            {
                if (value.Equals(_wbMinDist))
                {
                    return;
                }
                _wbMinDist = value;
                NotifyPropertyChanged(() => WbMinDistValue);
            }
        }

        [DefaultValue(65)]
        public int WbMaxDistValue
        {
            get { return _wbMaxDist; }
            set
            {
                if (value.Equals(_wbMaxDist))
                {
                    return;
                }
                _wbMaxDist = value;
                NotifyPropertyChanged(() => WbMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableScValue
        {
            get { return _enableScValue; }
            set
            {
                if (value.Equals(_enableScValue))
                {
                    return;
                }
                _enableScValue = value;
                NotifyPropertyChanged(() => EnableScValue);
            }
        }

        [DefaultValue(27)]
        public int ScMinDistValue
        {
            get { return _scMinDist; }
            set
            {
                if (value.Equals(_scMinDist))
                {
                    return;
                }
                _scMinDist = value;
                NotifyPropertyChanged(() => ScMinDistValue);
            }
        }

        [DefaultValue(65)]
        public int ScMaxDistValue
        {
            get { return _scMaxDist; }
            set
            {
                if (value.Equals(_scMaxDist))
                {
                    return;
                }
                _scMaxDist = value;
                NotifyPropertyChanged(() => ScMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableFrValue
        {
            get { return _enableFrValue; }
            set
            {
                if (value.Equals(_enableFrValue))
                {
                    return;
                }
                _enableFrValue = value;
                NotifyPropertyChanged(() => EnableFrValue);
            }
        }

        [DefaultValue(20)]
        public int FrMinDistValue
        {
            get { return _frMinDist; }
            set
            {
                if (value.Equals(_frMinDist))
                {
                    return;
                }
                _frMinDist = value;
                NotifyPropertyChanged(() => FrMinDistValue);
            }
        }

        [DefaultValue(65)]
        public int FrMaxDistValue
        {
            get { return _frMaxDist; }
            set
            {
                if (value.Equals(_frMaxDist))
                {
                    return;
                }
                _frMaxDist = value;
                NotifyPropertyChanged(() => FrMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableWsValue
        {
            get { return _enableWsValue; }
            set
            {
                if (value.Equals(_enableWsValue))
                {
                    return;
                }
                _enableWsValue = value;
                NotifyPropertyChanged(() => EnableWsValue);
            }
        }

        [DefaultValue(1)]
        public int WsMinDistValue
        {
            get { return _wsMinDist; }
            set
            {
                if (value.Equals(_wsMinDist))
                {
                    return;
                }
                _wsMinDist = value;
                NotifyPropertyChanged(() => WsMinDistValue);
            }
        }

        [DefaultValue(120)]
        public int WsMaxDistValue
        {
            get { return _wsMaxDist; }
            set
            {
                if (value.Equals(_wsMaxDist))
                {
                    return;
                }
                _wsMaxDist = value;
                NotifyPropertyChanged(() => WsMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableFbValue
        {
            get { return _enableFbValue; }
            set
            {
                if (value.Equals(_enableFbValue))
                {
                    return;
                }
                _enableFbValue = value;
                NotifyPropertyChanged(() => EnableFbValue);
            }
        }
        [DefaultValue(33)]
        public int FbMinDistValue
        {
            get { return _fbMinDist; }
            set
            {
                if (value.Equals(_fbMinDist))
                {
                    return;
                }
                _fbMinDist = value;
                NotifyPropertyChanged(() => FbMinDistValue);
            }
        }

        [DefaultValue(65)]
        public int FbMaxDistValue
        {
            get { return _fbMaxDist; }
            set
            {
                if (value.Equals(_fbMaxDist))
                {
                    return;
                }
                _fbMaxDist = value;
                NotifyPropertyChanged(() => FbMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableBsValue
        {
            get { return _enableBsValue; }
            set
            {
                if (value.Equals(_enableBsValue))
                {
                    return;
                }
                _enableBsValue = value;
                NotifyPropertyChanged(() => EnableBsValue);
            }
        }
        [DefaultValue(33)]
        public int BsMinDistValue
        {
            get { return _bsMinDist; }
            set
            {
                if (value.Equals(_bsMinDist))
                {
                    return;
                }
                _bsMinDist = value;
                NotifyPropertyChanged(() => BsMinDistValue);
            }
        }

        [DefaultValue(65)]
        public int BsMaxDistValue
        {
            get { return _bsMaxDist; }
            set
            {
                if (value.Equals(_bsMaxDist))
                {
                    return;
                }
                _bsMaxDist = value;
                NotifyPropertyChanged(() => BsMaxDistValue);
            }
        }

        [DefaultValue(false)]
        public bool EnableDsValue
        {
            get { return _enableDsValue; }
            set
            {
                if (value.Equals(_enableDsValue))
                {
                    return;
                }
                _enableDsValue = value;
                NotifyPropertyChanged(() => EnableDsValue);
            }
        }
        [DefaultValue(33)]
        public int DsMinDistValue
        {
            get { return _dsMinDist; }
            set
            {
                if (value.Equals(_dsMinDist))
                {
                    return;
                }
                _dsMinDist = value;
                NotifyPropertyChanged(() => DsMinDistValue);
            }
        }

        [DefaultValue(65)]
        public int DsMaxDistValue
        {
            get { return _dsMaxDist; }
            set
            {
                if (value.Equals(_dsMaxDist))
                {
                    return;
                }
                _dsMaxDist = value;
                NotifyPropertyChanged(() => DsMaxDistValue);
            }
        }
    }
}
