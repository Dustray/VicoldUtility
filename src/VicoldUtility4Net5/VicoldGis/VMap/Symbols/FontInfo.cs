using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace VicoldGis.VMap.Symbols
{
    internal class FontInfo
    {
        public string Text { get; set; }
        public double FontSize { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double XOffset { get; set; }
        public double YOffset { get; set; }
        public Color FontColor { get; set; } = Colors.White;
    }
}
