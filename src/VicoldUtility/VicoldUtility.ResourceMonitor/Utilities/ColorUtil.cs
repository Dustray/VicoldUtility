using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VicoldUtility.ResourceMonitor.Utilities
{
    public class ColorUtil
    {
        /// <summary>
        /// 获取红黄绿颜色
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Brush GetColor(float val)
        {
            float one = (255 + 255) / 6;
            int r = 0, g = 0, b = 0;
            if (val < 3)
            {
                r = (int)(one * val);
                g = 255;
            }
            else if (val >= 3 && val < 6)
            {
                r = 255;
                g = 255 - (int)((val - 3) * one);
            }
            else { r = 255; }
            return new SolidColorBrush(Color.FromArgb((byte)200, (byte)r, (byte)g, (byte)b));
        }
    }
}
