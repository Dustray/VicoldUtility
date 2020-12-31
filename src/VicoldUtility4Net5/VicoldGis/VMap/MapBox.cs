using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VicoldGis.VMap.Handlers;
using VicoldGis.VMap.Projections;

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
                var line = new Line();
                line.RenderTransform = new TransformGroup();
                line.StrokeThickness = 1;
                line.Stroke = new SolidColorBrush(color);
                var start = _projection.Project(i, 80);
                var end = _projection.Project(i, -80);
                line.X1 = start.X * scale;
                line.Y1 = start.Y * scale;
                line.X2 = end.X * scale;
                line.Y2 = end.Y * scale;
                line.Tag = new AdaptiveAntiZoomHandler()
                {
                    OnScale = (ratio) =>
                    {
                        line.StrokeThickness = ratio * 1;
                    }
                };
                list.Add(line);

                var font = new TextBlock();
                font.Text = i.ToString();
                font.Foreground = new SolidColorBrush(color);
                font.FontSize = 13;
                font.Tag = new AdaptiveAntiZoomHandler()
                {
                    OnScale = (ratio) =>
                    {
                        font.FontSize = ratio * 13;
                        Canvas.SetLeft(font, line.X1 - (ratio * 10));
                        Canvas.SetTop(font, line.Y1 - (ratio * 20));
                    }
                };
                Canvas.SetLeft(font, line.X1);
                Canvas.SetTop(font, line.Y1);

                var font2 = new TextBlock();
                font2.Text = i.ToString();
                font2.Foreground = new SolidColorBrush(color);
                font2.FontSize = 13;
                font2.Tag = new AdaptiveAntiZoomHandler()
                {
                    OnScale = (ratio) =>
                    {
                        font2.FontSize = ratio * 13;
                        Canvas.SetLeft(font2, line.X2 - (ratio * 10));
                        Canvas.SetTop(font2, line.Y2 - (ratio * 1));
                    }
                };
                Canvas.SetLeft(font2, line.X2);
                Canvas.SetTop(font2, line.Y2);
                OnRenderOne?.Invoke(font);
                OnRenderOne?.Invoke(font2);
            }

            //纬度
            for (var i = 80; i >= -80; i -= 10)
            {
                var line = new Line();
                line.RenderTransform = new TransformGroup();
                line.StrokeThickness = 1;
                line.Stroke = new SolidColorBrush(color);
                line.X1 = -180 * scale;
                line.Y1 = -i * scale;
                line.X2 = 180 * scale;
                line.Y2 = -i * scale;
                line.Tag = new AdaptiveAntiZoomHandler()
                {
                    OnScale = (ratio) =>
                    {
                        line.StrokeThickness = ratio * 1;
                    }
                };
                list.Add(line);

                var font = new TextBlock();
                font.Text = i.ToString();
                font.Foreground = new SolidColorBrush(color);
                font.FontSize = 13;
                font.Tag = new AdaptiveAntiZoomHandler()
                {
                    OnScale = (ratio) =>
                    {
                        font.FontSize = ratio * 13;
                        Canvas.SetLeft(font, line.X1 - (ratio * 22));
                        Canvas.SetTop(font, line.Y1 - (ratio * 7));
                    }
                };
                Canvas.SetLeft(font, line.X1);
                Canvas.SetTop(font, line.Y1);

                var font2 = new TextBlock();
                font2.Text = i.ToString();
                font2.Foreground = new SolidColorBrush(color);
                font2.FontSize = 13;
                font2.Tag = new AdaptiveAntiZoomHandler()
                {
                    OnScale = (ratio) =>
                    {
                        font2.FontSize = ratio * 13;
                        Canvas.SetLeft(font2, line.X2 + (ratio * 1));
                        Canvas.SetTop(font2, line.Y2 - (ratio * 7));
                    }
                };
                Canvas.SetLeft(font, line.X1);
                Canvas.SetTop(font, line.Y1);
                OnRenderOne?.Invoke(font);
                OnRenderOne?.Invoke(font2);
            }

            OnRender?.Invoke(list);
        }
    }
}
