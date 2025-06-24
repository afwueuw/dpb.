using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Game;
using DreamPoeBot.Loki.Game.GameData;
using DreamPoeBot.Loki.Game.Objects;
using FollowBot.Helpers;
using FollowBot.SimpleEXtensions;
using log4net;

namespace FollowBot
{
    class TravelToPartyZoneTask : ITask
    {
        private readonly ILog Log = Logger.GetLoggerInstanceForType();
        private bool _enabled = true;
        private Stopwatch _portalRequestStopwatch = Stopwatch.StartNew();
        private static int _zoneCheckRetry = 0;
        public static Stopwatch PortOutStopwatch = new Stopwatch();

        public string Name { get { return "TravelToPartyZone"; } }
        public string Description { get { return "This task will travel to party grind zone."; } }
        public string Author { get { return "NotYourFriend original from Unknown"; } }
        public string Version { get { return "0.0.0.1"; } }

        public void Start()
        {
            PortOutStopwatch.Reset();
        }
        public void Stop()
        {
            PortOutStopwatch.Reset();
        }
        public void Tick()
        {
        }

        public async Task<bool> Run()
        {
            if (!LokiPoe.IsInGame || LokiPoe.Me.IsDead)
            {
                return false;
            }

            await Coroutines.CloseBlockingWindows();

            var leader = LokiPoe.InstanceInfo.PartyMembers.FirstOrDefault(x => x.MemberStatus == PartyStatus.PartyLeader);
            if (leader == null) return false;
            var leaderPlayerEntry = leader.PlayerEntry;
            if (leaderPlayerEntry == null) return false;
            if (leaderPlayerEntry?.IsOnline != true)
            {
                GlobalLog.Warn($"Leader is not Online, probably loading.");
                return false;
            }

            var leadername = leaderPlayerEntry?.Name;
            var leaderArea = leaderPlayerEntry?.Area;
            if (string.IsNullOrEmpty(leadername) || leaderArea == null) return false;

            // "같은 존" 또는 "이름만 같아도" 둘 중 하나만 true면 즉시 리턴
            if (LokiPoe.InGameState.PartyHud.IsInSameZone(leadername) || World.CurrentArea.Name == leaderArea.Name)
            {
                _zoneCheckRetry = 0;
                PortOutStopwatch.Reset();
                return false;
            }
            else
            {
                _zoneCheckRetry++;
                if (_zoneCheckRetry < 2) // 1회만 재시도
                {
                    await Coroutines.LatencyWait();
                    GlobalLog.Warn($"IsInSameZone returned false for {leadername} retry [{_zoneCheckRetry}/1]");
                    return true;
                }
            }

            var curZone = World.CurrentArea;
            if (!curZone.IsTown && !curZone.IsHideoutArea && FollowBotSettings.Instance.DontPortOutofMap) return false;
            var delveportal = LokiPoe.ObjectManager.GetObjectsByType<AreaTransition>().FirstOrDefault(x => x.Name == "Azurite Mine" && (x.Metadata == "Metadata/MiscellaneousObject/PortalTransition" || x.Metadata == "Metadata/Terrain/Leagues/Delve/Objects/DelvePortal"));
            if (delveportal != null)
            {
                Log.DebugFormat("[{0}] Found walkable delve portal.", Name);
                if (LokiPoe.Me.Position.Distance(delveportal.Position) > 15)
                {
                    var walkablePosition = ExilePather.FastWalkablePositionFor(delveportal, 13);

                    FollowBot.PhaseRun();

                    Move.Towards(walkablePosition, "moving to delve portal");
                    return true;
                }

                var tele = await Coroutines.InteractWith(delveportal);

                if (!tele)
                {
                    Log.DebugFormat("[{0}] delve portal error.", Name);
                }

                FollowBot.Leader = null;
                return true;
            }
            var heistportal = LokiPoe.ObjectManager.GetObjectByMetadata("Metadata/Terrain/Leagues/Heist/Objects/MissionEntryPortal");
            if (heistportal != null && heistportal.Components.TargetableComponent.CanTarget)
            {
                Log.DebugFormat("[{0}] Found walkable heist portal.", Name);
                if (LokiPoe.Me.Position.Distance(heistportal.Position) > 20)
                {
                    var walkablePosition = ExilePather.FastWalkablePositionFor(heistportal, 20);

                    FollowBot.PhaseRun();

                    Move.Towards(walkablePosition, "moving to heist portal");
                    return true;
                }

                var tele = await Coroutines.InteractWith(heistportal);

                if (!tele)
                {
                    Log.DebugFormat("[{0}] heist portal error.", Name);
                }

                FollowBot.Leader = null;
                return true;
            }

            if (leaderArea.IsMap || leaderArea.IsTempleOfAtzoatl || leaderArea.Id.Contains("Expedition"))
            {
                if (!await TakePortal())
                    await Coroutines.ReactionWait();
                return true;
            }
            else if (leaderArea.IsLabyrinthArea)
            {
                if (leaderArea.Name == "Aspirants' Plaza")
                {
                    await PartyHelper.FastGotoPartyZone(leader.PlayerEntry.Name);
                    return true;
                }

                if (World.CurrentArea.Name == "Aspirants' Plaza")
                {
                    var trans = LokiPoe.ObjectManager.GetObjectByType<AreaTransition>();
                    if (trans == null)
                    {
                        var loc = ExilePather.FastWalkablePositionFor(new Vector2i(363, 423));
                        if (loc != Vector2i.Zero)
                        {
                            Move.Towards(loc, "Bronze Plaque");
                            return true;
                        }
                        else
                        {
                            GlobalLog.Warn($"[TravelToPartyZoneTask] Cant find Bronze Plaque location.");
                            return false;
                        }
                    }

                    if (LokiPoe.Me.Position.Distance(trans.Position) > 20)
                    {
                        var loc = ExilePather.FastWalkablePositionFor(trans.Position, 20);
                        Move.Towards(loc, $"{trans.Name}");
                        return true;
                    }

                    await PlayerAction.Interact(trans);
                    return true;
                }
                else if (World.CurrentArea.IsLabyrinthArea)
                {
                    AreaTransition areatransition = null;
                    areatransition = LokiPoe.ObjectManager.GetObjectsByType<AreaTransition>().OrderBy(x => x.Distance).FirstOrDefault(x => ExilePather.PathExistsBetween(LokiPoe.Me.Position, ExilePather.FastWalkablePositionFor(x.Position, 20)));
                    if (areatransition != null)
                    {
                        Log.DebugFormat("[{0}] Found walkable Area Transition [{1}].", Name, areatransition.Name);
                        if (LokiPoe.Me.Position.Distance(areatransition.Position) > 20)
                        {
                            var walkablePosition = ExilePather.FastWalkablePositionFor(areatransition, 20);

                            FollowBot.PhaseRun();

                            Move.Towards(walkablePosition, "moving to area transition");
                            return true;
                        }

                        var trans = await PlayerAction.TakeTransition(areatransition);

                        if (!trans)
                        {
                            Log.DebugFormat("[{0}] Areatransition error.", Name);
                        }

                        FollowBot.Leader = null;
                        return true;
                    }
                }
                GlobalLog.Warn($"[TravelToPartyZoneTask] Cant follow the leader in the Labirynt when the lab is already started.");
                return false;
            }

            if (curZone.IsCombatArea && FollowBotSettings.Instance.PortOutThreshold > 0)
            {
                if (!PortOutStopwatch.IsRunning)
                {
                    GlobalLog.Warn($"[TravelToPartyZoneTask] Party leader is in a diffrerent zone waiting {FollowBotSettings.Instance.PortOutThreshold} seconds to see if it come back.");
                    PortOutStopwatch.Restart();
                    await Coroutines.LatencyWait();
                    return true;
                }
                if (PortOutStopwatch.IsRunning && PortOutStopwatch.ElapsedMilliseconds >= (FollowBotSettings.Instance.PortOutThreshold * 1000))
                {
                    PortOutStopwatch.Reset();
                    GlobalLog.Warn($"[TravelToPartyZoneTask] {FollowBotSettings.Instance.PortOutThreshold} seconds elapsed and Party leader is in still a diffrerent zone porting!.");
                    await PartyHelper.FastGotoPartyZone(leadername);
                    return true;
                }

                await Coroutines.LatencyWait();
                return true;
            }
            else
            {
                await PartyHelper.FastGotoPartyZone(leadername);
                await Coroutines.LatencyWait();
            }
            await Coroutines.LatencyWait();
            return true;
        }

        private async Task<bool> TakePortal()
        {
            var portal = LokiPoe.ObjectManager.GetObjectsByType<Portal>().FirstOrDefault(x => x.IsTargetable);
            if (portal != null)
            {
                if (portal.Position.Distance(LokiPoe.Me.Position) > 18)
                    await Move.AtOnce(portal.Position, "Move to portal");
                if (await Coroutines.InteractWith<Portal>(portal))
                    return true;
                else
                    return false;
            }
            else
            {
                Log.DebugFormat("[{0}] Failed to find portals.", Name);
                return false;
            }
        }

        public async Task<LogicResult> Logic(Logic logic)
        {
            return LogicResult.Unprovided;
        }

        public MessageResult Message(Message message)
        {
            if (message.Id == Events.Messages.AreaChanged)
            {
                _zoneCheckRetry = 0;
                PortOutStopwatch.Reset();
                return MessageResult.Processed;
            }
            if (message.Id == "Enable")
            {
                _enabled = true;
                return MessageResult.Processed;
            }
            if (message.Id == "Disable")
            {
                _enabled = false;
                return MessageResult.Processed;
            }
            return MessageResult.Unprocessed;
        }
    }
}