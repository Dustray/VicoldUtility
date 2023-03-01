using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.MockLens
{
    internal class ImageRuntionBuffer
    {
        public ImageRuntionBuffer(byte[] data, int width, int height)
        {
            SourceData = data;
            Data = new byte[SourceData.Length];
            SourceData.CopyTo(Data, 0);

            Width = width;
            Height = height;
        }

        public byte[] SourceData { get; }

        public byte[] Data { get; }

        public int Width { get; }

        public int Height { get; }

        public void Add(int value)
        {
            //await Task.Run(() =>
            // {
            //     Parallel.For(0, 2, (channel) =>
            //     {
            //         for (var i = channel; i < SourceData.Length; i+=4)
            //         {
            //             Data[i] = (byte)Math.Max(0, Math.Min(255, SourceData[i] + value));
            //         }
            //     });
            // });
            for (var i = 0; i < SourceData.Length; i++)
            {
                if ((i + 1) % 4 == 0)
                {
                    Data[i] = 255;
                    continue;
                }

                Data[i] = (byte)Math.Max(0, Math.Min(255, SourceData[i] + value));
            }
        }
    }
}
