using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace VicoldUtility.Scr.ImageSlider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _firstMoving = false;
        ObservableCollection<BitmapImage> bmList;

        private LoopQueue<string> _loopQueue;

        int index = 0;  //记录索引
        bool isRendering = false;
        public MainWindow()
        {
            InitializeComponent();
            var files = Directory.GetFiles(@"F:\壁纸剪切好\Note9_Default_Wallpaper", "(.jpg|.png|.jpeg|.bmp)");
            _loopQueue.Add(files);
            InitList();
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_firstMoving)
            {
                Close();
            }
            _firstMoving = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                isRendering = true;
                Thread.Sleep(1000); 
            }
        }

        public static BitmapImage GetImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();

            if (File.Exists(imagePath))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
            }

            return bitmap;
        }

        public void InitList()
        {
            //bmList = new ObservableCollection<BitmapImage>();
            //var length = files.Length;
            //for (int i = 0; i < length; i++)
            //{
            //    BitmapImage bmImg = new BitmapImage(new Uri(files[i]));
            //    bmList.Add(bmImg);
            //}
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (isRendering)
            {
                if (index < bmList.Count)
                {
                    this.imgBase.Source = bmList[index];
                    this.imgBase.Width = this.imgBase.Source.Width;
                    this.imgBase.Height = this.imgBase.Source.Height;

                    index++;
                }
                else
                {
                    index = 0;
                }
                isRendering = false;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
