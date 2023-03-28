using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VicoldUtility.MockLens.ChannelColor;

namespace VicoldUtility.MockLens.Utilities
{
    internal static class ChannelUtility
    {

        public static byte DataNumericalLegitimacy(int value)
        {
            return (byte)Math.Max(0, Math.Min(255, value));
        }
        
        public static byte DataNumericalLegitimacy(float value)
        {
            return (byte)Math.Max(0, Math.Min(255, value));
        }
        public static byte DataNumericalLegitimacy(double value)
        {
            return (byte)Math.Max(0, Math.Min(255, value));
        }

        /// <summary>
        /// 单通道是否在目标通道中
        /// </summary>
        /// <param name="chanelType">目标通道</param>
        /// <param name="singleType">单通道</param>
        /// <returns></returns>
        public static bool IsChannelMatched(ChannelType chanelType, ChannelType singleType)
        {
            return (chanelType & singleType) == singleType;
        }
    }
}
