using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.MockLens.WriteableBuffer
{
    internal class ImageRuntimeBuffer
    {
        public ImageRuntimeBuffer(byte[] data, int width, int height)
        {
            SourceData = data;
            Data = new byte[SourceData.Length];
            SourceData.CopyTo(Data, 0);

            Width = width;
            Height = height;
        }

        public readonly byte[] SourceData;

        public byte[] Data { get; }

        public int Width { get; }

        public int Height { get; }
    }
}
