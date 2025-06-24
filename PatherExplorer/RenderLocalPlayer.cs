using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Game;
using Brushes = System.Drawing.Brushes;
using Pen = System.Drawing.Pen;


namespace PatherExplorer
{
    public class RenderLocalPlayer : RenderGroup
	{
		private TextBlock _playerLocationText;

		private PatherExplorerData _curData;
		private Vector2i _previousPos = Vector2i.Zero;

		private static readonly Pen PlayerPen = new Pen(Brushes.Red, 5);

		public RenderLocalPlayer(System.Windows.Controls.Image viewport, TextBlock playerLocationText) : base(viewport)
		{
            _playerLocationText = playerLocationText;
        }

        #region Overrides of RenderGroup
		private void CreateVisual()
        {
            _previousPos = _curData.MyPos;

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
					graphics.DrawEllipse(PlayerPen, _curData.MyPos.X, _curData.MyPos.Y, 1, 1);
                }
				//bitmap.MakeTransparent(Color.Black);
				BitmapImage bitmapimage = new BitmapImage();
				using (MemoryStream memory = new MemoryStream())
				{
					bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
					memory.Position = 0;

					bitmapimage.BeginInit();
					bitmapimage.StreamSource = memory;
					bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
					bitmapimage.EndInit();
                    bitmapimage.Freeze();
				}

                LokiPoe.BeginDispatchIfNecessary(View.Dispatcher,
					() => View.Source = bitmapimage);
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
                if (_previousPos != _curData.MyPos)
                {
                    var mpos = _curData.MyPos;

                    _playerLocationText.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        _playerLocationText.Text = "Player Location: " + mpos.X + ", " + mpos.Y;
                    }));
					CreateVisual();
                }
            }
		}

		#endregion
	}
}
