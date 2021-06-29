using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VicoldUtility.ImageClip.Pages
{
    /// <summary>
    /// CanvasPage.xaml 的交互逻辑
    /// </summary>
    public partial class CanvasPage : Page
    {
        private bool mouseDown;
        private Point mouseXY;

        /// <summary>
        /// 坐标点
        /// </summary>
        private List<Point> points;
        /// <summary>
        /// 直线
        /// </summary>
        private List<Line> lines = null;
        /// <summary>
        /// 定位点
        /// </summary>
        //private List<AnchorPoint> anchorPoints = null;

        /// <summary>
        /// 是否鼠标为按下
        /// </summary>
        private bool isMouseDown = false;

        /// <summary>
        /// 当前选中定位点
        /// </summary>
        //private AnchorPoint curAnchorPoint = null;


        public CanvasPage()
        {
            InitializeComponent();
        }

        #region 图片缩放
        private void IMG1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            img.CaptureMouse();
            mouseDown = true;
            mouseXY = e.GetPosition(img);
        }

        /// <summary>
        /// 鼠标按下时的事件，启用捕获鼠标位置并把坐标赋值给mouseXY.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IMG1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            img.ReleaseMouseCapture();
            mouseDown = false;
        }

        /// <summary>
        /// 鼠标松开时的事件，停止捕获鼠标位置。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IMG1_MouseMove(object sender, MouseEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            if (mouseDown)
            {
                Domousemove(img, e);
            }
        }

        /// <summary>
        ///  鼠标移动时的事件，当鼠标按下并移动时发生
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        private void Domousemove(ContentControl img, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var transform = group.Children[1] as TranslateTransform;
            var position = e.GetPosition(img);
            transform.X -= mouseXY.X - position.X;
            transform.Y -= mouseXY.Y - position.Y;
            mouseXY = position;
        }



        /// <summary>
        /// group.Children中的第二个是移动的函数
        /// 它根据X.Y的值来移动。并把当前鼠标位置赋值给mouseXY.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IMG1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            var point = e.GetPosition(img);
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var delta = e.Delta * 0.001;
            DowheelZoom(group, point, delta);
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
            if ((delta < 0 && transform.ScaleX + delta < 0.1) || (delta > 0 && transform.ScaleX + delta > 20))
            {
                return;
            }
            transform.ScaleX += delta * transform.ScaleX;
            transform.ScaleY += delta * transform.ScaleY;
            var transform1 = group.Children[1] as TranslateTransform;
            transform1.X = -1 * ((pointToContent.X * transform.ScaleX) - point.X);
            transform1.Y = -1 * ((pointToContent.Y * transform.ScaleY) - point.Y);
        }

        #endregion


        //#region canvas
        //public void Init()
        //{
        //    //按x轴分类
        //    IEnumerable<IGrouping<double, Point>> pointXs = points.GroupBy(o => o.X);
        //    //按y周分类
        //    IEnumerable<IGrouping<double, Point>> pointYs = points.GroupBy(o => o.Y);
        //    //绘制竖线
        //    DrawXLine(pointXs);
        //    //绘制横线
        //    DrawYLine(pointYs);
        //    //设置定位点
        //    AddAnchorPoints();
        //    //绘制定位点并且添加事件
        //    foreach (AnchorPoint anchorPoint in anchorPoints)
        //    {
        //        Rectangle rec = anchorPoint.Draw();
        //        rec.MouseLeftButtonDown += new MouseButtonEventHandler(rec_MouseLeftButtonDown);
        //        rec.MouseMove += new MouseEventHandler(rec_MouseMove);
        //        canvas.Children.Add(rec);
        //    }
        //    //canvas添加事件
        //    canvas.MouseLeftButtonUp += new MouseButtonEventHandler(canvas_MouseLeftButtonUp);
        //    canvas.MouseMove += new MouseEventHandler(canvas_MouseMove);
        //    canvas.MouseLeave += new MouseEventHandler(canvas_MouseLeave);
        //}
        //public void Move(double x, double y)
        //{
        //    double offset = this.Width / 2;
        //    this.retc.Margin = new Thickness(x - offset, y - offset, 0, 0);
        //    this.X = x;
        //    this.Y = y;
        //}
        //public Rectangle Draw()
        //{
        //    double offset = this.Width / 2;
        //    Rectangle retc = new Rectangle()
        //    {
        //        Margin = new Thickness(this.X - offset, this.Y - offset, 0, 0),
        //        Width = this.Width,
        //        Height = this.Height,
        //        Fill = Brushes.LightGoldenrodYellow,
        //        Stroke = Brushes.Black,
        //        StrokeThickness = 1,
        //        DataContext = this.Key
        //    };
        //    this.retc = retc;
        //    return retc;
        //}
        //private void MoveLines(double x, double y)
        //{
        //    List<Line> moveLines = new List<Line>();
        //    moveLines = lines.Where(o => o.Y1 == curAnchorPoint.Y
        //        || o.Y2 == curAnchorPoint.Y
        //        || o.X1 == curAnchorPoint.X
        //        || o.X2 == curAnchorPoint.X).ToList();
        //    foreach (Line line in moveLines)
        //    {
        //        if (line.Y1 == curAnchorPoint.Y)
        //        {
        //            line.Y1 = y;
        //        }
        //        if (line.Y2 == curAnchorPoint.Y)
        //        {
        //            line.Y2 = y;
        //        }
        //        if (line.X1 == curAnchorPoint.X)
        //        {
        //            line.X1 = x;
        //        }
        //        if (line.X2 == curAnchorPoint.X)
        //        {
        //            line.X2 = x;
        //        }
        //    }
        //}
        //private void MoveRefAnchorPoint(double x, double y, AnchorPoint movedAnchorPoint)
        //{
        //    foreach (AnchorPoint anchorPoint in anchorPoints)
        //    {
        //        if (anchorPoint.RefPoint.Length == 2)
        //        {
        //            if (anchorPoint.RefPoint[0].X == x && anchorPoint.RefPoint[0].Y == y)
        //            {
        //                anchorPoint.RefPoint[0].X = movedAnchorPoint.X;
        //                anchorPoint.RefPoint[0].Y = movedAnchorPoint.Y;
        //            }
        //            else if (anchorPoint.RefPoint[1].X == x && anchorPoint.RefPoint[1].Y == y)
        //            {
        //                anchorPoint.RefPoint[1].X = movedAnchorPoint.X;
        //                anchorPoint.RefPoint[1].Y = movedAnchorPoint.Y;
        //            }
        //            anchorPoint.X = (anchorPoint.RefPoint[0].X + anchorPoint.RefPoint[1].X) / 2;
        //            anchorPoint.Y = (anchorPoint.RefPoint[0].Y + anchorPoint.RefPoint[1].Y) / 2;
        //            anchorPoint.Move();
        //        }
        //    }
        //}
        //#endregion

    }
    public enum AnchorPointType
    {
        /// <summary>
        /// 上下
        /// </summary>
        NS,
        /// <summary>
        /// 左右
        /// </summary>
        WE,
        /// <summary>
        /// 右上
        /// </summary>
        NE,
        /// <summary>
        /// 左下
        /// </summary>
        SW,
        /// <summary>
        /// 右下
        /// </summary>
        NW,
        /// <summary>
        /// 左上
        /// </summary>
        SE
    }
}
