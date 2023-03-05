using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
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
using VicoldUtility.MockLens.WriteableBuffer;
using VicoldUtility.MockLens.WriteableBuffer.BufferOperators;

namespace VicoldUtility.MockLens
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageBytes? imageBytes_ = null;
        private WriteableBitmap? writeableBitmap_ = null;
        private OperatorManager operatorManager_;

        private LinearOperator linearOperator = new LinearOperator();

        public MainWindow()
        {
            InitializeComponent();
            operatorManager_ = App.Current.OperatorManager;
            operatorManager_.RegisterContainer(ControlFrame);
            operatorManager_.OnUpdated = Updated;

            operatorManager_.AddOperator(linearOperator);
            operatorManager_.AddOperator(new ContrastOperator("对比度 - 全局", ChannelColor.ChannelType.All));
            operatorManager_.AddOperator(new ContrastOperator("对比度 - 红", ChannelColor.ChannelType.Red));
            operatorManager_.AddOperator(new ContrastOperator("对比度 - 绿", ChannelColor.ChannelType.Green));
            operatorManager_.AddOperator(new ContrastOperator("对比度 - 蓝", ChannelColor.ChannelType.Blue));

            operatorManager_.AddOperator(new HueOperator("色相 - 红", ChannelColor.ChannelType.Red));
            operatorManager_.AddOperator(new HueOperator("色相 - 绿", ChannelColor.ChannelType.Green));
            operatorManager_.AddOperator(new HueOperator("色相 - 蓝", ChannelColor.ChannelType.Blue));
            operatorManager_.AddOperator(new HueOperator("色相 - 黄", ChannelColor.ChannelType.Yellow));
            operatorManager_.AddOperator(new HueOperator("色相 - 洋红", ChannelColor.ChannelType.Magenta));
            operatorManager_.AddOperator(new HueOperator("色相 - 青", ChannelColor.ChannelType.Cyan));


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // read file to bitmap
            //using Bitmap bitmap = new Bitmap(@"E:\十月一照片精选\IMG_4416.JPG");
            using Bitmap bitmap = new Bitmap(@"F:\照片\2023.01.01元旦青岛\DSC01645.JPG");
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

            if (operatorManager_.RuntimeBuffer is not { })
            {
                operatorManager_.RuntimeBuffer = imageBytes_.CreateSourceZoomCopy(width, height);
                operatorManager_.RuntimeBuffer = operatorManager_.RuntimeBuffer;
            }
            else if (width - operatorManager_.RuntimeBuffer.Width > 10 || height - operatorManager_.RuntimeBuffer.Height > 10)
            {
                operatorManager_.RuntimeBuffer = imageBytes_.CreateSourceZoomCopy(width, height);
                operatorManager_.RuntimeBuffer = operatorManager_.RuntimeBuffer;
            }
            else
            {
                return;
            }
            writeableBitmap_ = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            ImageSource.Source = writeableBitmap_;

            operatorManager_.ExecuteCalculation();
        }

        public void Updated()
        {
            if (operatorManager_.RuntimeBuffer is not { })
            {
                return;
            }

            byte[]? buffer = operatorManager_.RuntimeBuffer.Data;
            if (buffer is not { })
            {
                return;
            }

            this.Dispatcher.Invoke(() =>
            {
                writeableBitmap_?.WritePixels(new Int32Rect(0, 0, operatorManager_.RuntimeBuffer.Width, operatorManager_.RuntimeBuffer.Height), buffer, writeableBitmap_.BackBufferStride, 0);
            });
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

        //private void LinearSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    linearOperator.Update((int)LinearSL.Value);
        //    UpdateOperator();
        //}

        //private void ContrastSL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    contrastOperator.Update((float)ContrastSL.Value / 10);
        //    UpdateOperator();
        //}

        //private void UpdateOperator()
        //{
        //    if (operatorManager_ is not { })
        //    {
        //        return;
        //    }

        //    locker.Lock(() =>
        //    {
        //        return Task.Run(async () =>
        //        {
        //            await operatorManager_.ExecAsync();
        //            Update();
        //        });
        //    });

        //}
    }
}
