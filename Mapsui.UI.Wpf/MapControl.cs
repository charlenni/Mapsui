using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Rendering.Xaml;
using Mapsui.Utilities;
using Mapsui.Widgets;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Point = System.Windows.Point;
using VerticalAlignment = System.Windows.VerticalAlignment;
using XamlVector = System.Windows.Vector;

namespace Mapsui.UI.Wpf
{
    public enum RenderMode
    {
        Skia,
        Wpf
    }

    public partial class MapControl : Grid, IMapControl
    {
        // ReSharper disable once UnusedMember.Local // This registration triggers the call to OnResolutionChanged
        private static readonly DependencyProperty ResolutionProperty =
            DependencyProperty.Register(
                "Resolution", typeof(double), typeof(MapControl),
                new PropertyMetadata(OnResolutionChanged));

        private readonly Rectangle _selectRectangle = CreateSelectRectangle();
        private readonly DoubleAnimation _zoomAnimation = new DoubleAnimation();
        private readonly Storyboard _zoomStoryBoard = new Storyboard();
        private Geometries.Point _currentMousePosition;
        private Geometries.Point _downMousePosition;
        private bool _mouseDown;
        private Geometries.Point _previousMousePosition;
        private RenderMode _renderMode;
        private double _toResolution = double.NaN;
        private bool _hasBeenManipulated;
        private double _innerRotation;

        public MapControl()
        {
            Children.Add(WpfCanvas);
            Children.Add(SkiaCanvas);
            Children.Add(_selectRectangle);

            SkiaCanvas.IgnorePixelScaling = true;
            SkiaCanvas.PaintSurface += SKElementOnPaintSurface;

            Map = new Map();

            Loaded += MapControlLoaded;
            MouseLeftButtonDown += MapControlMouseLeftButtonDown;
            MouseLeftButtonUp += MapControlMouseLeftButtonUp;

            TouchUp += MapControlTouchUp;

            MouseMove += MapControlMouseMove;
            MouseLeave += MapControlMouseLeave;
            MouseWheel += MapControlMouseWheel;

            SizeChanged += MapControlSizeChanged;

            ManipulationStarted += OnManipulationStarted;
            ManipulationDelta += OnManipulationDelta;
            ManipulationCompleted += OnManipulationCompleted;
            ManipulationInertiaStarting += OnManipulationInertiaStarting;

            IsManipulationEnabled = true;

            RenderMode = RenderMode.Skia;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (RenderMode == RenderMode.Wpf) PaintWpf();
            base.OnRender(dc);
        }

        private static Rectangle CreateSelectRectangle()
        {
            return new Rectangle
            {
                Fill = new SolidColorBrush(Colors.Red),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 3,
                RadiusX = 0.5,
                RadiusY = 0.5,
                StrokeDashArray = new DoubleCollection { 3.0 },
                Opacity = 0.3,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Visibility = Visibility.Collapsed
            };
        }
        
        private bool IsInBoxZoomMode { get; set; }

        public bool ZoomToBoxMode { get; set; }

        public string ErrorMessage { get; private set; }

        public Canvas WpfCanvas { get; } = CreateWpfRenderCanvas();

        private SKElement SkiaCanvas { get; } = CreateSkiaRenderElement();

        public RenderMode RenderMode
        {
            get => _renderMode;
            set
            {
                if (value == RenderMode.Skia)
                {
                    WpfCanvas.Visibility = Visibility.Collapsed;
                    SkiaCanvas.Visibility = Visibility.Visible;
                    Renderer = new Rendering.Skia.MapRenderer();
                    RefreshGraphics();
                }
                else
                {
                    SkiaCanvas.Visibility = Visibility.Collapsed;
                    WpfCanvas.Visibility = Visibility.Visible;
                    Renderer = new MapRenderer();
                    RefreshGraphics();
                }
                _renderMode = value;
            }
        }

        private static Canvas CreateWpfRenderCanvas()
        {
            return new Canvas
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
        }

        private static SKElement CreateSkiaRenderElement()
        {
            return new SKElement
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
        }

        public event EventHandler ErrorMessageChanged;
        public event EventHandler<ViewChangedEventArgs> ViewChanged;
        public event EventHandler<FeatureInfoEventArgs> FeatureInfo;
        public event EventHandler ViewportInitialized;

        private void MapRefreshGraphics(object sender, EventArgs eventArgs)
        {
            RefreshGraphics();
        }

