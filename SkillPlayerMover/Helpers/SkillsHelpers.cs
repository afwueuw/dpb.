using System;
using System.Linq;
using System.Threading;
using DreamPoeBot.BotFramework;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Game;
using DreamPoeBot.Loki.Game.Objects;

namespace SkillPlayerMover.Helpers
{
    static class SkillsHelpers
    {
        public static bool CastAttackMovementSkill(int moveSkillSlot, Vector2i skillTargetPoint, string skillName)
        {
            var moveOnly = LokiPoe.InGameState.SkillBarHud.LastBoundMoveSkill.Slot;
            var useResult = LokiPoe.InGameState.SkillBarHud.UseAt(moveSkillSlot, false, skillTargetPoint);
            //Thread.Sleep(castTime);
            if (useResult != LokiPoe.InGameState.UseResult.None)
            {
                SkillPlayerMover.Log.WarnFormat("[SkillPlayerMover-CastWirlingBlade] {0} UseAt returned {1}.", skillName, useResult);
                return false;
            }

            LokiPoe.InGameState.SkillBarHud.UseAt(moveOnly, false, skillTargetPoint);

            return true;
        }

        /// <summary>
        /// Use the movement skill at the given slot in an attempt to move toward the given position.
        /// In the case of Phase Run, Blink Arrow, and Lightning Warp, this is a multi-frame process that will internally break free of a framelock.
        /// </summary>
        /// <param name="skillSlot"></param>
        /// <param name="skillTargetPoint"></param>
        /// <returns>"true" if use of the given skill was successful, otherwise "false".</returns>
        public static bool UseSkillAwareMovementSkill(int skillSlot, Vector2i skillTargetPoint, Vector2i finalDestination, Vector2i myposition)
        {
            if (skillTargetPoint == Vector2i.Zero) return false;

            const string phaseRunSkillName = "Phase Run";
            const string blinkArrowSkillName = "Blink Arrow";
            const string lightningWarpSkillName = "Lightning Warp";
            const string bodyswapSkillName = "Bodyswap";

            var skill = LokiPoe.InGameState.SkillBarHud.Slot(skillSlot);
            var skillName = skill.Name;
			var castTime = skill.CastTime;
            if (skill.Name == phaseRunSkillName)
            {
                if (LokiPoe.Me.Auras.All(x => x.Name != phaseRunSkillName))
                {
                    SkillPlayerMover.Log.Debug($"[SkillPlayerMover:UseMovementSkill] Using skill {skill.Name} at position {skillTargetPoint}.");
                    LokiPoe.InGameState.SkillBarHud.Use(skillSlot, false);
                }

                return false;
            }

            //var inPlace = skill.Name == "Flame Dash" || skill.Name == lightningWarpSkillName;
            var inPlace = false;
            var maxRange = MoveHelper.GetSkillMaxRange(skillName);
            var myPosition = myposition;
            LokiPoe.InGameState.UseResult useResult = LokiPoe.InGameState.UseResult.None;
            if (LokiPoe.Me.HasCurrentAction && skillName != blinkArrowSkillName && skillName != lightningWarpSkillName && skillName != bodyswapSkillName && (LokiPoe.ProcessHookManager.GetKeyState(skill.BoundKeys.Last()) & 0x8000) != 0)//0x8000) != 0)
            {
                // If I'm close enough to use it, use it at the exact point... otherwise just move the mcursor
                if (myPosition.Distance(finalDestination) <= maxRange)
                {
                    LokiPoe.ProcessHookManager.ClearAllKeyStates();
                    SkillPlayerMover.Log.Debug($"[SkillPlayerMover:UseMovementSkill] Using skill {skillName} at position {skillTargetPoint}.");                        
                    useResult = LokiPoe.InGameState.SkillBarHud.UseAt(skillSlot, inPlace, skillTargetPoint);
                }
                else
                {
                    MouseManager.SetMousePos("Mover.Logic", skillTargetPoint, false);
                }
            }
            else
            {
                LokiPoe.ProcessHookManager.ClearAllKeyStates();
                if (skillName != blinkArrowSkillName && skillName != lightningWarpSkillName && skillName != bodyswapSkillName && myPosition.Distance(finalDestination) <= maxRange)
                {
                    SkillPlayerMover.Log.Debug($"[SkillPlayerMover:UseMovementSkill] Using skill {skillName} at position {skillTargetPoint}.");                        
                    useResult = LokiPoe.InGameState.SkillBarHud.BeginUseAt(skillSlot, inPlace, skillTargetPoint);
                }
                else
                {
                    SkillPlayerMover.Log.Debug($"[SkillPlayerMover:UseMovementSkill] Using skill {skillName} at position {skillTargetPoint}.");                        
                    useResult = LokiPoe.InGameState.SkillBarHud.UseAt(skillSlot, inPlace, skillTargetPoint);
                }
            }
            if (skillName != "Flame Dash") Thread.Sleep(castTime);
            //useResult = LokiPoe.InGameState.SkillBarHud.UseAt(skillSlot, true, skillTargetPoint);
            if (useResult != LokiPoe.InGameState.UseResult.None)
            {
                SkillPlayerMover.Log.Debug($"[SkillPlayerMover:UseMovementSkill] {skillName} UseAt returned {useResult}.");                    
                return false;
            }
            return true;            
        }

