using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VicoldGis.VMap;

namespace VicoldGis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MapBox Map2;
        protected override void OnStartup(StartupEventArgs e)
        {
            ModEngineHolder.INSTANCE = this;
        }


        private static class ModEngineHolder
        {
            internal static App INSTANCE;
        }
        internal new static App Current => ModEngineHolder.INSTANCE;

    }
}
