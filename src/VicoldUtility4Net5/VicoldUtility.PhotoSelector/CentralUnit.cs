using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;
using VicoldUtility.PhotoSelector.Entities;
using VicoldUtility.PhotoSelector.Project;

namespace VicoldUtility.PhotoSelector
{
    internal class CentralUnit
    {
        private Dictionary<string, Brush> _colors = new Dictionary<string, Brush>() {
            {".jpg",new SolidColorBrush( Colors.CadetBlue) },
            {".png",new SolidColorBrush( Colors.Firebrick) },
            {".bmp",new SolidColorBrush(Colors.DarkGreen) },
            {".cr3",new SolidColorBrush(Colors.Fuchsia) },
            {".raw",new SolidColorBrush(Colors.DarkSalmon) },
            {".dng",new SolidColorBrush(Colors.Orange) },
            {"other",new SolidColorBrush(Colors.Gray )},
        };

        public CentralUnit()
        {
            ProjectHandler = new ProjectHandler();
        }

        public ProjectHandler ProjectHandler { get; private set; }

        public MainWindow MainWindow { get; set; }

        public Dispatcher Dispatcher => MainWindow.Dispatcher;

        public void Preview(ImageItemEtt imageItemEtt)
        {
            MainWindow.Preview(imageItemEtt);
        }

        public Brush GetImageLabelColor(string label)
        {
            if (_colors.TryGetValue(label, out var color))
            {
                return color;
            }

            return _colors["other"];
        }
    }
}
