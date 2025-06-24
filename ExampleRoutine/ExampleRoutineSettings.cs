using System;
using System.Collections.Generic;
using System.ComponentModel;
using DreamPoeBot.Loki;
using DreamPoeBot.Loki.Common;
using Newtonsoft.Json;

namespace ExampleRoutine
{
    class ExampleRoutineSettings : JsonSettings
    {
        private static ExampleRoutineSettings _instance;
        public static ExampleRoutineSettings Instance => _instance ?? (_instance = new ExampleRoutineSettings());

        private ExampleRoutineSettings()
            : base(GetSettingsFilePath(Configuration.Instance.Name, "ExampleRoutine.json"))
        {
            // You should initialize your collection in here, the component will read the json file and set the old settings
            // and after that execute this lines, so on first execution or adding new settings to future versions, you should always initialize collection to ensure they are not null.
            // Ex:
            // if (MyListOfSecretClass == null)
            // {
            //    MyListOfSecretClass = new ObservableCollection<SecretClass>();
            // }
        }

        private int _singleTargetMeleeSlot;
        private int _singleTargetRangedSlot;
        private int _aoeMeleeSlot;
        private int _aoeRangedSlot;
        private int _fallbackSlot;
        private int _combatRange;
        private int _maxMeleeRange;
        private int _maxRangeRange;
        private bool _skipShrines;
        private int _aoePackSize;
        private bool _alwaysAttackInPlace;

        /// <summary>
        /// The skill slot to use in melee range.
        /// </summary>
        [DefaultValue(-1)]
        public int SingleTargetMeleeSlot
        {
            get { return _singleTargetMeleeSlot; }
            set
            {
                if (value.Equals(_singleTargetMeleeSlot))
                {
                    return;
                }
                _singleTargetMeleeSlot = value;
                NotifyPropertyChanged(() => SingleTargetMeleeSlot);
            }
        }
        /// <summary>
        /// The skill slot to use outside of melee range.
        /// </summary>
        [DefaultValue(-1)]
        public int SingleTargetRangedSlot
        {
            get { return _singleTargetRangedSlot; }
            set
            {
                if (value.Equals(_singleTargetRangedSlot))
                {
                    return;
                }
                _singleTargetRangedSlot = value;
                NotifyPropertyChanged(() => SingleTargetRangedSlot);
            }
        }

        /// <summary>
        /// The skill slot to use in melee range.
        /// </summary>
        [DefaultValue(-1)]
        public int AoeMeleeSlot
        {
            get { return _aoeMeleeSlot; }
            set
            {
                if (value.Equals(_aoeMeleeSlot))
                {
                    return;
                }
                _aoeMeleeSlot = value;
                NotifyPropertyChanged(() => AoeMeleeSlot);
            }
        }
        /// <summary>
        /// The skill slot to use outside of melee range.
        /// </summary>
        [DefaultValue(-1)]
        public int AoeRangedSlot
        {
            get { return _aoeRangedSlot; }
            set
            {
                if (value.Equals(_aoeRangedSlot))
                {
                    return;
                }
                _aoeRangedSlot = value;
                NotifyPropertyChanged(() => AoeRangedSlot);
            }
        }

        /// <summary>
        /// The skill slot to use as a fallback if the desired skill cannot be cast.
        /// </summary>
        [DefaultValue(-1)]
        public int FallbackSlot
        {
            get { return _fallbackSlot; }
            set
            {
                if (value.Equals(_fallbackSlot))
                {
                    return;
                }
                _fallbackSlot = value;
                NotifyPropertyChanged(() => FallbackSlot);
            }
        }

        /// <summary>
        /// Only attack mobs within this range.
        /// </summary>
        [DefaultValue(70)]
        public int CombatRange
        {
            get { return _combatRange; }
            set
            {
                if (value.Equals(_combatRange))
                {
                    return;
                }
                _combatRange = value;
                NotifyPropertyChanged(() => CombatRange);
            }
        }

        /// <summary>
        /// How close does a mob need to be to trigger the Melee skill.
        /// Do not set too high, as the cursor will overlap the GUI.
        /// </summary>
        [DefaultValue(20)]
        public int MaxMeleeRange
        {
            get { return _maxMeleeRange; }
            set
            {
                if (value.Equals(_maxMeleeRange))
                {
                    return;
                }
                _maxMeleeRange = value;
                NotifyPropertyChanged(() => MaxMeleeRange);
            }
        }

        /// <summary>
        /// How close does a mob need to be to trigger the Ranged skill.
        /// Do not set too high, as the cursor will overlap the GUI.
        /// </summary>
        [DefaultValue(40)]
        public int MaxRangeRange
        {
            get { return _maxRangeRange; }
            set
            {
                if (value.Equals(_maxRangeRange))
                {
                    return;
                }
                _maxRangeRange = value;
                NotifyPropertyChanged(() => MaxRangeRange);
            }
        }

        [DefaultValue(false)]
        public bool SkipShrines
        {
            get { return _skipShrines; }
            set
            {
                _skipShrines = value;
                NotifyPropertyChanged(() => SkipShrines);
            }
        }

        /// <summary>
        /// Number of mobs near the best target to use AOE skill.
        /// </summary>
        [DefaultValue(3)]
        public int AoePackSize
        {
            get { return _aoePackSize; }
            set
            {
                if (value.Equals(_aoePackSize))
                {
                    return;
                }
                _aoePackSize = value;
                NotifyPropertyChanged(() => AoePackSize);
            }
        }

        /// <summary>
        /// Should the CR always attack in place.
        /// </summary>
        [DefaultValue(false)]
        public bool AlwaysAttackInPlace
        {
            get { return _alwaysAttackInPlace; }
            set
            {
                if (value.Equals(_alwaysAttackInPlace))
                {
                    return;
                }
                _alwaysAttackInPlace = value;
                NotifyPropertyChanged(() => AlwaysAttackInPlace);
            }
        }

        // Settings static types mostly used in the Gui
        [JsonIgnore] private static List<int> _allSkillSlots;

        /// <summary>List of all available skill slots </summary>
        [JsonIgnore]
        public static List<int> AllSkillSlots => _allSkillSlots ?? (_allSkillSlots = new List<int>
        {
            -1,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13
        });
    }
}
