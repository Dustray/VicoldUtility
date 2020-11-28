using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace VicoldUtility.Scr.ImageSlider
{
    internal class ImageManager : IDisposable
    {
        private Image _firstImage;
        private Image _lastImage;
        private bool _isFirstImage = true;
        private Window _thisWindow;

        public ImageManager(Window thisWindow)
        {
            _thisWindow = thisWindow;
            _firstImage = new Image();
            _firstImage.Name = "first";
            _firstImage.Stretch = Stretch.UniformToFill;
            _firstImage.StretchDirection = StretchDirection.Both;
            _lastImage = new Image();
            _lastImage.Name = "last";
            _lastImage.Stretch = Stretch.UniformToFill;
            _lastImage.StretchDirection = StretchDirection.Both;
        }

        public void AddToContainer(Panel ui)
        {
            ui.Children.Add(_firstImage);
            ui.Children.Add(_lastImage);
        }

        public void Next(string url)
        {
            _thisWindow.Dispatcher.Invoke(() =>
            {
                var newI = _isFirstImage ? _firstImage : _lastImage;
                var oldI = _isFirstImage ? _lastImage : _firstImage;
                _isFirstImage = !_isFirstImage;

                Panel.SetZIndex(oldI, 1);
                FloatElement(oldI, 0);

                newI.Visibility = Visibility.Visible;
                newI.Opacity = 1;
                newI.Source = GetSource(url);
                Panel.SetZIndex(newI, 0);
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
                Duration = new TimeSpan(0, 0, 0, 0, 1000)
            };
            opacity.FillBehavior = FillBehavior.Stop;
            EventHandler handler = null;
            opacity.Completed += handler = (s, e) =>
            {
                opacity.Completed -= handler;
                if (to == 0)
                {
                    elem.Visibility = Visibility.Collapsed;
                    //if (elem.Source is BitmapImage bi)
                    //{
                    //    bi.StreamSource.Dispose();
                    //}
                    //elem.Source = null;
                }
                opacity = null;
                //GC.Collect();
            };
            elem.BeginAnimation(UIElement.OpacityProperty, opacity);
        }

        public void Dispose()
        {

        }


        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);


        public BitmapSource GetSource(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    using (var bmp = new System.Drawing.Bitmap(ms))
                    {
                        return ToBitmapSource(bmp);
                    }
                }
            }
            return null;
        }
        private BitmapSource ToBitmapSource(System.Drawing.Bitmap bmp)
        {
            try
            {
                var ptr = bmp.GetHbitmap();
                var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(ptr);
                return source;
            }
            catch
            {
                return null;
            }
        }
    }
}
