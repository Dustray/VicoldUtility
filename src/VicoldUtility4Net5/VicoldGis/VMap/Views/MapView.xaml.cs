using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VicoldGis.VMap.Views
{
    /// <summary>
    /// MapView.xaml 的交互逻辑
    /// </summary>
    public partial class MapView : Canvas
    {
        public MapView()
        {
            InitializeComponent();
        }
        private List<Visual> visuals = new List<Visual>();

        ////获取Visual的个数
        //protected override int VisualChildrenCount
        //{
        //    get { return visuals.Count; }
        //}

        ////获取Visual
        //protected override Visual GetVisualChild(int index)
        //{
        //    return visuals[index];
        //}

        //添加Visual
        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        //删除Visual
        public void RemoveVisual(Visual visual)
        {
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }

        //命中测试
        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult.VisualHit as DrawingVisual;
        }

        //使用DrawVisual画Polyline
        public Visual Polyline(PointCollection points, Brush color, double thinkness)
        {
            DrawingVisual visual = new DrawingVisual();
            DrawingContext dc = visual.RenderOpen();
            Pen pen = new Pen(color, thinkness);
            pen.Freeze();  //冻结画笔，这样能加快绘图速度

            for (int i = 0; i < points.Count - 1; i++)
            {
                dc.DrawLine(pen, points[i], points[i + 1]);
            }

            dc.Close();
            return visual;
        }

        //使用DrawVisual画Polyline
        public Visual Polyline(Point[] points, Brush color, double thinkness)
        {
            DrawingVisual visual = new DrawingVisual();
            DrawingContext dc = visual.RenderOpen();
            Pen pen = new Pen(color, thinkness);
            pen.Freeze();  //冻结画笔，这样能加快绘图速度

            for (int i = 0; i < points.Length - 1; i++)
            {
                dc.DrawLine(pen, points[i], points[i + 1]);
            }

            dc.Close();
            return visual;
        }
    }
}
