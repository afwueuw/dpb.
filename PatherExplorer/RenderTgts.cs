using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Game;
using CheckBox = System.Windows.Controls.CheckBox;
using Matrix = System.Drawing.Drawing2D.Matrix;
using Point = System.Drawing.Point;
using TextBox = System.Windows.Controls.TextBox;

namespace PatherExplorer
{
    public class RenderTgts : RenderGroup
    {
        private CheckBox _TrackPlayerCheckBox;
        private TextBox _tgtsAroundLocationsTextBlock;
        private TextBox _tdtsAroundLocationsTextBlock;
        private Slider _tgtRangeSlider;
        private Vector2i _previousPlayerPos = Vector2i.Zero;
        private Vector2i _previousSelectedTgtsPos = Vector2i.Zero;
        private PatherExplorerData _curData;
        private uint _initialSeed;
        private readonly Pen _pen = new Pen(Brushes.Yellow, 1);
        private readonly Font _font = new Font(FontFamily.GenericMonospace, 9);
        public RenderTgts(System.Windows.Controls.Image viewport, CheckBox track_Tgts_under_Player_Checkbos, TextBox tgtsAroundLocationsTextBlock, TextBox tdtsAroundLocationsTextBlock, Slider rangeSlider) : base(viewport)
        {
            _TrackPlayerCheckBox = track_Tgts_under_Player_Checkbos;
            _tgtsAroundLocationsTextBlock = tgtsAroundLocationsTextBlock;
            _tdtsAroundLocationsTextBlock = tdtsAroundLocationsTextBlock;
            _tgtRangeSlider = rangeSlider;
        }

        private void CreateVisual()
        {
            if (_curData.IsInGame && (_TrackPlayerCheckBox.IsChecked == true || PatherExplorerData._previousSelectedTgtsPos != Vector2i.Zero))
            {
                int tGtX = 0;
                int tGty = 0;
                if (_TrackPlayerCheckBox.IsChecked == true && _previousPlayerPos != _curData.MyPos)
                {
                    _previousSelectedTgtsPos = Vector2i.Zero;
                    PatherExplorerData._previousSelectedTgtsPos = Vector2i.Zero;
                    _previousPlayerPos = _curData.MyPos;
                    tGtX = (int)Math.Truncate(_curData.MyPos.X / 23d);
                    tGty = (int)Math.Truncate(_curData.MyPos.Y / 23d);
                }
                else if (PatherExplorerData._previousSelectedTgtsPos != Vector2i.Zero && _previousSelectedTgtsPos != PatherExplorerData._previousSelectedTgtsPos )
                {
                    _previousSelectedTgtsPos = PatherExplorerData._previousSelectedTgtsPos;
                    _previousPlayerPos = _curData.MyPos;
                    tGtX = (int)Math.Truncate(_previousSelectedTgtsPos.X / 23d);
                    tGty = (int)Math.Truncate(_previousSelectedTgtsPos.Y / 23d);
                }
                else if (_TrackPlayerCheckBox.IsChecked == false && _previousPlayerPos != _curData.MyPos)
                {
                    _previousSelectedTgtsPos = Vector2i.Zero;
                    PatherExplorerData._previousSelectedTgtsPos = Vector2i.Zero;
                    _previousPlayerPos = _curData.MyPos;
                    LokiPoe.BeginDispatchIfNecessary(View.Dispatcher, () => View.Source = null);
                }

                if (tGtX == 0 || tGty == 0) return;
                // Collect Tgts
                
                List<Vector2i> tGtsVectors = new List<Vector2i>();
                string tGtsString = "";
                string tDtsString = "";
                var index = 0;
                var tgtslengtX = PatherExplorerData.TgtsEntry.GetLength(0);
                var tgtslengtY = PatherExplorerData.TgtsEntry.GetLength(1);
                var range = (int)_tgtRangeSlider.Value;
                for (int tx = tGtX - range; tx <= tGtX + range; tx++)
                {
                    for (int ty = tGty - range; ty <= tGty + range; ty++)
                    {
                        index++; 
                        if (tx < 0 || ty < 0) continue;
                        if (tx >= tgtslengtX || ty >= tgtslengtY) continue;
                        var tgtName = PatherExplorerData.TgtsEntry[tx, ty].TgtName;
                        var tdtName = PatherExplorerData.TdtsEntry[tx, ty].TgtName;
                        tGtsVectors.Add(new Vector2i(tx, ty));
                        tGtsString += $"{index.ToString()} - {tgtName}" + Environment.NewLine;
                        tDtsString += $"{index.ToString()} - {tdtName}" + Environment.NewLine;
                    }
                }

                _tgtsAroundLocationsTextBlock.Text = tGtsString;
                _tdtsAroundLocationsTextBlock.Text = tDtsString;

                int cols = _curData.CachedTerrainData.Cols * 23;
                int rows = _curData.CachedTerrainData.Rows * 23;
                Bitmap bitmap = new Bitmap(cols, rows);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    GraphicsContainer graphicsContainer;
                    graphics.InterpolationMode = PatherExplorerGui.InterpolationMode;
                    graphics.SmoothingMode = PatherExplorerGui.SmoothingMode;
                    graphics.PixelOffsetMode = PatherExplorerGui.PixelOffsetMode;
                    graphics.CompositingQuality = PatherExplorerGui.CompositingQuality;
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                    index = 0;
                    foreach (var tGtsVector in tGtsVectors)
                    {
                        index++;
                        Rectangle rect = new Rectangle(new Point(tGtsVector.X * 23, tGtsVector.Y * 23), new Size(23, 23));
                        graphics.DrawRectangle(_pen, rect);
                        graphicsContainer = graphics.BeginContainer();
                        graphics.TranslateTransform(tGtsVector.X * 23f + 11.5f, tGtsVector.Y * 23f + 11.5f);
                        graphics.RotateTransform(-45);
                        graphics.MultiplyTransform(new Matrix(1, 0, 0, -1, 0, 0));
                        var strSize = graphics.MeasureString(index.ToString(), _font);
                        graphics.DrawString(index.ToString(), _font, Brushes.MediumVioletRed, -(strSize.Width / 2), -(strSize.Height / 2));
                        graphics.ResetTransform();
                        graphics.EndContainer(graphicsContainer);
                    }

                }

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

            if (PatherExplorerData.TgtsEntry == null) return;
            if (polyPathfinder.AreaGenerated)
            {
                if (polyPathfinder.GeneratedAreaHash != _initialSeed)
                {
                    _initialSeed = polyPathfinder.GeneratedAreaHash;
                    _previousPlayerPos = Vector2i.Zero;
                    _previousSelectedTgtsPos = Vector2i.Zero;
                    PatherExplorerData._previousSelectedTgtsPos = Vector2i.Zero;
                }
                CreateVisual();
            }

        }
    }
}
