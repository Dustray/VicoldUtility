using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VicoldUtility.PhotoLens.Algorithm.Isoline;
using Color = System.Drawing.Color;

namespace VicoldUtility.PhotoLens
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            e.Handled = false;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            //仅支持文件的拖放
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }
            //获取拖拽的文件
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var file = files.First();

            var bp = new Bitmap(file);
            var width = bp.Width;
            var height = bp.Height;
            float[] values = new float[bp.Width * bp.Height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    values[y * bp.Width + x] = bp.GetPixel(x, y).GetBrightness() * 255;
                }
            }
            bp.Dispose();
            // var newAnalyzeValue = new float[] { 0, 25, 50, 75, 100, 125, 150, 175, 200, 225, 250 };
            var newAnalyzeValue = new float[] { 0, 50, 100, 150, 200,  250 };
            Task.Run(() =>
            {
                var startX = 0;
                var startY = 0;

                var line = Algorithm.AlgorithmFactory.CreateIsolineLoader(values, startX, startY, width, height)
                .SetAnalyzeValue(newAnalyzeValue)
                .SetSmoothCount(0)
                .Build();
                Dispatcher.Invoke(new Action<int,int>((w,h) =>
                {
                    //DrawGray(w,h,startX, startY, line);
                    using var image = DrawImage(w, h, startX, startY, line);
                    image.Save(@"D:\photolens.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }), width,height);
            });
        }



        private Bitmap DrawImage(int width, int height, int startX, int startY, IEnumerable<ValueLine> lines)
        {
            var _myImage = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(_myImage); //申请画图对象
                g.Clear(Color.White);
                foreach (var line in lines)
                {

                    var p = new System.Drawing.Pen(Color.FromArgb((byte)line.Value, 0, 0, 0), 1);//设置笔的粗细为5,颜色为蓝色
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;//定义虚线的样式为点
                    var pof = new PointF[line.LinePoints.Length / 2];
                    for (var i = 0; i < line.LinePoints.Length / 2; i++)
                    {
                        pof[i] = new PointF(startX + line.LinePoints[2 * i] , startY + line.LinePoints[2 * i + 1] );
                    }

                    g.DrawLines(p, pof);
                }

                return _myImage;

        }



        private void DrawGray(int width, int height, int startX, int startY, IEnumerable<ValueLine> lines)
        {
            var vLines = MakeVisualMiltiLine(lines);
            foreach(var line in vLines)
            {
                MainImg.AddVisual(line);
            }
            
        }

        public static List<Visual> MakeVisualMiltiLine(IEnumerable<ValueLine> lines)
        {
            var paths = new List<Visual>();
            foreach (var line in lines)
            {
                if (line.LinePoints.Length < 2)
                {
                    continue;
                }

                DrawingVisual visual = new DrawingVisual();
                DrawingContext dc = visual.RenderOpen();
                System.Windows.Media.Pen pen = new System.Windows.Media.Pen(
                    new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, (byte)line.Value, (byte)line.Value, (byte)line.Value)), 2);
                pen.Freeze();  //冻结画笔，这样能加快绘图速度

                for (int i = 0; i < line.LinePoints.Length - 4; i+=4)
                {
                    var point1 = new System.Windows.Point(line.LinePoints[i], line.LinePoints[i + 1]); 
                    var point2 = new System.Windows.Point(line.LinePoints[i+2], line.LinePoints[i + 3]); 
                    dc.DrawLine(pen, point1, point2);
                }

                dc.Close();
                paths.Add(visual);
            }

            return paths;
        }


    }
}
