using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Game;
using Brushes = System.Drawing.Brushes;

namespace PatherExplorer
{
    public class RenderWalkPosition : RenderGroup
    {
        private PatherExplorerData _curData;
        private Vector2i _currentWalkPosition = Vector2i.Zero;

        public RenderWalkPosition(System.Windows.Controls.Image viewport) : base(viewport)
        {
            
        }
        private void CreateVisual()
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

                    graphics.FillEllipse(Brushes.Green, _curData.NextWalkPosition.X - 1f, _curData.NextWalkPosition.Y - 1f, 3f, 3f);
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
                if (_curData != null && _curData.NextWalkPosition != _currentWalkPosition)
                {
                    CreateVisual();
                    _currentWalkPosition = _curData.NextWalkPosition;
                }
            }
                
        }

        
    }
}
