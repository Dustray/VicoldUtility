using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VicoldUtility.AvatarMaker.Pages;
using VicoldUtility.AvatarMaker.ViewModels;

namespace VicoldUtility.AvatarMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = new MainWindowViewModel();
            vm.ToolbarViewModel = new ToolbarViewModel();
            vm.StatusbarViewModel = new StatusbarViewModel();
            vm.Pages.Add(new PreviewPage());
            vm.Pages.Add(new AdjustPage());
            vm.CurrentPage = vm.Pages[0];
            DataContext = vm;
        }
    }
}
