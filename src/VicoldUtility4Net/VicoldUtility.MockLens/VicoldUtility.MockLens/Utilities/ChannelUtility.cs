using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
