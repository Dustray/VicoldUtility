using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using VicoldUtility.PhotoSelector.Entities;
using VicoldUtility.PhotoSelector.Project;

namespace VicoldUtility.PhotoSelector
{
    internal class CentralUnit
    {
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

    }
}
