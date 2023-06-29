using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace VicoldUtility.AvatarMaker.ImageLoader
{
    internal class ImageManager
    {
        private readonly Dictionary<string, Layer> _layers = new Dictionary<string, Layer>();
        private readonly WriteableBitmap _bitmap;
        public ImageManager(int width, int height)
        {
            _bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
        }

        public WriteableBitmap Bitmap => _bitmap;
        
        public void AddLayer(string id, int level, byte[] data)
        {
            if (_layers.ContainsKey(id))
            {
                throw new ArgumentException($"Layer with id '{id}' already exists.");
            }
            var layer = new Layer { Id = id, Level = level, Data = data };
            layer.Bitmap = _bitmap.Clone();
            layer.Bitmap.Lock();
            layer.Bitmap.WritePixels(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight), data, _bitmap.PixelWidth * 4, 0);
            layer.Bitmap.Unlock();
            _layers[id] = layer;
        }
        public void RemoveLayer(string id)
        {
            if (_layers.TryGetValue(id, out var layer))
            {
                _layers.Remove(id);
                layer.Bitmap = null;
                layer.Data = null;
            }
        }
        public Layer GetLayer(string id)
        {
            return _layers.TryGetValue(id, out var layer) ? layer : null;
        }

        //public void Draw()
        //{
        //    _bitmap.Lock();
        //    _bitmap.Clear();
        //    var layers = _layers.OrderBy(layer => layer.Value.Level).ToList();
        //    foreach (var layer in layers)
        //    {
        //        if (layer.Value.Bitmap != null)
        //        {
        //            _bitmap.Blit(new Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight), layer.Value.Bitmap, new Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight), WriteableBitmapExtensions.BlendMode.Alpha);
        //        }
        //    }
        //    _bitmap.Unlock();
        //}

        //public static void Draw(ImageManager imageManager, Layer layer, int x, int y)
        //{
        //    WriteableBitmap targetBitmap = imageManager.GetTargetBitmap(layer);
        //    WriteableBitmap sourceBitmap = layer.Bitmap;
        //    int targetWidth = targetBitmap.PixelWidth;
        //    int targetHeight = targetBitmap.PixelHeight;
        //    int sourceWidth = sourceBitmap.PixelWidth;
        //    int sourceHeight = sourceBitmap.PixelHeight;
        //    int stride = targetWidth * 4;
        //    int sourceStride = sourceWidth * 4;
        //    int sourceOffset = x * 4 + y * sourceStride;
        //    int targetOffset = 0;

        //    // Check if target region is within target bitmap bounds
        //    if (x + sourceWidth > targetWidth || y + sourceHeight > targetHeight)
        //    {
        //        throw new ArgumentException("Source bitmap does not fit within target bitmap bounds.");
        //    }

        //    // Copy pixel data from source to target bitmap
        //    for (int row = 0; row < sourceHeight; row++)
        //    {
        //        sourceBitmap.CopyPixels(new Int32Rect(0, row, sourceWidth, 1), targetBitmap.BackBuffer, sourceStride, sourceOffset);
        //        targetOffset += stride;
        //        sourceOffset += sourceStride;
        //    }

        //    // Invalidate the updated region of the target bitmap
        //    int left = x;
        //    int top = y;
        //    int right = x + sourceWidth;
        //    int bottom = y + sourceHeight;
        //    imageManager.InvalidateRegion(layer, left, top, right, bottom);
        //}
        public static void Draw(WriteableBitmap parentBmp, WriteableBitmap childBmp, int x, int y)
        {
            int parentWidth = parentBmp.PixelWidth;
            int parentHeight = parentBmp.PixelHeight;
            int childWidth = childBmp.PixelWidth;
            int childHeight = childBmp.PixelHeight;
            // 限定x和y的范围
            x = Math.Max(0, Math.Min(x, parentWidth - childWidth));
            y = Math.Max(0, Math.Min(y, parentHeight - childHeight));
            // 计算子图像和父图像每行的像素字节数
            int childStride = childWidth * 4;
            int parentStride = parentWidth * 4;
            // 获取子图像的像素数组
            byte[] childPixels = new byte[childStride * childHeight];
            childBmp.CopyPixels(childPixels, childStride, 0);
            // 获取父图像的像素数组
            byte[] parentPixels = new byte[parentStride * parentHeight];
            parentBmp.CopyPixels(parentPixels, parentStride, 0);
            // 绘制子图像到父图像
            for (int row = 0; row < childHeight; row++)
            {
                int parentOffset = (x + (y + row) * parentWidth) * 4;
                int childOffset = row * childStride;
                if (parentOffset < 0 || parentOffset >= parentPixels.Length)
                {
                    continue;
                }
                if (childOffset < 0 || childOffset >= childPixels.Length)
                {
                    continue;
                }
                Array.Copy(childPixels, childOffset, parentPixels, parentOffset, childStride);
            }
            // 将父图像的更新内容呈现到屏幕上
            parentBmp.WritePixels(new Int32Rect(x, y, childWidth, childHeight), parentPixels, parentStride, 0);
        }
    }
}
