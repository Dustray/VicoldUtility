using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using VicoldGis.VMap.Projections;
using VicoldGis.VMap.Symbols;

namespace VicoldGis.VMap
{
    public class MapBox
    {
        private IProjection _projection;
        public MapBox()
        {
            _projection = new MercatorProj();
            Manager = new MapManager();
            Manager.OnShowVisualCallback = (eles) => OnRenderVisual.Invoke(eles);
            Manager.OnShowCallback = (eles) => OnRender.Invoke(eles);
            Manager.OnDeleteCallback = (eles) => OnUnRender.Invoke(eles);
        }
        public double Scale { get; set; } = 100;

        public MapManager Manager { get; private set; }

        internal Action<ICollection<FrameworkElement>> OnRender { get; set; }
        internal Action<ICollection<Visual>> OnRenderVisual { get; set; }

        internal Action<FrameworkElement> OnRenderOne { get; set; }

        internal Action<ICollection<FrameworkElement>> OnUnRender { get; set; }

        /// <summary>
        /// 加载经纬线和标签
        /// </summary>
        internal void LoadCoordinateGrid()
        {
            var list = new List<FrameworkElement>();
            var color = Colors.Gray;
            //经度
            for (var i = -70; i <= 290; i += 10)
            {
                var start = _projection.Project(i, 80);
                var end = _projection.Project(i, -80);
                var line = SymbolFactory.MakeLine(new SingleLineInfo()
                {
                    LineWidth = 1,
                    X1 = start.X * Scale,
                    Y1 = start.Y * Scale,
                    X2 = end.X * Scale,
                    Y2 = end.Y * Scale,
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
                    X1 = -180 * Scale,
                    Y1 = -i * Scale,
                    X2 = 180 * Scale,
                    Y2 = -i * Scale,
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
