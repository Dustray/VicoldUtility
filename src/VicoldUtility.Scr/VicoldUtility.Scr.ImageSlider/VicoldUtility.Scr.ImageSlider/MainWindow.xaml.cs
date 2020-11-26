using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
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
            var files = Directory.GetFiles(@"C:\Users\Dustray\Pictures\Build 2020\Desktop Wallpapers - Build 2020");
            var index = 0;
            imgBase.Source = new BitmapImage();

            for (var i = 0; i < 100; i++)
            {
                if (index >= files.Length)
                {
                    index = 0;
                }
                imgBase.Source = GetImage(files[index]);
                index++;
                Thread.Sleep(1);
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
    }
}
