using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VicoldUtility.PhotoSelector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal CentralUnit SZM { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            ModEngineHolder.INSTANCE = this;
            SZM = new CentralUnit();
        }
        private static class ModEngineHolder
        {
            internal static App INSTANCE;
        }
        internal new static App Current => ModEngineHolder.INSTANCE;
    }
}
