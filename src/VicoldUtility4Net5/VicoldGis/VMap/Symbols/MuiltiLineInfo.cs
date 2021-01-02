using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace VicoldGis.VMap.Symbols
{
    public class MuiltiLineInfo
    {
        public double LineWidth { get; set; }
        public List<Point[]> Lines { get; set; }
        public Color LineColor { get; set; } = Colors.White;
    }
}
