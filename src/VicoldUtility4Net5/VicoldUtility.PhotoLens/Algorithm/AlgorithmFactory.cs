using System;
using System.Collections.Generic;
using System.Text;
using VicoldUtility.PhotoLens.Algorithm.Isoline;

namespace VicoldUtility.PhotoLens.Algorithm
{
    internal class AlgorithmFactory
    {
        /// <summary>
        /// 等值线分析
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static IsolineBuilder CreateIsolineLoader(float[] data, int startX, int startY, int width, int height)
        {
            return new IsolineBuilder(data, startX, startY, width, height);
        }

    }
}
