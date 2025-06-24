using DreamPoeBot.Common;

namespace SkillPlayerMover.Helpers
{
    public static class MoveHelper
    {
        public static bool SafePath(Vector2i destination, int combatRange)
        {
            return MiscHelpers.NumberOfMobsNearPosition(destination, combatRange) <= 0;
        }
        
        public static int GetSkillMinRange(string skillName)
        {
            switch (skillName)
            {
                case "Phase Run":
                    return SkillPlayerMoverSettings.Instance.PrMinDistValue;

                case "Frenzy":
                    return SkillPlayerMoverSettings.Instance.FrMinDistValue;

                case "Flame Dash":
                    return SkillPlayerMoverSettings.Instance.FdMinDistValue;
             
                case "Lightning Warp":
                    return SkillPlayerMoverSettings.Instance.LwMinDistValue;
                   
                case "Blink Arrow":
                    return SkillPlayerMoverSettings.Instance.BaMinDistValue;
                   
                case "Leap Slam":
                    return SkillPlayerMoverSettings.Instance.LsMinDistValue;
                   
                case "Whirling Blades":
                    return SkillPlayerMoverSettings.Instance.WbMinDistValue;

                case "Shield Charge":
                    return SkillPlayerMoverSettings.Instance.ScMinDistValue;

                case "Dash":
                    return SkillPlayerMoverSettings.Instance.DsMinDistValue;

                case "Withering Step":
                    return SkillPlayerMoverSettings.Instance.WsMinDistValue;

                case "Frostblink":
                    return SkillPlayerMoverSettings.Instance.FbMinDistValue;

                case "Bodyswap":
                    return SkillPlayerMoverSettings.Instance.BsMinDistValue;

                default:
                    return 1;
            }
        }
        public static int GetSkillMaxRange(string skillName)
        {
            switch (skillName)
            {
                case "Phase Run":
                    return SkillPlayerMoverSettings.Instance.PrMaxDistValue;

                case "Frenzy":
                    return SkillPlayerMoverSettings.Instance.FrMaxDistValue;

                case "Flame Dash":
                    return SkillPlayerMoverSettings.Instance.FdMaxDistValue;

                case "Lightning Warp":
                    return SkillPlayerMoverSettings.Instance.LwMaxDistValue;

                case "Blink Arrow":
                    return SkillPlayerMoverSettings.Instance.BaMaxDistValue;

                case "Leap Slam":
                    return SkillPlayerMoverSettings.Instance.LsMaxDistValue;

                case "Whirling Blades":
                    return SkillPlayerMoverSettings.Instance.WbMaxDistValue;

                case "Shield Charge":
                    return SkillPlayerMoverSettings.Instance.ScMaxDistValue;

                case "Dash":
                    return SkillPlayerMoverSettings.Instance.DsMaxDistValue;

                case "Withering Step":
                    return SkillPlayerMoverSettings.Instance.WsMaxDistValue;

                case "Frostblink":
                    return SkillPlayerMoverSettings.Instance.FbMaxDistValue;

                case "Bodyswap":
                    return SkillPlayerMoverSettings.Instance.BsMaxDistValue;

                default:
                    return 70;
            }
        }
    }
}
