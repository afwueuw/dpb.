using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Game;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace PatherExplorer
{
    internal class RenderRaycast : RenderGroup
    {
        private PatherExplorerData _curData;
        private static readonly Pen _linePen = new Pen(Color.Red, 1);
        private static readonly Pen _arrowPen = new Pen(Color.DarkGoldenrod, 1);

        public RenderRaycast(System.Windows.Controls.Image viewport) : base(viewport)
        {
            
        }
        private void CreateVisual(List<RaycastData> rays)
        {
            if (_curData.IsInGame && rays != null && rays.Any())
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
                    for (int i = rays.Count - 1; i >= 0; i--)
                    {
                        var ray = rays[i];
                        //var raylife = DateTime.Now.Subtract(ray.CreationDateTime).TotalSeconds;
                        //if (raylife > 15)
                        //{
                        //    _curData.RemoveRaycastData(ray);
                        //    continue;
                        //}

                        //var test = (int)(255 - (16 * raylife));
                        //var alpha = test < 0 ? 0 : test > 255 ? 255 : test;
                        var linelength = Math.Sqrt(Math.Pow((ray.Destination.Y - ray.Origin.Y), 2) + Math.Pow((ray.Destination.X - ray.Origin.X), 2));
                        using (GraphicsPath capPath = new GraphicsPath())
                        {
                            // Arrow
                            var arrowLenght = linelength < 120 ? 6 : 10;
                            var arrowSpace = linelength < 120 ? 3 : 6;
                            
                            capPath.AddLine(-arrowSpace, -arrowLenght, 0, 0);
                            capPath.AddLine(0, 0, arrowSpace, -arrowLenght);

                            _arrowPen.CustomEndCap = new CustomLineCap(null, capPath);

                            graphics.DrawLine(_arrowPen, ray.Origin.X, ray.Origin.Y, ray.Destination.X, ray.Destination.Y);
                        }

                        if (!ray.Result)
                        {
                            float delta = 2.5f;
                            graphics.FillEllipse(Brushes.Violet, (float)ray.Hitpoint.X - delta, (float)ray.Hitpoint.Y - delta, 5f, 5f);
                            graphics.DrawLine(_linePen,
                                (float)ray.Hitpoint.X - delta,
                                (float)ray.Hitpoint.Y - delta,
                                (float)ray.Hitpoint.X + delta,
                                (float)ray.Hitpoint.Y + delta);
                            graphics.DrawLine(_linePen,
                                (float)ray.Hitpoint.X - delta,
                                (float)ray.Hitpoint.Y + delta,
                                (float)ray.Hitpoint.X + delta,
                                (float)ray.Hitpoint.Y - delta);
                        }
                            //graphics.FillEllipse(Brushes.Violet, ray.Hitpoint.X - 2, ray.Hitpoint.Y - 2, 5, 5);
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
                if (_curData.RaycastChanged)
                {
                    var rays = _curData.GetRaycastData();
                    CreateVisual(rays);
                    _curData.RaycastChanged = false;
                }
            }
                
            
        }
    }

    public class RaycastData
    {
        public Vector2i Origin { get; set; }
        public Vector2i Destination { get; set; }
        public bool Result { get; set; }
        public Vector2i Hitpoint { get; set; }
        public DateTime CreationDateTime { get; set; }

        public RaycastData(Vector2i origin, Vector2i destination, bool result, Vector2i hitpoint)
        {
            Origin = origin;
            Destination = destination;
            Result = result;
            Hitpoint = hitpoint;
            CreationDateTime = DateTime.Now;
        }
    }
}
