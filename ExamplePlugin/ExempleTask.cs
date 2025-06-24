using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Game;
using DreamPoeBot.Loki.Game.GameData;
using DreamPoeBot.Loki.Game.Objects;
using FlaskHud = DreamPoeBot.Loki.Game.LokiPoe.InGameState.QuickFlaskHud;

namespace ExamplePlugin
{
    public class ExempleTask : ITask
    {
        Stopwatch limiterSw = new Stopwatch();
        public string Author => "Alcor75";
        public string Description => "An Example Task";
        public string Name => "ExempleTask";
        public string Version => "1.0";
        public async Task<LogicResult> Logic(Logic logic)
        {
            return LogicResult.Unprovided;
        }

        private void Use(FlaskType flaskType, string reason)
        {
            switch (flaskType)
            {
                case FlaskType.LifeFlask:
                    Use(Flasks.LifeFlask, reason);
                    break;
                case FlaskType.InstantLifeFlask:
                    Use(Flasks.InstantLifeFlask, reason);
                    break;
                case FlaskType.ManaFlask:
                    Use(Flasks.ManaFlask, reason);
                    break;
                case FlaskType.IstantManaFlask:
                    Use(Flasks.InstantManaFlask, reason);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flaskType), flaskType, null);
            }
        }
        private static void Use(Item flask, string reason)
        {
            if (flask == null) return;

            var slot = flask.LocationTopLeft.X + 1;

            ExamplePlugin.Log.InfoFormat($"[ExamplePlugin] Using {flask.ProperName()} (slot {slot}) because {reason}.");

            if (LokiPoe.ProcessHookManager.IsEnabled)
            {
                if (!FlaskHud.UseFlaskInSlot(slot))
                    ExamplePlugin.Log.ErrorFormat($"[ExamplePlugin] UseFlaskInSlot returned false for slot {slot}.");
            }
            else
            {
                switch (slot)
                {
                    case 1:
                        LokiPoe.Input.SimulateKeyEvent(System.Windows.Forms.Keys.D1, true, false, false);
                        break;
                    case 2:
                        LokiPoe.Input.SimulateKeyEvent(System.Windows.Forms.Keys.D2, true, false, false);
                        break;
                    case 3:
                        LokiPoe.Input.SimulateKeyEvent(System.Windows.Forms.Keys.D3, true, false, false);
                        break;
                    case 4:
                        LokiPoe.Input.SimulateKeyEvent(System.Windows.Forms.Keys.D4, true, false, false);
                        break;
                    case 5:
                        LokiPoe.Input.SimulateKeyEvent(System.Windows.Forms.Keys.D5, true, false, false);
                        break;
                }
            }

        }
        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public void Start()
        {
            limiterSw = Stopwatch.StartNew();
        }

        public void Stop()
        {
            limiterSw.Stop();
        }

        public void Tick()
        {
            if (limiterSw.ElapsedMilliseconds < 100) return;
            limiterSw.Restart();

            if (!LokiPoe.IsInGame || !LokiPoe.CurrentWorldArea.IsCombatArea || LokiPoe.Me.IsDead) return;

            var hpPercent = LokiPoe.Me.HealthPercent;
            var manaPercent = LokiPoe.Me.ManaPercent;
            if (hpPercent < ExamplePluginSettings.Instance.Health && !LokiPoe.Me.HasAura(Constants.LifeFlaskEffect))
            {
                Use(FlaskType.LifeFlask, $"we are at {hpPercent}% HP");
            }
            if (hpPercent < ExamplePluginSettings.Instance.IstantHealth)
            {
                Use(FlaskType.InstantLifeFlask, $"we are at {hpPercent}% HP");
            }
            if (manaPercent < ExamplePluginSettings.Instance.Mana && !LokiPoe.Me.HasAura(Constants.ManaFlaskEffect))
            {
                Use(FlaskType.ManaFlask, $"we are at {manaPercent}% Mana");
            }
            if (manaPercent < ExamplePluginSettings.Instance.IstantMana)
            {
                Use(FlaskType.IstantManaFlask, $"we are at {manaPercent}% Mana");
            }
        }

        public async Task<bool> Run()
        {
            return false;
        }
        private enum FlaskType
        {
            LifeFlask,
            InstantLifeFlask,
            ManaFlask,
            IstantManaFlask
        }

        public static class Constants
        {
            public const int MoveSkillId = 10505;

            public const string GracePeriod = "grace_period";

            public const string LifeFlaskEffect = "flask_effect_life";
            public const string ManaFlaskEffect = "flask_effect_mana";
            public const string QsilverEffect = "flask_utility_sprint";

            public const string FreezeEffect = "frozen";
            public const string ShockEffect = "shocked";
            public const string IgniteEffect = "ignited";
            public const string PoisonEffect = "poison";
            public const string SilenceEffect = "curse_silence";
            public const string BleedMovementEffect = "bleeding_moving";

            public const StatTypeGGG AntiFreezeStat = StatTypeGGG.LocalFlaskChillOrFreezeImmunityIfChilledOrFrozen;
            public const StatTypeGGG AntiIgniteStat = StatTypeGGG.LocalFlaskIgniteImmunityIfIgnitedAndRemoveBurning;
            public const StatTypeGGG AntiShockStat = StatTypeGGG.LocalFlaskShockImmunityIfShocked;
            public const StatTypeGGG AntiBleedStat = StatTypeGGG.LocalFlaskBleedingImmunityIfBleedingAndRemoveCorruptedBlood;
            public const StatTypeGGG AntiPoisonStat = StatTypeGGG.LocalFlaskPoisonImmunityIfPoisoned;
            public const StatTypeGGG AntiCurseStat = StatTypeGGG.LocalFlaskRemoveCursesOnUse;
        }
    }
}
