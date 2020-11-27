using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VicoldUtility.Scr.ImageSlider
{
    internal class ImageManager : IDisposable
    {

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);


        public BitmapSource GetSource(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    using(var bmp = new Bitmap(ms))
                    {
                       return ToBitmapSource(bmp);
                    }
                }
            }
            return null;
        }
        private BitmapSource ToBitmapSource(Bitmap bmp)
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
