using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace VicoldGis.VMap.Symbols
{
    internal class SingleLineInfo
    {
        public double LineWidth { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public Color LineColor { get; set; } = Colors.White;
    }
}
