using System;
using System.Collections.Generic;
using System.ComponentModel;
using DreamPoeBot.Loki;
using DreamPoeBot.Loki.Common;
using Newtonsoft.Json;

namespace ExamplePlugin
{
    class ExamplePluginSettings : JsonSettings
    {
        private static ExamplePluginSettings _instance;
        public static ExamplePluginSettings Instance => _instance ?? (_instance = new ExamplePluginSettings());

        public ExamplePluginSettings()
            : base(GetSettingsFilePath(Configuration.Instance.Name, "ExamplePlugin.json"))
        {
            // You should initialize your collection in here, the component will read the json file and set the old settings
            // and after that execute this lines, so on first execution or adding new settings to future versions, you should always initialize collection to ensure they are not null.
            // Ex:
            // if (MyListOfSecretClass == null)
            // {
            //    MyListOfSecretClass = new ObservableCollection<SecretClass>();
            // }
        }

        private int _health;
        private int _istantHealth;
        private int _mana;
        private int _istantMana;

        [DefaultValue(70)]
        public int Health
        {
            get {return _health;}
            set
            {
                _health = value;
                NotifyPropertyChanged(() => Health);
            }
        }
        [DefaultValue(50)]
        public int IstantHealth
        {
            get { return _istantHealth; }
            set
            {
                _istantHealth = value;
                NotifyPropertyChanged(() => IstantHealth);
            }
        }
        [DefaultValue(30)]
        public int Mana
        {
            get { return _mana; }
            set
            {
                _mana = value;
                NotifyPropertyChanged(() => Mana);
            }
        }
        [DefaultValue(10)]
        public int IstantMana
        {
            get { return _istantMana; }
            set
            {
                _istantMana = value;
                NotifyPropertyChanged(() => IstantMana);
            }
        }
    }
}
