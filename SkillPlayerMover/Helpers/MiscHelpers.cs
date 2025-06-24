using System.Linq;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Game;
using DreamPoeBot.Loki.Game.GameData;
using DreamPoeBot.Loki.Game.Objects;

namespace SkillPlayerMover.Helpers
{
    public static class MiscHelpers
    {
        /// <summary>
        /// Curtesy of pushedx.
        /// Checks for a specific closed door between start and end.
        /// </summary>
        /// <param name="door"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="distanceFromPoint">How far to check around each point for a door object.</param>
        /// <param name="stride">The distance between points to check in the path.</param>
        /// <param name="dontLeaveFrame">Should the current frame not be left?</param>
        /// <returns>true if there's a closed door and false otherwise.</returns>
        public static bool ClosedDoorBetween(TriggerableBlockage door, Vector2i start, Vector2i end, int distanceFromPoint = 10, int stride = 10,
            bool dontLeaveFrame = false)
        {

            var path = ExilePather.GetPointsOnSegment(start, end, dontLeaveFrame);

            for (var i = 0; i < path.Count; i += stride)
            {

                    if (door.Position.Distance(path[i]) <= distanceFromPoint)
                    {
                        return true;
                    }
                
            }

            return false;
        }
        /// <summary>
        /// Checks for a closed door between start and end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="distanceFromPoint">How far to check around each point for a door object.</param>
        /// <param name="stride">The distance between points to check in the path.</param>
        /// <param name="dontLeaveFrame">Should the current frame not be left?</param>
        /// <returns>true if there's a closed door and false otherwise.</returns>
        public static bool ClosedDoorBetween(
            NetworkObject start,
            NetworkObject end,
            int distanceFromPoint = 10,
            int stride = 10,
            bool dontLeaveFrame = false)
        {
            return ClosedDoorBetween(start.Position, end.Position, distanceFromPoint, stride, dontLeaveFrame);
        }

        /// <summary>
        /// Checks for a closed door between start and end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="distanceFromPoint">How far to check around each point for a door object.</param>
        /// <param name="stride">The distance between points to check in the path.</param>
        /// <param name="dontLeaveFrame">Should the current frame not be left?</param>
        /// <returns>true if there's a closed door and false otherwise.</returns>
        public static bool ClosedDoorBetween(
            NetworkObject start,
            Vector2i end,
            int distanceFromPoint = 10,
            int stride = 10,
            bool dontLeaveFrame = false)
        {
            return ClosedDoorBetween(start.Position, end, distanceFromPoint, stride, dontLeaveFrame);
        }

        /// <summary>
        /// Checks for a closed door between start and end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="distanceFromPoint">How far to check around each point for a door object.</param>
        /// <param name="stride">The distance between points to check in the path.</param>
        /// <param name="dontLeaveFrame">Should the current frame not be left?</param>
        /// <returns>true if there's a closed door and false otherwise.</returns>
        public static bool ClosedDoorBetween(
            Vector2i start,
            NetworkObject end,
            int distanceFromPoint = 10,
            int stride = 10,
            bool dontLeaveFrame = false)
        {
            return ClosedDoorBetween(start, end.Position, distanceFromPoint, stride, dontLeaveFrame);
        }

        /// <summary>
        /// Checks for a closed door between start and end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="distanceFromPoint">How far to check around each point for a door object.</param>
        /// <param name="stride">The distance between points to check in the path.</param>
        /// <param name="dontLeaveFrame">Should the current frame not be left?</param>
        /// <returns>true if there's a closed door and false otherwise.</returns>
        public static bool ClosedDoorBetween(
            Vector2i start,
            Vector2i end,
            int distanceFromPoint = 10,
            int stride = 10,
            bool dontLeaveFrame = false)
        {
            // We need to store positions and not objects to avoid frame leaving issues.
            var doorPositions = LokiPoe.ObjectManager.AnyDoors.Where(d => !d.IsOpened).Select(d => d.Position).ToList();
            if (!doorPositions.Any())
                return false;

            var path = ExilePather.GetPointsOnSegment(start, end, dontLeaveFrame);

            for (var i = 0; i < path.Count; i += stride)
            {
                foreach (var doorPosition in doorPositions)
                {
                    if (doorPosition.Distance(path[i]) <= distanceFromPoint)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static int NumberOfMobsNearPosition(Vector2i position
            , float distance, bool dead = false)
        {
            var mpos = position;

            var mobPositions =
                LokiPoe.ObjectManager.GetObjectsByType<Monster>().Where(d => d.IsAliveHostile).Select(m => m.Position).ToList();
            if (!mobPositions.Any())
                return 0;

            var curCount = 0;

            foreach (var mobPosition in mobPositions)
            {
                if (mobPosition.Distance(mpos) < distance)
                {
                    curCount++;
                }
            }

            return curCount;
        }
    }
}
