﻿using System.Threading.Tasks;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Game;

namespace FollowBot.SimpleEXtensions.CommonTasks
{
    public class CombatTask : ITask
    {
        private readonly int _leashRange;

        public CombatTask(int leashRange)
        {
            _leashRange = leashRange;
        }

        public async Task<bool> Run()
        {
            // 리더가 없거나, 내가 리더와 같은 존(맵)에 없으면 즉시 공격 중지
            var leader = FollowBot.Leader;
            if (leader == null || !LokiPoe.InGameState.PartyHud.IsInSameZone(leader.Name))
                return false;

            if (!FollowBotSettings.Instance.ShouldKill) return false;
            if (!World.CurrentArea.IsCombatArea) return false;

            var routine = RoutineManager.Current;

            routine.Message(new Message("SetLeash", this, _leashRange));

            var res = await routine.Logic(new Logic("hook_combat", this));
            return res == LogicResult.Provided;
        }

        #region Unused interface methods

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        public async Task<LogicResult> Logic(Logic logic)
        {
            return LogicResult.Unprovided;
        }

        public void Start()
        {
        }

        public void Tick()
        {
        }

        public void Stop()
        {
        }

        public string Name => "CombatTask (Leash " + _leashRange + ")";

        public string Description => "This task executes routine logic for combat.";

        public string Author => "NotYourFriend original from EXVault";

        public string Version => "1.0";

        #endregion
    }
}