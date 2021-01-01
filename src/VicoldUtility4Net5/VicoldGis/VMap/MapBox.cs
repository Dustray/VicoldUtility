using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VicoldGis.VMap.Handlers;
using VicoldGis.VMap.Projections;
using VicoldGis.VMap.Symbols;

namespace VicoldGis.VMap
{
    public class MapBox
    {
        public Action<List<FrameworkElement>> OnRender;
        public Action<FrameworkElement> OnRenderOne;
        private IProjection _projection;
        private double scale = 100;
        public MapBox()
        {
            _projection = new MercatorProj();
        }
        internal void LoadCoordinateGrid()
        {
            var list = new List<FrameworkElement>();
            var color = Colors.White;
            //经度
            for (var i = -70; i <= 290; i += 10)
            {
                var start = _projection.Project(i, 80);
                var end = _projection.Project(i, -80);
                var line = SymbolFactory.MakeLine(new SingleLineInfo()
                {
                    LineWidth = 1,
                    X1 = start.X * scale,
                    Y1 = start.Y * scale,
                    X2 = end.X * scale,
                    Y2 = end.Y * scale,
                    LineColor = color
                });
                list.Add(line);
                var font = SymbolFactory.MakeFont(new FontInfo()
                {
                    Text = i.ToString(),
                    FontSize = 13,
                    X = line.X1,
                    Y = line.Y1,
                    XOffset = -10,
                    YOffset = -20,
                    FontColor = color
                });
                var font2 = SymbolFactory.MakeFont(new FontInfo()
                {
                    Text = i.ToString(),
                    FontSize = 13,
                    X = line.X2,
                    Y = line.Y2,
                    XOffset = -10,
                    YOffset = 1,
                    FontColor = color
                });
                OnRenderOne?.Invoke(font);
                OnRenderOne?.Invoke(font2);
            }

            //纬度
            for (var i = 80; i >= -80; i -= 10)
            {
                var line = SymbolFactory.MakeLine(new SingleLineInfo()
                {
                    LineWidth = 1,
                    X1 = -180 * scale,
                    Y1 = -i * scale,
                    X2 = 180 * scale,
                    Y2 = -i * scale,
                    LineColor = color
                });
                list.Add(line);

                var font = SymbolFactory.MakeFont(new FontInfo()
                {
                    Text = i.ToString(),
                    FontSize = 13,
                    X = line.X1,
                    Y = line.Y1,
                    XOffset = -22,
                    YOffset = -7,
                    FontColor = color
                });
                var font2 = SymbolFactory.MakeFont(new FontInfo()
                {
                    Text = i.ToString(),
                    FontSize = 13,
                    X = line.X2,
                    Y = line.Y2,
                    XOffset = 1,
                    YOffset = -7,
                    FontColor = color
                });

                OnRenderOne?.Invoke(font);
                OnRenderOne?.Invoke(font2);
            }

            OnRender?.Invoke(list);
        }
    }
}
