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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VicoldUtility.MockLens
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageBytes? imageBytes_ = null;
        private WriteableBitmap? writeableBitmap_ = null;
        private ImageRuntionBuffer? buffer_ = null;
        private bool updating = false;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // read file to bitmap
            using Bitmap bitmap = new Bitmap(@"E:\十月一照片精选\IMG_4416.JPG");
            //using Bitmap bitmap = new Bitmap(@"F:\照片\2023.01.01元旦青岛\DSC01645.JPG");
            // get orientation from id: 274
            var orientation = bitmap.PropertyItems.FirstOrDefault(x => x.Id == 274);

            if (orientation is { } && orientation.Value is { })
            {
                int value = orientation.Value[0];
                Rotating(bitmap, value);
            }

            imageBytes_ = new ImageBytes(bitmap);
            (int newWidth, int newHeight) = GetImageSizeByMaxSize((int)ImageSourceFrame.ActualWidth, (int)ImageSourceFrame.ActualHeight);
            LoadBitmap(newWidth, newHeight);
        }
        private (int width, int height) GetImageSizeByMaxSize(int maxWidth, int maxHeight)
        {
            if (imageBytes_ is not { })
            {
                return (0, 0);
            }

            var width = imageBytes_.Width;
            var height = imageBytes_.Height;
            if (width > maxWidth || height > maxHeight)
            {
                var ratio = (double)width / height;
                if (ratio > 1)
                {
                    width = maxWidth;
                    height = (int)(width / ratio);
                }
                else
                {
                    height = maxHeight;
                    width = (int)(height * ratio);
                }
            }

            return (width, height);
        }

        /// <summary>
        /// 加载图片（当尺寸变化）
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void LoadBitmap(int width, int height)
        {
            if (imageBytes_ is not { })
            {
                return;
            }

            if (buffer_ is not { })
            {
                buffer_ = imageBytes_.CreateSourceZoomCopy(width, height);
            }
            else if (width - buffer_.Width > 10 || height - buffer_.Height > 10)
            {
                buffer_ = imageBytes_.CreateSourceZoomCopy(width, height);
            }
            else
            {
                return;
            }
            writeableBitmap_ = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            ImageSource.Source = writeableBitmap_;

            Update();
        }

        public void Update()
        {
            if (updating)
            {
                return;
            }

            if (buffer_ is not { })
            {
                return;
            }

            byte[]? buffer = buffer_.Data;
            if (buffer is not { })
            {
                return;
            }

            //this.Dispatcher.Invoke(() =>
            //{
            updating = true;
            writeableBitmap_?.WritePixels(new Int32Rect(0, 0, buffer_.Width, buffer_.Height), buffer, writeableBitmap_.BackBufferStride, 0);
            updating = false;
            //});
        }
        public static void Rotating(Bitmap img, int orien)
        {
            switch (orien)
            {
                case 2:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX); //horizontal flip
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone); //right-top
                    break;
                case 4:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY); //vertical flip
                    break;
                case 5:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone); //right-top
                    break;
                case 7:
                    img.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone); //left-bottom
                    break;
                default:
                    break;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            (int newWidth, int newHeight) = GetImageSizeByMaxSize((int)ImageSourceFrame.ActualWidth, (int)ImageSourceFrame.ActualHeight);
            LoadBitmap(newWidth, newHeight);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (buffer_ is not { })
            {
                return;

            }
            buffer_.Add((short)AddSlider.Value);
            Update();
        }
    }

}
