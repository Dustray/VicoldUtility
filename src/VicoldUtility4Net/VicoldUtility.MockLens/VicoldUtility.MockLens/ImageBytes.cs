using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using VicoldUtility.MockLens.Algorithms;
using VicoldUtility.MockLens.WriteableBuffer;

namespace VicoldUtility.MockLens
{
    internal class ImageByteChannel
    {
        public ImageByteChannel(int length)
        {
            Data = new byte[length];
        }

        public byte[] Data { get; set; }

        public byte this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }
    }


    internal class ImageBytes
    {
        private ImageByteChannel _r { get; }
        private ImageByteChannel _g { get; }
        private ImageByteChannel _b { get; }
        private ImageByteChannel _a { get; }

        public ImageBytes(int width, int height)
        {
            Width = width;
            Height = height;
            var len = Width * Height;
            _r = new ImageByteChannel(len);
            _g = new ImageByteChannel(len);
            _b = new ImageByteChannel(len);
            _a = new ImageByteChannel(len);
        }

        public ImageBytes(Bitmap bitmap)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            var len = Width * Height;
            _r = new ImageByteChannel(len);
            _g = new ImageByteChannel(len);
            _b = new ImageByteChannel(len);
            _a = new ImageByteChannel(len);

            var data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            unsafe
            {
                var ptr = (byte*)data.Scan0;
                for (int i = 0; i < len; i++)
                {
                    _b[i] = ptr[0];
                    _g[i] = ptr[1];
                    _r[i] = ptr[2];
                    _a[i] = ptr[3];
                    ptr += 4;
                }
            }
            bitmap.UnlockBits(data);
        }

        public int Width { get; }

        public int Height { get; }

        public int Deep { get; } = 4;

        public byte this[int x, int y, int channel]
        {
            get
            {
                var index = y * Width + x;
                switch (channel)
                {
                    case 0:
                        return _r.Data[index];
                    case 1:
                        return _g.Data[index];
                    case 2:
                        return _b.Data[index];
                    case 3:
                        return _a.Data[index];
                    default:
                        throw new ArgumentOutOfRangeException(nameof(channel));
                }
            }
            set
            {
                var index = y * Width + x;
                switch (channel)
                {
                    case 0:
                        _r.Data[index] = value;
                        break;
                    case 1:
                        _g.Data[index] = value;
                        break;
                    case 2:
                        _b.Data[index] = value;
                        break;
                    case 3:
                        _a.Data[index] = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(channel));
                }
            }
        }

        public ImageRuntimeBuffer CreateSourceCopy()
        {
            byte[] copies = new byte[Width * Height * Deep];
            int i = 0;
            for (int x = 0; x < Width; x += 4)
            {
                for (int y = 0; y < Height; y += 4)
                {
                    int index = y * Width + x;
                    copies[i++] = _b.Data[index];//B
                    copies[i++] = _g.Data[index];//G
                    copies[i++] = _r.Data[index];//R
                    copies[i++] = _a.Data[index];//A
                }
            }

            return new ImageRuntimeBuffer(copies, Width, Height);
        }

        public ImageRuntimeBuffer CreateSourceZoomCopy(int width, int height)
        {
            byte[] copies = new byte[width * height * Deep];
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float zoomX = (float)Width * ((float)x / width);
                    float zoomY = (float)Height * ((float)y / height);

                    var b = BilinearInterpolationAlgorithm.Execute(_b.Data, Width, Height, zoomX, zoomY);
                    var g = BilinearInterpolationAlgorithm.Execute(_g.Data, Width, Height, zoomX, zoomY);
                    var r = BilinearInterpolationAlgorithm.Execute(_r.Data, Width, Height, zoomX, zoomY);
                    var a = BilinearInterpolationAlgorithm.Execute(_a.Data, Width, Height, zoomX, zoomY);

                    copies[i++] = b;//B
                    copies[i++] = g;//G
                    copies[i++] = r;//R
                    copies[i++] = a;//A
                }
            }

            return new ImageRuntimeBuffer(copies, width, height);
        }
    }
}
