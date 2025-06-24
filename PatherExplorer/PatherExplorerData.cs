using System.Collections.Generic;
using System.Linq;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Game;

namespace PatherExplorer
{
    public class PatherExplorerData
    {
        private readonly object _locker = new object();
        public bool IsValid { get; set; }
        public bool IsInGame { get; private set; }
        public uint Seed { get; private set; }
        public Vector2i MyPos { get; private set; }
        public Vector2 MyWorldPos { get; private set; }
        public bool ForceReload { get; set; }
        public float Zoom { get; set; } = 1;
        public static bool LockCamera { get; set; }
        public static bool ShowFlyMap { get; set; }
        public static LokiPoe.TerrainDataEntry[,] TgtsEntry { get; set; }
        public static LokiPoe.TerrainDataEntry[,] TdtsEntry { get; set; }
        public CachedTerrainData CachedTerrainData { get; private set; }

        private PathfindingCommand _pathfindingCommand = null;
        public static Vector2i _previousSelectedTgtsPos { get; set; }
        public PathfindingCommand PathfindingCommand
        {
            get { lock (_locker) { return _pathfindingCommand; } }
            set
            {
                lock (_locker)
                {
                    _pathfindingCommand = value;
                }
                NextWalkPosition = Vector2i.Zero;
            }
        }

        private Vector2i _nextWalkPosition = Vector2i.Zero;

        public Vector2i NextWalkPosition
        {
            get { lock (_locker) { return _nextWalkPosition; } }
            set { lock (_locker) { _nextWalkPosition = value; } }
        }

        public bool RaycastChanged = false;
        private List<RaycastData> _raycastData = new List<RaycastData>();

        public void AddRaycastData(RaycastData raycastData)
        {
            lock (_locker)
            {
                var ret = _raycastData.FirstOrDefault(x => x.Origin == raycastData.Origin && x.Destination == raycastData.Destination);
                if (ret != null)
                    _raycastData.Remove(ret);
                _raycastData.Add(raycastData);
                RaycastChanged = true;
            }
        }
        public void RemoveRaycastData(RaycastData raycastData)
        {
            lock (_locker)
            {
                var ret = _raycastData.FirstOrDefault(x => x.Origin == raycastData.Origin && x.Destination == raycastData.Destination);
                if (ret != null)
                    _raycastData.Remove(ret);
                RaycastChanged = true;
            }
        }
        public List<RaycastData> GetRaycastData()
        {
            List<RaycastData> data = new List<RaycastData>();
            lock (_locker)
            {
                data = new List<RaycastData>(_raycastData);
            }

            return data;
        }

        public void ClearRaycastData()
        {
            lock (_locker)
            {
                _raycastData = new List<RaycastData>();
                RaycastChanged = true;
            }
        }


        public void Update()
        {
            IsInGame = LokiPoe.IsInGame;

            if (IsInGame)
            {
                Seed = LokiPoe.LocalData.AreaHash;

                MyPos = LokiPoe.MyPosition;
                MyWorldPos = LokiPoe.MyWorldPosition;
                CachedTerrainData = LokiPoe.TerrainData.Cache;
                TgtsEntry = (LokiPoe.TerrainDataEntry[,])LokiPoe.TerrainData.TgtEntries.Clone();
                TdtsEntry = (LokiPoe.TerrainDataEntry[,])LokiPoe.TerrainData.TdtEntries.Clone();
            }
            else
            {
                Seed = 0;
                PathfindingCommand = null;
                NextWalkPosition = Vector2i.Zero;
                TgtsEntry = null;
                TdtsEntry = null;
            }

            IsValid = true;

            //ForceReload = false; // If we set this to false, we'll never be able to process it when set
        }
    }
}
