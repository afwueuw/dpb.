using DreamPoeBot.Loki.Bot;

namespace SkillPlayerMover.Helpers
{
    public class RoutineHelpers
    {
        private readonly IRoutine _loadedCr;

        public RoutineHelpers()
        {
            _loadedCr = RoutineManager.Current;
        }
        public int GetMaxMeleeRange()
        {
            var msg = new Message("GetMaxMeleeRange");
            _loadedCr.Message(msg);
            var value = msg.GetOutput<int>();

            return value;
        }
        public int GetMaxRangedRange()
        {
            var msg = new Message("GetMaxRangeRange");
            _loadedCr.Message(msg);
            var value = msg.GetOutput<int>();

            return value;
        }
        public int GetAoeRangedSlot()
        {
            var msg = new Message("GetAoeRangedSlot");
            _loadedCr.Message(msg);
            var value = msg.GetOutput<int>();

            return value;
        }
        public int GetSingleTargetMeleeSlot()
        {
            var msg = new Message("GetSingleTargetMeleeSlot");
            _loadedCr.Message(msg);
            var value = msg.GetOutput<int>();

            return value;
        }
        public int GetSingleTargetRangedSlot()
        {
            var msg = new Message("GetSingleTargetRangedSlot");
            _loadedCr.Message(msg);
            var value = msg.GetOutput<int>();

            return value;
        }

    }
}