        private void MapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!Dispatcher.CheckAccess()) Dispatcher.BeginInvoke(new Action(() => MapPropertyChanged(sender, e)));
            else
            {
                if (e.PropertyName == nameof(Layer.Enabled))
                {
                    RefreshGraphics();
                }
                else if (e.PropertyName == nameof(Layer.Opacity))
                {
                    RefreshGraphics();
                }
            }
        }

        private void OnViewChanged(bool userAction = false)
        {
            if (_map == null) return;

            ViewChanged?.Invoke(this, new ViewChangedEventArgs { Viewport = Map.Viewport, UserAction = userAction });
        }

        public void RefreshGraphics()
        {
            Dispatcher.BeginInvoke(new Action(InvalidateCanvas));
        }

        internal void InvalidateCanvas()
        {
            if (RenderMode == RenderMode.Wpf) InvalidateVisual(); // To trigger OnRender of this MapControl
            else SkiaCanvas.InvalidateVisual();

        }

        public void RefreshData()
        {
            _map?.RefreshData(true);
        }

        public void Clear()
        {
            _map?.ClearCache();
            RefreshGraphics();
        }

        public void ZoomIn()
        {
            if (ZoomLock)
                return;

            if (double.IsNaN(_toResolution))
                _toResolution = Map.Viewport.Resolution;

            var resolution = ZoomHelper.ZoomIn(_map.Resolutions, _toResolution);

            _toResolution = ViewportLimiter.LimitResolution(resolution, ActualWidth, ActualHeight,
                _map.ZoomMode, _map.ZoomLimits, _map.Resolutions, _map.Envelope);

            ZoomMiddle();
        }

        public void ZoomOut()
        {
            if (double.IsNaN(_toResolution))
                _toResolution = Map.Viewport.Resolution;

            var resolution = ZoomHelper.ZoomOut(_map.Resolutions, _toResolution);

            _toResolution = ViewportLimiter.LimitResolution(resolution, ActualWidth, ActualHeight,
                _map.ZoomMode, _map.ZoomLimits, _map.Resolutions, _map.Envelope);

            ZoomMiddle();
        }

        private void OnErrorMessageChanged(EventArgs e)
        {
            ErrorMessageChanged?.Invoke(this, e);
        }

        private static void OnResolutionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var newResolution = (double)e.NewValue;
            ((MapControl)dependencyObject).ZoomToResolution(newResolution);
        }

        private void ZoomToResolution(double resolution)
        {
            var current = _currentMousePosition;

            Map.Viewport.Transform(current.X, current.Y, current.X, current.Y, Map.Viewport.Resolution / resolution);

            ViewportLimiter.Limit(_map.Viewport, _map.ZoomMode, _map.ZoomLimits, _map.Resolutions,
                _map.PanMode, _map.PanLimits, _map.Envelope);

            _map.RefreshData(true);
            OnViewChanged();
            RefreshGraphics();
        }

        private void ZoomMiddle()
        {
            _currentMousePosition = new Geometries.Point(ActualWidth / 2, ActualHeight / 2);
            StartZoomAnimation(Map.Viewport.Resolution, _toResolution);
        }

        private void MapControlLoaded(object sender, RoutedEventArgs e)
        {
            _scale = GetDeviceIndependentUnits();
            TryInitializeViewport();
            UpdateSize();
            InitAnimation();
            Focusable = true;
        }

        public float GetDeviceIndependentUnits()
        {
            if (RenderMode == RenderMode.Skia)
            {
                return DetermineSkiaScale();
            }
            return 1; // Scale is always 1 in WPF
        }

        private float DetermineSkiaScale()
        {
            var presentationSource = PresentationSource.FromVisual(this);
            if (presentationSource == null) throw new Exception("PresentationSource is null");
            var compositionTarget = presentationSource.CompositionTarget;
            if (compositionTarget == null) throw new Exception("CompositionTarget is null");

            var matrix = compositionTarget.TransformToDevice;

            var dpiX = matrix.M11;
            var dpiY = matrix.M22;

            if (dpiX != dpiY) throw new ArgumentException();

            return (float)dpiX;
        }

        private void InitAnimation()
        {
            _zoomAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 1000));
            _zoomAnimation.EasingFunction = new QuarticEase();
            Storyboard.SetTarget(_zoomAnimation, this);
            Storyboard.SetTargetProperty(_zoomAnimation, new PropertyPath("Resolution"));
            _zoomStoryBoard.Children.Add(_zoomAnimation);
        }

        private void MapControlMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!_map.Viewport.Initialized) return;
            if (ZoomLock) return;

            _currentMousePosition = e.GetPosition(this).ToMapsui();
            //Needed for both MouseMove and MouseWheel event for mousewheel event

            if (double.IsNaN(_toResolution))
                _toResolution = Map.Viewport.Resolution;

            if (e.Delta > Constants.Epsilon)
            {
                var resolution = ZoomHelper.ZoomIn(_map.Resolutions, _toResolution);

                _toResolution = ViewportLimiter.LimitResolution(resolution, ActualWidth, ActualHeight,
                    _map.ZoomMode, _map.ZoomLimits, _map.Resolutions, _map.Envelope);

            }
            else if (e.Delta < Constants.Epsilon)
            {
                var resolution = ZoomHelper.ZoomOut(_map.Resolutions, _toResolution);

                _toResolution = ViewportLimiter.LimitResolution(resolution, ActualWidth, ActualHeight,
                    _map.ZoomMode, _map.ZoomLimits, _map.Resolutions, _map.Envelope);
            }

            // Some cheating for personal gain. This workaround could be ommitted if the zoom animations was on CenterX, CenterY and Resolution, not Resolution alone.
            Map.Viewport.Center.X += 0.000000001;
            Map.Viewport.Center.Y += 0.000000001;

            Map.NavigateTo(_toResolution);
            //StartZoomAnimation(Map.Viewport.Resolution, _toResolution);
        }

        private void StartZoomAnimation(double begin, double end)
        {
            _zoomStoryBoard.Pause(); //using Stop() here causes unexpected results while zooming very fast.
            _zoomAnimation.From = begin;
            _zoomAnimation.To = end;
            _zoomAnimation.Completed += ZoomAnimationCompleted;
            _zoomStoryBoard.Begin();
        }

        private void ZoomAnimationCompleted(object sender, EventArgs e)
        {
            _toResolution = double.NaN;
        }

        private void MapControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            TryInitializeViewport();
            Clip = new RectangleGeometry { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
            UpdateSize();
            _map.RefreshData(true);
            OnViewChanged();
            Refresh();
        }

        private void UpdateSize()
        {
            if (Map.Viewport != null)
            {
                Map.Viewport.Width = ActualWidth;
                Map.Viewport.Height = ActualHeight;

                ViewportLimiter.Limit(_map.Viewport, _map.ZoomMode, _map.ZoomLimits, _map.Resolutions,
                    _map.PanMode, _map.PanLimits, _map.Envelope);
            }
        }

        private void MapControlMouseLeave(object sender, MouseEventArgs e)
        {
            _previousMousePosition = new Geometries.Point();
            ReleaseMouseCapture();
        }

        public void MapDataChanged(object sender, DataChangedEventArgs e) // todo: make private?
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new DataChangedEventHandler(MapDataChanged), sender, e);
            }
            else
            {
                if (e == null)
                {
                    ErrorMessage = "Unexpected error: DataChangedEventArgs can not be null";
                    OnErrorMessageChanged(EventArgs.Empty);
                }
                else if (e.Cancelled)
                {
                    ErrorMessage = "Cancelled";
                    OnErrorMessageChanged(EventArgs.Empty);
                }
                else if (e.Error is WebException)
                {
                    ErrorMessage = "WebException: " + e.Error.Message;
                    OnErrorMessageChanged(EventArgs.Empty);
                }
                else if (e.Error != null)
                {
                    ErrorMessage = e.Error.GetType() + ": " + e.Error.Message;
                    OnErrorMessageChanged(EventArgs.Empty);
                }
                else // no problems
                {
                    RefreshGraphics();
                }
            }
        }

        private void MapControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var touchPosition = e.GetPosition(this).ToMapsui();
            _previousMousePosition = touchPosition;
            _downMousePosition = touchPosition;
            _mouseDown = true;
            CaptureMouse();
            IsInBoxZoomMode = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (!IsInBoxZoomMode && !ZoomToBoxMode)
            {
                if (IsClick(_currentMousePosition, _downMousePosition))
                {
                    HandleFeatureInfo(e);
                    Map.InvokeInfo(touchPosition, _downMousePosition, 1, Renderer.SymbolCache,
                        WidgetTouched, e.ClickCount);
                }
            }
        }

        private void MapControlMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(this).ToMapsui();

            if (IsInBoxZoomMode || ZoomToBoxMode)
            {
                ZoomToBoxMode = false;
                
                var previous = Map.Viewport.ScreenToWorld(_previousMousePosition.X, _previousMousePosition.Y);
                var current = Map.Viewport.ScreenToWorld(mousePosition.X, mousePosition.Y);
                ZoomToBox(previous, current);
            }

            _map.RefreshData(true);
            OnViewChanged(true);
            _mouseDown = false;

            _previousMousePosition = new Geometries.Point();
            ReleaseMouseCapture();
        }

        private static bool IsClick(Geometries.Point currentPosition, Geometries.Point previousPosition)
        {
            return
                Math.Abs(currentPosition.X - previousPosition.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(currentPosition.Y - previousPosition.Y) < SystemParameters.MinimumVerticalDragDistance;
        }

        private void MapControlTouchUp(object sender, TouchEventArgs e)
        {
            if (!_hasBeenManipulated)
            {
                var touchPosition = e.GetTouchPoint(this).Position.ToMapsui();
                // todo: Pass the touchDown position. It needs to be set at touch down.

                // TODO Figure out how to do a number of taps for WPF
                Map.InvokeInfo(touchPosition, touchPosition, 1, Renderer.SymbolCache, WidgetTouched, 1);
            }
        }

        private void WidgetTouched(IWidget widget, Geometries.Point screenPosition)
        {
            if (widget is Hyperlink hyperlink)
            {
                Process.Start(hyperlink.Url);
            }

            widget.HandleWidgetTouched(screenPosition);
        }

        private void HandleFeatureInfo(MouseButtonEventArgs e)
        {
            if (FeatureInfo == null) return; // don't fetch if you the call back is not set.

            if (_downMousePosition == e.GetPosition(this).ToMapsui())
                foreach (var layer in Map.Layers)
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    (layer as IFeatureInfo)?.GetFeatureInfo(Map.Viewport, _downMousePosition.X, _downMousePosition.Y,
                        OnFeatureInfo);
                }
        }

        private void OnFeatureInfo(IDictionary<string, IEnumerable<IFeature>> features)
        {
            FeatureInfo?.Invoke(this, new FeatureInfoEventArgs { FeatureInfo = features });
        }

        private void MapControlMouseMove(object sender, MouseEventArgs e)
        {
            if (IsInBoxZoomMode || ZoomToBoxMode)
            {
                DrawBbox(e.GetPosition(this));
                return;
            }

            if (!_mouseDown) Map.InvokeHover(e.GetPosition(this).ToMapsui(), 1, Renderer.SymbolCache);

            if (_mouseDown && !PanLock)
            {
                if (_previousMousePosition == default(Geometries.Point))
                    return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown

                _currentMousePosition = e.GetPosition(this).ToMapsui(); //Needed for both MouseMove and MouseWheel event

                _map.Viewport.Transform(
                    _currentMousePosition.X, _currentMousePosition.Y,
                    _previousMousePosition.X, _previousMousePosition.Y);

                ViewportLimiter.Limit(_map.Viewport, _map.ZoomMode, _map.ZoomLimits, _map.Resolutions,
                    _map.PanMode, _map.PanLimits, _map.Envelope);

                _previousMousePosition = _currentMousePosition;
                _map.RefreshData(false);
                OnViewChanged(true);
                RefreshGraphics();

            }
        }

        private void TryInitializeViewport()
        {
            if (_map?.Viewport == null) return;
            if (_map.Viewport.Initialized) return;

            if (_map.Viewport.TryInitializeViewport(_map.Envelope, ActualWidth, ActualHeight))
            {
                ViewportLimiter.Limit(_map.Viewport, _map.ZoomMode, _map.ZoomLimits, _map.Resolutions,
                    _map.PanMode, _map.PanLimits, _map.Envelope);

                Map.RefreshData(true);
                OnViewportInitialized();
            }
        }

        private void OnViewportInitialized()
        {
            ViewportInitialized?.Invoke(this, EventArgs.Empty);
        }

        public void ZoomToBox(Geometries.Point beginPoint, Geometries.Point endPoint)
        {
            var width = Math.Abs(endPoint.X - beginPoint.X);
            var height = Math.Abs(endPoint.Y - beginPoint.Y);
            if (width <= 0) return;
            if (height <= 0) return;

            ZoomHelper.ZoomToBoudingbox(beginPoint.X, beginPoint.Y, endPoint.X, endPoint.Y,
                ActualWidth, ActualHeight, out var x, out var y, out var resolution);

            resolution = ViewportLimiter.LimitResolution(resolution, _map.Viewport.Width, _map.Viewport.Height,
                _map.ZoomMode, _map.ZoomLimits, _map.Resolutions, _map.Envelope);

            _map.Viewport.Resolution = resolution;
            Map.Viewport.Center = new Geometries.Point(x, y);

            _toResolution = resolution; // for animation

            _map.RefreshData(true);
            OnViewChanged(true);
            RefreshGraphics();
            ClearBBoxDrawing();
        }

        private void ClearBBoxDrawing()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _selectRectangle.Visibility = Visibility.Collapsed;
            }));
        }

        private void DrawBbox(Point newPos)
        {
            if (_mouseDown)
            {
                var from = _previousMousePosition;
                var to = newPos;

                if (from.X > to.X)
                {
                    var temp = from;
                    from.X = to.X;
                    to.X = temp.X;
                }

                if (from.Y > to.Y)
                {
                    var temp = from;
                    from.Y = to.Y;
                    to.Y = temp.Y;
                }

                _selectRectangle.Width = to.X - from.X;
                _selectRectangle.Height = to.Y - from.Y;
                _selectRectangle.Margin = new Thickness(from.X, from.Y, 0, 0);
                _selectRectangle.Visibility = Visibility.Visible;
            }
        }

        public void ZoomToFullEnvelope()
        {
            if (Map.Envelope == null) return;
            if (ActualWidth.IsNanOrZero()) return;
            Map.Viewport.Resolution = Math.Max(Map.Envelope.Width / ActualWidth, Map.Envelope.Height / ActualHeight);
            Map.Viewport.Center = Map.Envelope.GetCentroid();
        }

        private static void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 25 * 96.0 / (1000.0 * 1000.0);
        }

        private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            _hasBeenManipulated = false;
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var translation = e.DeltaManipulation.Translation;
            var center = e.ManipulationOrigin.ToMapsui().Offset(translation.X, translation.Y);
            var radius = GetDeltaScale(e.DeltaManipulation.Scale);
            var angle = e.DeltaManipulation.Rotation;
            var prevCenter = e.ManipulationOrigin.ToMapsui();
            var prevRadius = 1f;
            var prevAngle = 0f;

            _hasBeenManipulated |= Math.Abs(e.DeltaManipulation.Translation.X) > SystemParameters.MinimumHorizontalDragDistance
                     || Math.Abs(e.DeltaManipulation.Translation.Y) > SystemParameters.MinimumVerticalDragDistance;

            double rotationDelta = 0;

            if (!RotationLock)
            {
                _innerRotation += angle - prevAngle;
                _innerRotation %= 360;

                if (_innerRotation > 180)
                    _innerRotation -= 360;
                else if (_innerRotation < -180)
                    _innerRotation += 360;

                if (_map.Viewport.Rotation == 0 && Math.Abs(_innerRotation) >= Math.Abs(UnSnapRotationDegrees))
                    rotationDelta = _innerRotation;
                else if (_map.Viewport.Rotation != 0)
                {
                    if (Math.Abs(_innerRotation) <= Math.Abs(ReSnapRotationDegrees))
                        rotationDelta = -_map.Viewport.Rotation;
                    else
                        rotationDelta = _innerRotation - _map.Viewport.Rotation;
                }
            }

            _map.Viewport.Transform(center.X, center.Y, prevCenter.X, prevCenter.Y, radius / prevRadius, rotationDelta);

            ViewportLimiter.Limit(_map.Viewport, _map.ZoomMode, _map.ZoomLimits, _map.Resolutions,
                _map.PanMode, _map.PanLimits, _map.Envelope);

            OnViewChanged(true);
            e.Handled = true;
        }

        private double GetDeltaScale(XamlVector scale)
        {
            if (ZoomLock) return 1;
            var deltaScale = (scale.X + scale.Y) / 2;
            if (Math.Abs(deltaScale) < Constants.Epsilon)
                return 1; // If there is no scaling the deltaScale will be 0.0 in Windows Phone (while it is 1.0 in wpf)
            if (!(Math.Abs(deltaScale - 1d) > Constants.Epsilon)) return 1;
            return deltaScale;
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Refresh();
        }

        public MapInfo GetMapInfo(Geometries.Point screenPosition, int margin = 0)
        {
            return InfoHelper.GetMapInfo(Map.Viewport, screenPosition, 1, Map.InfoLayers, Renderer.SymbolCache, margin);
        }

        private void SKElementOnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            if (Renderer == null) return;
            if (_map == null) return;

            Debug.WriteLine(DateTime.Now.Ticks);
            TryInitializeViewport();
            if (!_map.Viewport.Initialized) return;

            Renderer.Render(args.Surface.Canvas, Map.Viewport, Map.Layers, Map.Widgets, Map.BackColor);
        }
        
        private void PaintWpf()
        {
            if (Renderer == null) return;
            if (_map == null) return;

            TryInitializeViewport();
            if (!_map.Viewport.Initialized) return;

            Renderer.Render(WpfCanvas, Map.Viewport, _map.Layers, Map.Widgets, _map.BackColor);
        }
    }
}
