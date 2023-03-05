using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VicoldUtility.MockLens.WriteableBuffer;

namespace VicoldUtility.MockLens
{
    
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static new App Current { get => (Application.Current as App) ?? throw new Exception("ds"); }

        internal OperatorManager OperatorManager = new OperatorManager();
    }
}
