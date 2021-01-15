using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VicoldGis.DefaultMapData;
using VicoldGis.VMap;
using VicoldGis.VMap.Handlers;

namespace VicoldGis.Pages
{
    /// <summary>
    /// MapPage.xaml 的交互逻辑
    /// </summary>
    public partial class MapPage : Page
    {
        public MapPage()
        {
            InitializeComponent();
            App.Current.Map2 = new MapBox();
            App.Current.Map2.OnRenderVisual = (eles) =>
            {
                foreach (var ele in eles)
                {
                    inside.AddVisual(ele);
                }
            };
            App.Current.Map2.OnRender = (eles) =>
            {
                foreach (var ele in eles)
                {
                    inside.Children.Add(ele);
                }
            };
            App.Current.Map2.OnRenderOne = (ele) =>
            {
                inside.Children.Add(ele);
            };
            App.Current.Map2.OnUnRender = (eles) =>
            {
                try
                {
                    foreach (var ele in eles)
                    {
                        inside.Children.Remove(ele);
                    }
                }
                catch { }
            };
        }

        #region map oprate \ mouse event

        Point previousPoint;
        Size previousOutSize;
        bool isLoaded = false;
        private Point mouseXY;
        private bool mouseDown;

        private void outsidewrapper_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.MiddleButton == MouseButtonState.Pressed || e.LeftButton == MouseButtonState.Pressed) && e.RightButton == MouseButtonState.Released)
            {
                outside.CaptureMouse();
                mouseDown = true;
                mouseXY = e.GetPosition(outside);
            }
            e.Handled = true;
        }

        private void outsidewrapper_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if ((e.MiddleButton == MouseButtonState.Pressed || e.LeftButton == MouseButtonState.Pressed) && e.RightButton == MouseButtonState.Released)
            {
                if (mouseDown)
                {
                    Domousemove(outside, e);
                }

            }
            e.Handled = true;
        }

        /// <summary>
        ///  鼠标移动时的事件，当鼠标按下并移动时发生
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        private void Domousemove(Border img, MouseEventArgs e)
        {
            var group = inside.RenderTransform as TransformGroup;
            var transform = group.Children[1] as TranslateTransform;
            var position = e.GetPosition(img);
            transform.X -= mouseXY.X - position.X;
            transform.Y -= mouseXY.Y - position.Y;
            mouseXY = position;
        }

        private void outside_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((e.MiddleButton == MouseButtonState.Released || e.LeftButton == MouseButtonState.Released) && e.RightButton == MouseButtonState.Released)
            {
                outside.ReleaseMouseCapture();
                mouseDown = false;
            }
            e.Handled = true;
        }

        private void outside_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var point = e.GetPosition(outside);
            var group = inside.RenderTransform as TransformGroup;
            var delta = e.Delta * 0.001;
            DowheelZoom(group, point, delta);
            var transform = group.Children[0] as ScaleTransform;
            //元素线条宽度不变
            foreach (var childObj in inside.Children)
            {
                var child = childObj as FrameworkElement;
                if (child != null)
                {
                    if (child.Tag is AdaptiveAntiZoomHandler handler)
                    {
                        handler.AntiScaling(transform.ScaleX);
                    }
                }
            }
            e.Handled = true;
        }
        /// <summary>
        /// 鼠标滑轮事件，得到坐标，放缩函数和滑轮指数，由于滑轮值变化较大所以*0.001.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="point"></param>
        /// <param name="delta"></param>
        private void DowheelZoom(TransformGroup group, Point point, double delta)
        {
            var pointToContent = group.Inverse.Transform(point);
            var transform = group.Children[0] as ScaleTransform;
            if ((delta < 0 && transform.ScaleX + delta < -0.1) || (delta > 0 && transform.ScaleX + delta > 20))
            {
                return;
            }
            transform.ScaleX += delta * transform.ScaleX;
            transform.ScaleY += delta * transform.ScaleY;

            App.Current.Map2.Manager.ScaleX = transform.ScaleX;
            App.Current.Map2.Manager.ScaleY = transform.ScaleY;
            var transform1 = group.Children[1] as TranslateTransform;
            transform1.X = -1 * ((pointToContent.X * transform.ScaleX) - point.X);
            transform1.Y = -1 * ((pointToContent.Y * transform.ScaleY) - point.Y);
        }
        #endregion

        private void inside_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (isLoaded)
            {
                Point outCenterPoint = new Point(
                    previousPoint.X / previousOutSize.Width * outside.ActualWidth,
                    previousPoint.Y / previousOutSize.Height * outside.ActualHeight);
                var currentPoint = outCenterPoint - previousPoint;
                TransformGroup group = inside.RenderTransform as TransformGroup;
                var transform1 = group.Children[1] as TranslateTransform;
                transform1.X += currentPoint.X;
                transform1.Y += currentPoint.Y;
                previousPoint = outCenterPoint;
                previousOutSize = new Size(outside.ActualWidth, outside.ActualHeight);
            }
        }

        private void outside_Loaded(object sender, RoutedEventArgs e)
        {
            Point currentPoint = new Point(outside.ActualWidth / 2, outside.ActualHeight / 2);  //不能用 inside，必须用outside
            TransformGroup tg = inside.RenderTransform as TransformGroup;
            tg.Children.Add(new TranslateTransform(currentPoint.X, currentPoint.Y));  //centerX和centerY用外部包装元素的坐标，不能用内部被变换的Canvas元素的坐标
                                                                                      //    inside.RenderTransform = tg;
            previousPoint = currentPoint;
            previousOutSize = new Size(outside.ActualWidth, outside.ActualHeight);


            isLoaded = true;

            App.Current.Map2.LoadCoordinateGrid();
            new DefaultDataManager().LoadProvinceLine();

            var transform = tg.Children[0] as ScaleTransform;
            transform.ScaleX = 0.182;
            transform.ScaleY = 0.182;
            var transform1 = tg.Children[1] as TranslateTransform;
            transform1.X = 705;
            transform1.Y = 1017;
            //元素线条宽度不变
            foreach (var childObj in inside.Children)
            {
                var child = childObj as FrameworkElement;
                if (child != null)
                {
                    if (child.Tag is AdaptiveAntiZoomHandler handler)
                    {
                        handler.AntiScaling(transform.ScaleX);
                    }
                }
            }

        }
    }
}
