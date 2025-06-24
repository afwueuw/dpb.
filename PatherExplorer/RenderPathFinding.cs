using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Game;
using Brushes = System.Drawing.Brushes;

namespace PatherExplorer
{
    public class RenderPathFinding : RenderGroup
    {
        private readonly object _locker = new object();
        private PatherExplorerData _curData;
        private List<Vector2i> _path;
        private bool _shouldUpdate;
        private static readonly Pen LinePen = new Pen(Brushes.DarkRed, 3);
        public RenderPathFinding(System.Windows.Controls.Image viewport) : base(viewport)
        {
            
        }
        private void CreateVisual()
        {
			if (_curData.IsInGame && _path != null && _path.Count > 1)
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
                    graphics.DrawLines(LinePen, _path.Select(x => new Point(x.X, x.Y)).ToArray());
                    foreach (var vector2I in _path)
                    {
                        graphics.FillEllipse(Brushes.DodgerBlue, vector2I.X - 1f, vector2I.Y - 1f, 3f, 3f);
                    }
                }
                //try
                //{
                //    using (Graphics graphics = Graphics.FromImage(bitmap))
                //    {
                //        graphics.InterpolationMode = PatherExplorerGui.InterpolationMode;
                //        graphics.SmoothingMode = PatherExplorerGui.SmoothingMode;
                //        graphics.PixelOffsetMode = PatherExplorerGui.PixelOffsetMode;
                //        graphics.CompositingQuality = PatherExplorerGui.CompositingQuality;
                //        //graphics.FillRectangle(Brushes.Black, 0, 0, cols, rows);
                //        graphics.DrawLines(_linePen, _path.Select(x => new Point(x.X, x.Y)).ToArray());
                //        foreach (var vector2I in _path)
                //        {
                //            graphics.FillEllipse(Brushes.DodgerBlue, (float)vector2I.X - 1f, (float)vector2I.Y - 1f, 3f, 3f);
                //        }
                //    }
                //}
                //catch (Exception e)
                //{
                //    LokiPoe.BeginDispatchIfNecessary(View.Dispatcher, () => View.Source = null);
                //    return;
                //}
                
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
                if (_curData != null)
                {
                    lock (_locker)
                    {
                        if (_curData.PathfindingCommand == null)
                        {
                            if (_path != null)
                            {
                                _path = null;
                                _shouldUpdate = true;
                            }
                        }
                        else
                        {
                            Vector2i[] p = new Vector2i[_curData.PathfindingCommand.Path.Count];
                            _curData.PathfindingCommand?.Path.CopyTo(p);
                            var tempPath = p.ToList();
                            if (_path == null || _path != tempPath)
                            {
                                _path = tempPath;
                                _shouldUpdate = true;
                            }
                        }
                    }

                    if (_shouldUpdate)
                    {
                        CreateVisual();
                        _shouldUpdate = false;
                    }
                }
            }
                
        }

        public void PathFinding(Vector2i dest)
        {
            var cmd = new PathfindingCommand(_curData.MyPos, dest, 3);
            if (!ExilePather.FindPath(ref cmd))
            {
                return;
            }
            _path = cmd.Path;
        }
    }
}