        public static int SkillReady(Vector2i movePos, bool ignorMana = false)
        {
            int moveSkillSlot = -1;
            var listskill = LokiPoe.Me.AvailableSkills.Where(s => (
                                                                      (s.Name == "Phase Run" && SkillPlayerMoverSettings.Instance.EnablePrValue) ||
                                                                      (s.Name == "Withering Step" && SkillPlayerMoverSettings.Instance.EnableWsValue) ||
                                                                      (s.Name == "Frenzy" && SkillPlayerMoverSettings.Instance.EnableFrValue) ||
                                                                      (s.Name == "Lightning Warp" && SkillPlayerMoverSettings.Instance.EnableLwValue) ||
                                                                      (s.Name == "Blink Arrow" && SkillPlayerMoverSettings.Instance.EnableBaValue) ||
                                                                      (s.Name == "Flame Dash" && SkillPlayerMoverSettings.Instance.EnableFdValue) ||
                                                                      (s.Name == "Dash" && SkillPlayerMoverSettings.Instance.EnableDsValue) ||
                                                                      (s.Name == "Leap Slam" && SkillPlayerMoverSettings.Instance.EnableLsValue) ||
                                                                      (s.Name == "Whirling Blades" && SkillPlayerMoverSettings.Instance.EnableWbValue) ||
                                                                      (s.Name == "Shield Charge" && SkillPlayerMoverSettings.Instance.EnableScValue) ||
                                                                      (s.Name == "Frostblink" && SkillPlayerMoverSettings.Instance.EnableFbValue) ||
                                                                      (s.Name == "Bodyswap" && SkillPlayerMoverSettings.Instance.EnableBsValue)
                                                                  ) &&
                                                                  s.IsOnSkillBar);
            foreach (var skill in listskill)
            {
                moveSkillSlot = skill.Slot;
                if (CanUseSkillToMove(movePos, moveSkillSlot, ignorMana))
                {
                    return moveSkillSlot;
                }

                moveSkillSlot = -1;
            }

            return moveSkillSlot;
        }
        private static bool CanUseSkillToMove(Vector2i movePos, int skillSlot, bool ignoreManaCost = false)
        {
            var skill = LokiPoe.InGameState.SkillBarHud.Slot(skillSlot);
            if (skill.Name.Equals("Bodyswap") && !LokiPoe.ObjectManager.GetObjectsByType<Monster>().Any(m => m.Position.Distance(movePos) < 15)) return false;
            if (skill.IsOnCooldown && skill.UsesAvailable <= 0) return false;
            if (LokiPoe.Me.EquippedItems.Any(x => x.FullName == "Soul Taker")) return true;
            if (SkillPlayerMoverSettings.Instance.UseBloodMagicValue) return true;

            if (!ignoreManaCost && (LokiPoe.Me.ManaReservedPercent >= 100 || LokiPoe.Me.ManaPercent < SkillPlayerMoverSettings.Instance.MoveMinManaValue))
                return false;

            return skill.CanUse();

        }

    }
}
