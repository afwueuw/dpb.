using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Game;
using Brushes = System.Drawing.Brushes;

namespace PatherExplorer
{
	public class RenderNavGrid : RenderGroup
    {
        public bool ForceRedraw = false;
        private uint _initialSeed;
        private PatherExplorerData _curData;

        public RenderNavGrid(System.Windows.Controls.Image viewport) : base(viewport)
        {
            ExilePather.OnExilePatherReload += ExilePather_OnExilePatherReload;
        }

        private void ExilePather_OnExilePatherReload()
        {
            _initialSeed = 0;
        }

        private void CreateVisual()
        {
            _initialSeed = _curData.Seed;

            if (_curData.IsInGame)
            {
                int cols = _curData.CachedTerrainData.Cols * 23;
                int rows = _curData.CachedTerrainData.Rows * 23;
                Bitmap bitmap = new Bitmap(cols, rows);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = PatherExplorerGui.InterpolationMode;
                    graphics.SmoothingMode = PatherExplorerGui.SmoothingMode;
                    graphics.PixelOffsetMode = PatherExplorerGui.PixelOffsetMode;
                    graphics.CompositingQuality = PatherExplorerGui.CompositingQuality;
                    //graphics.FillRectangle(Brushes.Black, 0, 0, cols, rows);
                    for (int c = 0; c < cols; c++)
                    {
                        for (int r = 0; r < rows; r++)
                        {
                            if (WalkabilityGrid.IsWalkable(_curData.CachedTerrainData.Data, _curData.CachedTerrainData.BPR, c, r, 2))
                            {
                                graphics.FillRectangle(Brushes.White, c, r, 1, 1);
                            }
                            else 
                            {
                                if (PatherExplorerData.ShowFlyMap && WalkabilityGrid.IsWalkable(_curData.CachedTerrainData.FlyData, _curData.CachedTerrainData.BPR, c, r, 2))
                                    graphics.FillRectangle(Brushes.Turquoise, c, r, 1, 1);
                            }

                        }
                    }
                }
                //bitmap.MakeTransparent(Color.Black);
                BitmapImage bitmapimage = new BitmapImage();
                using (MemoryStream memory = new MemoryStream())
                {

                    bitmap.Save(memory, PatherExplorerGui.ImageFormat);
                    memory.Position = 0;
                    
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                    bitmapimage.Freeze();
                }

                LokiPoe.BeginDispatchIfNecessary(View.Dispatcher, () => View.Source = bitmapimage);
            }
            else
            {
                LokiPoe.BeginDispatchIfNecessary(View.Dispatcher, () => View.Source = null);
            }
        }

        #region Overrides of RenderGroup

        public override void Render(PatherExplorerData data)
        {
            var polyPathfinder = ExilePather.PolyPathfinder;
            if (polyPathfinder == null)
                return;

            _curData = data;

            // Detect mesh updates by checking if the mesh is generated, and the seed it was generated on.
            if (polyPathfinder.AreaGenerated && polyPathfinder.GeneratedAreaHash == _curData.Seed)
            {
                if (_initialSeed != _curData.Seed || ForceRedraw)
                {
                    CreateVisual();
                    _curData.NextWalkPosition = Vector2i.Zero;
                    PatherExplorerData._previousSelectedTgtsPos = Vector2i.Zero;
                    _curData.PathfindingCommand = null;
                    _curData.ClearRaycastData();
                    ForceRedraw = false;
                }
            }
        }

        #endregion
    }
}
