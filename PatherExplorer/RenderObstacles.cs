using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Game;

namespace PatherExplorer
{
    public class RenderObstacles : RenderGroup
    {
        private PatherExplorerData _curData;
        private bool _shouldUpdate;
        private static readonly Pen LinePen = new Pen(Color.Red, 1);
        public RenderObstacles(System.Windows.Controls.Image viewport) : base(viewport)
        {
            ExilePather.OnObstacleUpdate += PolyPathfinder_OnObstacleUpdate;
        }

        private void PolyPathfinder_OnObstacleUpdate()
        {
            _shouldUpdate = true;
        }

        private void CreateVisual(Dictionary<Vector2i, float> obstacles)
        {
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
                    foreach (var obstacle in obstacles)
                    {
                        var delta = obstacle.Value / 2f;
                        graphics.FillEllipse(Brushes.Gray, obstacle.Key.X - delta, obstacle.Key.Y - delta, obstacle.Value, obstacle.Value);
                        graphics.DrawLine(LinePen,
                            obstacle.Key.X - delta,
                            obstacle.Key.Y - delta,
                            obstacle.Key.X + delta,
                            obstacle.Key.Y + delta);
                        graphics.DrawLine(LinePen,
                            obstacle.Key.X - delta,
                            obstacle.Key.Y + delta,
                            obstacle.Key.X + delta,
                            obstacle.Key.Y - delta);
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

        public override void Render(PatherExplorerData data)
        {
            var polyPathfinder = ExilePather.PolyPathfinder;
            if (polyPathfinder == null)
                return;
            _curData = data;
            if (polyPathfinder.AreaGenerated && polyPathfinder.GeneratedAreaHash == _curData.Seed)
            {
                if (_shouldUpdate)
                {
                    CreateVisual(polyPathfinder.Obstacles);
                    _shouldUpdate = false;
                }
            }
                
        }
    }

    
}
