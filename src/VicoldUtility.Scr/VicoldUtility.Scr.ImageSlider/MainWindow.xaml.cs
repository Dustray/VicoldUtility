using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace VicoldUtility.Scr.ImageSlider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _firstMoving = false;
        private LoopQueue<string> _loopQueue;
        private ImageManager _imageManager;
        private double _x;
        private double _y;

        public MainWindow()
        {
            InitializeComponent();
            _loopQueue = new LoopQueue<string>();
            _imageManager = new ImageManager();
            Mouse.OverrideCursor = Cursors.None;
            //var fi = new string[] { "*.jpg", "*.png", "*.jpeg", "*.bmp" };
            var files = Directory.GetFiles(@"F:\壁纸剪切好\Note9_Default_Wallpaper")
                .Where(file => file.ToLower().EndsWith("jpg")
                || file.ToLower().EndsWith("png")
                || file.ToLower().EndsWith("jpeg")
                || file.ToLower().EndsWith("bmp")).ToArray();
            _loopQueue.Add(files);
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            CloseWindow(e.GetPosition(this));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                ChangeImage(_loopQueue.Next());
                await Task.Delay(3000);
            }
        }

        public BitmapSource GetImage(string imagePath)
        {
            return _imageManager.GetSource(imagePath);
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //if (isRendering)
            //{
            //    if (index < bmList.Count)
            //    {
            //        this.imgBase.Source = bmList[index];
            //        this.imgBase.Width = this.imgBase.Source.Width;
            //        this.imgBase.Height = this.imgBase.Source.Height;

            //        index++;
            //    }
            //    else
            //    {
            //        index = 0;
            //    }
            //    isRendering = false;
            //}
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void ChangeImage(string newUrl)
        {
            this.Dispatcher.Invoke(() =>
            {
                var newImage = new Image();
                newImage.Stretch = Stretch.Fill;
                newImage.Source = GetImage(newUrl);
                if (baseGrid.Children.Count > 0)
                {
                    var lastImage = baseGrid.Children[0] as Image;
                    Panel.SetZIndex(lastImage, 1);
                    FloatElement(lastImage, 0);
                }

                Panel.SetZIndex(newImage, 0);
                baseGrid.Children.Add(newImage);
            });
        }

        /// <summary>
        /// 透明度动画
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="to"></param>
        public void FloatElement(Image elem, double to)
        {
            if (to == 1)
            {
                elem.Visibility = Visibility.Visible;
            }
            DoubleAnimation opacity = new DoubleAnimation()
            {
                To = to,
                Duration = new TimeSpan(0, 0, 0, 0, 500)
            };
            EventHandler handler = null;
            opacity.Completed += handler = (s, e) =>
            {
                opacity.Completed -= handler;
                if (to == 0)
                {
                    elem.Visibility = Visibility.Collapsed;
                    baseGrid.Children.Remove(elem);
                    //if (elem.Source is BitmapImage bi)
                    //{
                    //    bi.StreamSource.Dispose();
                    //}
                    elem.Source = null;
                }
                opacity = null;
                //GC.Collect();
            };
            elem.BeginAnimation(UIElement.OpacityProperty, opacity);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Close();
        }

        private void CloseWindow(Point posi)
        {
            if (_firstMoving)
            {
                if (_x != posi.X && _y != posi.Y)
                {
                    // Close();
                }
            }
            else
            {
                _x = posi.X;
                _y = posi.Y;
            }
            _firstMoving = true;
        }
    }
}
