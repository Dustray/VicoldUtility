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
            var full = 10;
            var half = 4;
            val = val < 0 ? 0 : (val > full ? full : val);
            int r = 0, g = 0, b = 0;
            if (val < half)
            {
                r = (int)(255* val / half);
                g = 255;
            }
            else
            {
                r = 255;
                g = 255 - (int)(255*(val - half) /(full - half));
            }
            
            return new SolidColorBrush(Color.FromArgb((byte)200, (byte)r, (byte)g, (byte)b));
        }
    }
}
