using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VicoldGis.VMap.Handlers;

namespace VicoldGis.VMap.Symbols
{
    internal static class SymbolFactory
    {
        public static TextBlock MakeFont(FontInfo info)
        {
            var font = new TextBlock();
            font.Text = info.Text;
            font.Foreground = new SolidColorBrush(info.FontColor);
            font.FontSize = info.FontSize;
            font.Tag = new AdaptiveAntiZoomHandler()
            {
                OnScale = (ratio) =>
                {
                    font.FontSize = ratio * info.FontSize;
                    Canvas.SetLeft(font, info.X + (ratio * info.XOffset));
                    Canvas.SetTop(font, info.Y + (ratio * info.YOffset));
                }
            };
            Canvas.SetLeft(font, info.X + info.XOffset);
            Canvas.SetTop(font, info.Y + info.YOffset);
            return font;
        }

        public static Line MakeLine(SingleLineInfo info)
        {
            var line = new Line();
            line.RenderTransform = new TransformGroup();
            line.StrokeThickness = info.LineWidth;
            line.Stroke = new SolidColorBrush(info.LineColor);
            line.X1 =info.X1;
            line.Y1 =info.Y1;
            line.X2 =info.X2;
            line.Y2 = info.Y2;
            line.Tag = new AdaptiveAntiZoomHandler()
            {
                OnScale = (ratio) =>
                {
                    line.StrokeThickness = ratio * info.LineWidth;
                }
            };
            return line;
        }
    }
}
