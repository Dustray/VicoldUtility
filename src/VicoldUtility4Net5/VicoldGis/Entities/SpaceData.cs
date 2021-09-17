using System;
using System.Collections.Generic;
using System.Text;

namespace VicoldGis.Entities
{
    public class SpaceData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float[] Data { get; set; }

        public float[] CreateAnaValue(int count = 15)
        {
            var min = float.MaxValue;
            var max = float.MinValue;
            var len = Data.Length;
            for (var i = 0; i < len; i++)
            {
                var v = Data[i];
                if (v < min) min = v;
                if (v > max) max = v;
            }

            var result = new float[count];
            int step = (int)(max-min)/ count;
            for(var i = 0; i < count; i++)
            {
                result[i] = min + step * i;
            }

            return result;
        }
    }
}
