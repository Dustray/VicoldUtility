using System;
using System.Collections.Generic;
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
using VicoldUtility.AvatarMaker.ImageLoader;

namespace VicoldUtility.AvatarMaker.Pages
{
    
    /// <summary>
    /// PreviewPage.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewPage : Page
    {
        private ImageManager _imageManager;

        public PreviewPage()
        {
            InitializeComponent();
            _imageManager = new ImageManager((int)ActualWidth, (int)ActualHeight);
            var bitmapImage = new BitmapImage(new Uri("image_path_here"));
            var decoder = new PngBitmapDecoder(bitmapImage.UriSource, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            var frame = decoder.Frames[0];
            var stride = frame.PixelWidth * 4;
            var data = new byte[stride * frame.PixelHeight];
            frame.CopyPixels(data, stride, 0);
            _imageManager.AddLayer("background", 0, data);
            imageControl.Source = _imageManager.Bitmap;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _imageManager.Draw();
        }
    }
}
