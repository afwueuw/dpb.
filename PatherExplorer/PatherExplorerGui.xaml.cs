using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot;
using log4net;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Game;
using Matrix = System.Windows.Media.Matrix;

namespace PatherExplorer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PatherExplorerGui : UserControl
    {
        public enum GraphicSettings
        {
            Speed,
            Balanced,
            Quality
        }
        public enum MapLayer
        {
            Walk,
            Fly
        }
        private static InterpolationMode _interpolationMode = InterpolationMode.Default;
        public static InterpolationMode InterpolationMode
        {
            get { return _interpolationMode; }
            set => _interpolationMode = value == InterpolationMode.Invalid ? InterpolationMode.Default : value;
        }
        private static SmoothingMode _smoothingMode = SmoothingMode.Default;
        public static SmoothingMode SmoothingMode
        {
            get { return _smoothingMode; }
            set { _smoothingMode = value == SmoothingMode.Invalid ? SmoothingMode.Default : value; }
        }
        private static PixelOffsetMode _pixelOffsetMode = PixelOffsetMode.Default;
        public static PixelOffsetMode PixelOffsetMode
        {
            get { return _pixelOffsetMode; }
            set { _pixelOffsetMode = value == PixelOffsetMode.Invalid ? PixelOffsetMode.Default : value; }
        }
        private static CompositingQuality _compositingQuality = CompositingQuality.Default;
        public static CompositingQuality CompositingQuality
        {
            get { return _compositingQuality; }
            set { _compositingQuality = value == CompositingQuality.Invalid ? CompositingQuality.Default : value; }
        }
        private static ImageFormat _imageFormat = ImageFormat.Png;
        public static ImageFormat ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value ; }
        }
        private RenderNavGrid _renderNavGrid;
        private RenderLocalPlayer _renderLocalPlayer;
        private RenderPathFinding _renderPathFinding;
        private RenderWalkPosition _renderWalkPosition;
        private RenderObstacles _renderObstacles;
        private RenderRaycast _renderRaycast;
        private RenderTgts _renderTgts;

        private readonly List<RenderGroup> _renderGroups = new List<RenderGroup>();

        private readonly DispatcherTimer _tickTimer;

        private readonly Func<bool> IsEnabledFunc;
        private readonly Func<PatherExplorerData> GetDataFunc;

        private Point origin;  // Original Offset of image
        private Point start;   // Original Position of the mouse

        private Point mouseBackupPosition = new Point(0,0);
        public void OnDeinitialize()
        {
            _tickTimer?.Stop();
        }
        public PatherExplorerGui(Func<bool> isEnabledFunc, Func<PatherExplorerData> getDataFunc)
        {
            InitializeComponent();

            IsEnabledFunc = isEnabledFunc;
            GetDataFunc = getDataFunc;

            _renderLocalPlayer = new RenderLocalPlayer(My_Pos_Picture, PlayerLocationText);
            _renderLocalPlayer.Enabled = true;
            _renderGroups.Add(_renderLocalPlayer);

            _renderTgts = new RenderTgts(Tgts_Picture, Track_Tgts_under_Player_CheckBox, Tgts_TextBlock, Tdts_TextBlock, Tgts_Range_Slider);
            _renderTgts.Enabled = true;
            _renderGroups.Add(_renderTgts);

            _renderObstacles = new RenderObstacles(Obstacles_Picture);
            _renderObstacles.Enabled = true;
            _renderGroups.Add(_renderObstacles);

            _renderRaycast = new RenderRaycast(Raycast_Picture);
            _renderRaycast.Enabled = true;
            _renderGroups.Add(_renderRaycast);

            _renderNavGrid = new RenderNavGrid(Map_Picture);
            _renderNavGrid.Enabled = true;
            _renderGroups.Add(_renderNavGrid);

            _renderPathFinding = new RenderPathFinding(FindPath_Result_Picture);
            _renderPathFinding.Enabled = true;
            _renderGroups.Add(_renderPathFinding);

            _renderWalkPosition = new RenderWalkPosition(Next_Selected_Walk_Position_Picture);
            _renderWalkPosition.Enabled = true;
            _renderGroups.Add(_renderWalkPosition);

            Graphic_Quality_ComboBox.ItemsSource = Enum.GetValues(typeof(GraphicSettings));
            Graphic_Quality_ComboBox.SelectedIndex = 0;
            BlockLockedDoors_ComboBox.ItemsSource = Enum.GetValues(typeof(FeatureEnum)); //typeof(FeatureEnum).GetProperties();
            BlockTrialOfAscendancy_ComboBox.ItemsSource = Enum.GetValues(typeof(FeatureEnum)); //typeof(FeatureEnum).GetProperties();
            BlockLockedTempleDoors_ComboBox.ItemsSource = Enum.GetValues(typeof(FeatureEnum)); //typeof(FeatureEnum).GetProperties();
            // This ctor is called from the main gui thread when the control is being added to the Plugins tab.
            // Because of this, we have to track enable/disable state beforehand since they will get invoked before this ctor is called.
            _tickTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(300), DispatcherPriority.Normal, TimerCallback, Dispatcher);
        }

        private void TimerCallback(object sender, EventArgs e)
        {
            // Make sure everything is loaded first.
            if (!LokiPoe.IsBotFullyLoaded)
                return;
            if (MainGrid.IsVisible)
            {
                var test = 1;
            }

            if (!IsEnabledFunc())
            {
                return;
            }

            LokiPoe.BeginDispatchIfNecessary(Dispatcher, () =>
            {
                var data = GetDataFunc();
                if (data.IsValid)
                {
                    foreach (var renderGroup in _renderGroups)
                    {
                        renderGroup.Render(data);
                    }

                    data.IsValid = false;
                }

                PatherExplorerData.LockCamera = LockCameraToPlayer_cb.IsChecked == true;
                if (PatherExplorerData.LockCamera)
                    UpdateTransforms();

                BlockLockedDoors_ComboBox.SelectedItem = ExilePather.BlockLockedDoors;
                BlockTrialOfAscendancy_ComboBox.SelectedItem = ExilePather.BlockTrialOfAscendancy;
                BlockLockedTempleDoors_ComboBox.SelectedItem = ExilePather.BlockLockedTempleDoors;
            });
        }
        

        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private void ReloadExilePatherButton_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ExilePather.Reload(true);

                GetDataFunc().ForceReload = true;
                Log.InfoFormat("[ReloadExilePatherButton_OnClick] Done!");
            }));
        }

        private void MapPicture_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                GetDataFunc().Zoom *= 1.1f;
            }
            else
            {
                GetDataFunc().Zoom *= 1 / 1.1f;
            }
            UpdateTransforms();
        }

        private void UpdateTransforms()
        {
            // Credit go to keks for this contribution on locking the camera on player.
            var data = GetDataFunc();

            Matrix m = Matrix.Identity;

            if (PatherExplorerData.LockCamera)
            {
                // construct our transform based on the player position
                var mapW = data.CachedTerrainData.Cols * 23;
                var mapH = data.CachedTerrainData.Rows * 23;
                // the bitmap is scaled by the current minimum window size to fit in - we have to compensate
                var displayScale = My_Pos_Border.ActualHeight < My_Pos_Border.ActualWidth
                    ? My_Pos_Border.ActualHeight / mapH
                    : My_Pos_Border.ActualWidth / mapW;

                // how far are we away from the center of the map?
                m.Translate((mapW / 2 - data.MyPos.X) * displayScale, (mapH / 2 - data.MyPos.Y) * displayScale);

                // our own transform has no rotation yet - contrary to the re-used
                m.Rotate(225);
            }
            else
            {
                m = Map_Picture.RenderTransform.Value;
                // get the scaling factor
                var om = Map_Picture.RenderTransform.Clone().Value;
                om.OffsetX = 0;
                om.OffsetY = 0;
                var one = om.Transform(new Vector(1, 0));
                // make it unit scale again
                m.Scale(-1 / one.Length, 1 / one.Length);
            }

            //data.Zoom = 1;

            // in any case we want the zoom feature
            m.Scale(-data.Zoom, data.Zoom);

            var rt = Map_Picture.RenderTransform.CloneCurrentValue();//.RenderTransform.Clone();
            var renderTransform = new MatrixTransform(m);

            // Map
            Map_Picture.RenderTransform = renderTransform;

            // Tgts
            Tgts_Picture.RenderTransform = renderTransform;

            // Obstacles_Position
            Obstacles_Picture.RenderTransform = renderTransform;

            // Raycast_Position
            Raycast_Picture.RenderTransform = renderTransform;

            // FindPath_Result
            FindPath_Result_Picture.RenderTransform = renderTransform;

            // Next_Selected_Walk_Position
            Next_Selected_Walk_Position_Picture.RenderTransform = renderTransform;

            // MyPos
            My_Pos_Picture.RenderTransform = renderTransform;
        }

        private void MapPicture_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (My_Pos_Picture.IsMouseCaptured) return;
            My_Pos_Picture.CaptureMouse();

            start = e.GetPosition(My_Pos_Border);
            origin.X = My_Pos_Picture.RenderTransform.Value.OffsetX;
            origin.Y = My_Pos_Picture.RenderTransform.Value.OffsetY;
        }

        private void MapPicture_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!My_Pos_Picture.IsMouseCaptured) return;
            My_Pos_Picture.ReleaseMouseCapture();
        }

        private void MapPicture_OnMouseMove(object sender, MouseEventArgs e)
        {
            mouseBackupPosition = e.MouseDevice.GetPosition(Map_Picture);

            if (My_Pos_Picture.IsMouseCaptured)
            {
                PopupWindow.IsOpen = false;

                Point p = e.MouseDevice.GetPosition(My_Pos_Border);

                Matrix m = Map_Picture.RenderTransform.Value;
                m.OffsetX = origin.X + (p.X - start.X);
                m.OffsetY = origin.Y + (p.Y - start.Y);

                Map_Picture.RenderTransform = new MatrixTransform(m);

                UpdateTransforms();
            }
            else
            {
                var data = GetDataFunc();
                //Popup Window
                string popupContent = "";
                if (mouseBackupPosition != new Point(0, 0))
                {
                    Point p = mouseBackupPosition;
                    var px = p.X * Map_Picture.Source.Width / Map_Picture.ActualWidth;
                    var py = p.Y * Map_Picture.Source.Height / Map_Picture.ActualHeight;

                    Rect rect = new Rect(new Point(px - 2, py - 2), new Size(5, 5));

                    if (rect.Contains(new Point(data.MyPos.X, data.MyPos.Y)))
                    {
                        popupContent += ($"My Position: {data.MyPos.X}, {data.MyPos.Y}{System.Environment.NewLine}");
                    }

                    if (data.PathfindingCommand != null)
                    {
                        var path = new List<Vector2i>(data.PathfindingCommand.Path);

                        var found = path.FirstOrDefault(x => rect.Contains(new Point(x.X, x.Y)));
                        if (found != null && found != Vector2i.Zero)
                        {
                            popupContent += $"Selected Path Node: {found.X}, {found.Y}{System.Environment.NewLine}";
                        }
                    }

                    if (data.NextWalkPosition != Vector2i.Zero)
                    {
                        Point pos = new Point(data.NextWalkPosition.X, data.NextWalkPosition.Y);

                        if (rect.Contains(pos))
                        {
                            popupContent += $"Next Walk Position: {pos.X}, {pos.Y}{System.Environment.NewLine}";
                        }
                    }

                    if (ExilePather.PolyPathfinder != null)
                    {
                        var obstacles = ExilePather.PolyPathfinder.Obstacles;
                        var found = obstacles.FirstOrDefault(x => new Rect(new Point(x.Key.X, x.Key.Y), new Size(x.Value, x.Value)).Contains(px, py));
                        if (found.Key != Vector2i.Zero && found.Value > 0)
                        {
                            popupContent += $"Obstacle Location: {found.Key.X}, {found.Key.Y} Size: {found.Value}{System.Environment.NewLine}";
                        }
                    }

                    var rays = data.GetRaycastData();
                    if (rays.Count > 0)
                    {
                        var found = rays.FirstOrDefault(x => (rect.Contains(x.Origin.X, x.Origin.Y) || rect.Contains(x.Destination.X, x.Destination.Y) || rect.Contains(x.Hitpoint.X, x.Hitpoint.Y)));
                        if (found != null)
                        {
                            popupContent += $"[Raycast] Origin: {found.Origin.X}, {found.Origin.Y} Destination: {found.Destination.X}, {found.Destination.Y}, Hitpoint: {found.Hitpoint.X}, {found.Hitpoint.Y}{System.Environment.NewLine}";
                        }
                    }
                }

                if (!string.IsNullOrEmpty(popupContent))
                {
                    PopupWindow_txt.Text = popupContent.TrimEnd(Environment.NewLine.ToCharArray());
                    PopupWindow.IsOpen = true;
                }
                else
                {
                    PopupWindow_txt.Text = "";
                    PopupWindow.IsOpen = false;
                }

            }
        }

        private void Pathfind_Origin_btn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!LokiPoe.IsInGame) return;
            var pos = LokiPoe.Me.Position;
            Pathfind_Origin_X_txt.Text = pos.X.ToString();
            Pathfind_Origin_Y_txt.Text = pos.Y.ToString();
        }

        private void Pathfind_Destinatio_btn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!LokiPoe.IsInGame) return;
            var pos = LokiPoe.Me.Position;
            Pathfind_Destinatio_X_txt.Text = pos.X.ToString();
            Pathfind_Destinatio_Y_txt.Text = pos.Y.ToString();
        }

        private void CreatePath_btn_OnClick(object sender, RoutedEventArgs e)
        {
            ExecutePathFinding();
        }

        private void ExecutePathFinding()
        {
            if (!LokiPoe.IsInGame)
            {
                Log.ErrorFormat($"Can't create a path while not in game.");
                return;
            }
            if (!ExilePather.IsReady)
            {
                Log.ErrorFormat($"ExilePater is not ready, Reload the ExilePather.");
                return;
            }

            if (string.IsNullOrEmpty(Pathfind_Origin_X_txt.Text) ||
                !Int32.TryParse(Pathfind_Origin_X_txt.Text, out int originX))
            {
                Log.ErrorFormat($"OriginX textbox is empty or in incorrect format, make sure its a valid integer.");
                return;
            }
            if (string.IsNullOrEmpty(Pathfind_Origin_Y_txt.Text) ||
                !Int32.TryParse(Pathfind_Origin_Y_txt.Text, out int originY))
            {
                Log.ErrorFormat($"OriginY textbox is empty or in incorrect format, make sure its a valid integer.");
                return;
            }
            var originPos = new Vector2i(originX, originY);

            if (string.IsNullOrEmpty(Pathfind_Destinatio_X_txt.Text) ||
                !Int32.TryParse(Pathfind_Destinatio_X_txt.Text, out int destinationX))
            {
                Log.ErrorFormat($"DestinatioX textbox is empty or in incorrect format, make sure its a valid integer.");
                return;
            }
            if (string.IsNullOrEmpty(Pathfind_Destinatio_Y_txt.Text) ||
                !Int32.TryParse(Pathfind_Destinatio_Y_txt.Text, out int destinationY))
            {
                Log.ErrorFormat($"DestinatioY textbox is empty or in incorrect format, make sure its a valid integer.");
                return;
            }
            var destinationPos = new Vector2i(destinationX, destinationY);

            //if (string.IsNullOrEmpty(Nodes_Dist_txt.Text) ||
            //    !Int32.TryParse(Nodes_Dist_txt.Text, out int nodesDist))
            //{
            //    Log.ErrorFormat($"Nodes Dist textbox is empty or in incorrect format, make sure its a valid integer.");
            //    return;
            //}
            PathfindingCommand _cmd = new PathfindingCommand(originPos, destinationPos, (int)Nodes_Dist_Slider.Value, true);
            if (!ExilePather.FindPath(ref _cmd))
            {
                Log.ErrorFormat($"Failed to create a Path from {originX},{originY} to {destinationX},{destinationY}.");
                return;
            }

            Utility.BroadcastMessage(null, "FindPath_Result", _cmd);
        }

        private void My_Pos_Picture_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.W))
            {
                Point p = e.MouseDevice.GetPosition(Map_Picture);
                var px = p.X * Map_Picture.Source.Width / Map_Picture.ActualWidth;
                var py = p.Y * Map_Picture.Source.Height / Map_Picture.ActualHeight;
                Pathfind_Origin_X_txt.Text = ((int)px).ToString();
                Pathfind_Origin_Y_txt.Text = ((int)py).ToString();
            }
            else if (Keyboard.IsKeyDown(Key.D))
            {
                Point p = e.MouseDevice.GetPosition(Map_Picture);
                var px = p.X * Map_Picture.Source.Width / Map_Picture.ActualWidth;
                var py = p.Y * Map_Picture.Source.Height / Map_Picture.ActualHeight;
                Pathfind_Destinatio_X_txt.Text = ((int)px).ToString();
                Pathfind_Destinatio_Y_txt.Text = ((int)py).ToString();
            }

            else if (Keyboard.IsKeyDown(Key.Q))
            {
                ExecutePathFinding();
            }

            else if (Keyboard.IsKeyDown(Key.T))
            {
                Point p = e.MouseDevice.GetPosition(Map_Picture);
                var px = p.X * Map_Picture.Source.Width / Map_Picture.ActualWidth;
                var py = p.Y * Map_Picture.Source.Height / Map_Picture.ActualHeight;
                PatherExplorerData._previousSelectedTgtsPos = new Vector2i((int)px, (int)py);
            }
        }

        private void BlockLockedDoors_ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BlockLockedDoors_ComboBox.SelectedItem == null) return;
            var ret = Enum.TryParse(BlockLockedDoors_ComboBox.SelectedItem.ToString(), out FeatureEnum fenum);
            if (ret)
            {
                ExilePather.BlockLockedDoors = fenum;
            }
        }
        private void BlockTrialOfAscendancy_ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BlockTrialOfAscendancy_ComboBox.SelectedItem == null) return;
            var ret = Enum.TryParse(BlockTrialOfAscendancy_ComboBox.SelectedItem.ToString(), out FeatureEnum fenum);
            if (ret)
            {
                ExilePather.BlockTrialOfAscendancy = fenum;
            }
        }
        private void BlockLockedTempleDoors_ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BlockLockedTempleDoors_ComboBox.SelectedItem == null) return;
            var ret = Enum.TryParse(BlockLockedTempleDoors_ComboBox.SelectedItem.ToString(), out FeatureEnum fenum);
            if (ret)
            {
                ExilePather.BlockLockedTempleDoors = fenum;
            }
        }

        private void Graphic_Quality_ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Graphic_Quality_ComboBox.SelectedItem == null) return;
            var ret = Enum.TryParse(Graphic_Quality_ComboBox.SelectedItem.ToString(), out GraphicSettings fenum);
            if (ret)
            {
                if (fenum == GraphicSettings.Speed)
                {
                    InterpolationMode = InterpolationMode.NearestNeighbor;
                    SmoothingMode = SmoothingMode.HighSpeed;
                    PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    CompositingQuality = CompositingQuality.HighSpeed;
                }
                else if (fenum == GraphicSettings.Balanced)
                {
                    InterpolationMode = InterpolationMode.NearestNeighbor;
                    SmoothingMode = SmoothingMode.None;
                    PixelOffsetMode = PixelOffsetMode.Default;
                    CompositingQuality = CompositingQuality.Default;
                }
                else if (fenum == GraphicSettings.Quality)
                {
                    InterpolationMode = InterpolationMode.NearestNeighbor;
                    SmoothingMode = SmoothingMode.HighQuality;
                    PixelOffsetMode = PixelOffsetMode.HighQuality;
                    CompositingQuality = CompositingQuality.HighQuality;
                }
                _renderNavGrid.ForceRedraw = true;
            }
        }

        private void Fps_Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_tickTimer == null) return;
            _tickTimer.Interval = TimeSpan.FromMilliseconds( (int)((double)1000 / e.NewValue));
        }

        private void ShowFlyMap_cb_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            if (cb == null) return;
            PatherExplorerData.ShowFlyMap = cb.IsChecked == true;
            if (_renderNavGrid == null) return;
            _renderNavGrid.ForceRedraw = true;
        }

    }
}
