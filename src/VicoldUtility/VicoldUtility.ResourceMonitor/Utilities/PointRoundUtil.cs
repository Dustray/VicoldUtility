

using System;

namespace VicoldUtility.ResourceMonitor.Utilities
{
    public class PointRoundUtil
    {
        public static string ToVision0Point(double p)
        {
            var value = Math.Round(p, 0, MidpointRounding.AwayFromZero);
            return value.ToString();
        }
        public static string ToVision1Point(double p)
        {
            var value = Math.Round(p, 0, MidpointRounding.AwayFromZero);
            return value.ToString();
        }
        public static string ToVision2Point(double p)
        {
            var value = Math.Round(p, 2, MidpointRounding.AwayFromZero);
            return value.ToString();
        }
    } 
}
