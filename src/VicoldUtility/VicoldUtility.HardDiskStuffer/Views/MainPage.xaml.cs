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
using VicoldUtility.HardDiskStuffer.Entities;
using VicoldUtility.HardDiskStuffer.Utility;

namespace VicoldUtility.HardDiskStuffer.Views
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        private FileStufferUtil _fileStufferUtil;
        private List<DriveEtt> _driveEttList;
        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _driveEttList = DriveUtil.GetDriveList();
            lviewDrive.ItemsSource = DriveUtil.Ett2EttShow(_driveEttList);
        }

        private void btnStartOrStop_Click(object sender, RoutedEventArgs e)
        {
            var a = lviewDrive.ItemsSource as List<DriveShowEtt>;
            if (null == _fileStufferUtil)
                _fileStufferUtil = new FileStufferUtil(a);
            _fileStufferUtil.StartOrStop();
        }
    }
}
