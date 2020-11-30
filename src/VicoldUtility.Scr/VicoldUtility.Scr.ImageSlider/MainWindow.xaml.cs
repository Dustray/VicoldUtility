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
            Properties.Settings.Default.Upgrade();
            _loopQueue = new LoopQueue<string>();
            _imageManager = new ImageManager(this);
            _imageManager.AddToContainer(baseGrid);
            var folderPath = Properties.Settings.Default.FolderPath;
            if (!Directory.Exists(folderPath))
            {
                folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }

            // Mouse.OverrideCursor = Cursors.None;
            //var fi = new string[] { "*.jpg", "*.png", "*.jpeg", "*.bmp" };
            var files = Directory.GetFiles(folderPath)
                .Where(file => file.ToLower().EndsWith("jpg")
                || file.ToLower().EndsWith("png")
                || file.ToLower().EndsWith("jpeg")
                || file.ToLower().EndsWith("bmp")).ToArray();
            if (files.Length == 0)
            {
                files = new string[6];
                for (var i = 1; i < 7; i++)
                {
                    files[i - 1] = $@"./Asserts/default{i}.jpg";
                }
            }

            _loopQueue.Add(files);
            _loopQueue.IsRandom = Properties.Settings.Default.IsRandom;
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
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            if (args.Length == 0)
            {
                return;
            }

            //switch (args[0].ToLower())
            //{
            //    case "/s": // 预览
            //        break;
            //    case "/c": // 设置
            //        new SettingWindow().Show();
            //        break;
            //    case "/p": // 预览窗格
            //        break;
            //        Close();
            //}
            //MessageBox.Show(args[0]);
            if (args[0].ToLower().StartsWith("/c"))
            {
                new SettingWindow().Show();
                Close();
            }else if (args[0].ToLower().StartsWith("/p"))
            {
                Close();
            }
            //MessageBox.Show(args[0]);
        }

        private async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                //ChangeImage(_loopQueue.Next());
                //MessageBox.Show(_loopQueue.Next());
                _imageManager.Next(_loopQueue.Next());
                await Task.Delay(Properties.Settings.Default.ExchangeSpeed * 1000);
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

            //this.Dispatcher.Invoke(() =>
            //{
            //    var newImage = new Image();
            //    newImage.Stretch = Stretch.Fill;
            //    newImage.Source = GetImage(newUrl);
            //    if (baseGrid.Children.Count > 0)
            //    {
            //        var lastImage = baseGrid.Children[0] as Image;
            //        Panel.SetZIndex(lastImage, 1);
            //        FloatElement(lastImage, 0);
            //    }

            //    Panel.SetZIndex(newImage, 0);
            //    baseGrid.Children.Add(newImage);
            //});
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S)
            {
                new SettingWindow().Show();
            }
            else
            {
                Close();
            }
        }

        private void CloseWindow(Point posi)
        {
            if (_firstMoving)
            {
                if (_x != posi.X && _y != posi.Y)
                {
                    Close();
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
