using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Vicold.Popup;
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
        private bool _flushFlag = false;

        #region 页面事件

        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _driveEttList = DriveUtil.GetDriveList();
            lviewDrive.ItemsSource = DriveUtil.Ett2EttShow(_driveEttList);
        }

        #endregion

        #region 成员方法

        private void btnStartOrStop_Click(object sender, RoutedEventArgs e)
        {
            var showCollection = lviewDrive.ItemsSource as ObservableCollection<DriveShowEtt>;
            if (showCollection.Where(v => v.IsChosen == true).Count() == 0)
            {
                Alert.Show("警告", "海内存知己，不能不选驱动器", AlertTheme.Warning);
                return;
            }
            var driveList = Enumerable.ToList(showCollection);
            if (null == _fileStufferUtil)
            {
                _fileStufferUtil = new FileStufferUtil(driveList);
                _fileStufferUtil.OnStarted = OnExchangeStarted;
                _fileStufferUtil.OnStopped = OnExchangeStopped;
                _fileStufferUtil.OnDriveStart = OnDriveStart;
                _fileStufferUtil.OnDriveComplete = OnDriveComplete;
                _fileStufferUtil.OnDeleteStufferFile = OnDeleteStufferFile;
                _fileStufferUtil.IsDeleteAfterStuffer = (bool)cboxDeleteFile.IsChecked;
            }
            _fileStufferUtil?.StartOrStop();
        }

        private void btnClearStufferFile_Click(object sender, RoutedEventArgs e)
        {
            var showCollection = lviewDrive.ItemsSource as ObservableCollection<DriveShowEtt>;  
            if (showCollection.Where(v => v.IsChosen == true).Count() == 0)
            {
                Alert.Show("警告", "海内存知己，不能不选驱动器", AlertTheme.Warning);
                return;
            }
            var driveList = Enumerable.ToList(showCollection);
            var util = new FileStufferUtil(driveList);
            util.OnDeleteStufferFile = OnDeleteStufferFile;
            new Task(() =>
            {
                util.DoDeleteInDrive();
            }).Start();
        }

        private void cboxCkeckAll_Click(object sender, RoutedEventArgs e)
        {
            var showCollection = lviewDrive.ItemsSource as ObservableCollection<DriveShowEtt>;
            var choose = showCollection.Where(v => v.IsChosen == false).Count() != 0;
            cboxCkeckAll.Content = choose ? "全不选" : "全选";
            foreach (var ett in showCollection)
            {
                ett.IsChosen = choose;
            }
        }

        #endregion

        #region 填充回调方法

        public void OnExchangeStarted()
        {
            ReloadDriveInfo();
            this.Dispatcher.Invoke(() =>
            {
                StartStopUIReflush(false);
            });
        }

        public void OnExchangeStopped()
        {
            _flushFlag = false;
            _fileStufferUtil = null;
            this.Dispatcher.Invoke(() =>
            {
                StartStopUIReflush(true);
            });
        }
        public void OnDriveStart(string driveNum)
        {
            this.Dispatcher.Invoke(() =>
            {
                tbStuffering.Text = $"正在填充：[{driveNum}]...";
                WriteLog($"[{driveNum}]盘开始填充...");
            });
        }

        public void OnDriveComplete(string driveNum)
        {
            this.Dispatcher.Invoke(() =>
            {
                Alert.Show($"[{driveNum}]已填充完成", AlertTheme.Success);
                WriteLog($"[{driveNum}]盘填充数据成功", true);
                tbStuffering.Text = $"[{driveNum}]填充完成";
            });
        }

        public void OnDeleteStufferFile(string driveNum, bool isSuccess)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (isSuccess)
                {
                    Alert.Show($"[{driveNum}]删除成功", AlertTheme.Success);
                    WriteLog($"[{driveNum}]盘清理填充数据成功", true);
                }
                else
                {
                    Alert.Show($"[{driveNum}]删除失败（或部分失败），请手动删除", AlertTheme.Error);
                    WriteLog($"[{driveNum}]盘清理填充数据失败（或部分失败），请手动删除", false);
                }
                var newList = DriveUtil.GetDriveList();
                var newObList = DriveUtil.Ett2EttShow(newList);
                lviewDrive.ItemsSource = newObList;
            });
        }

        #endregion

        #region 页面数据刷新

        public void ReloadDriveInfo()
        {
            _flushFlag = true;
            new Task(async () =>
            {

                while (_flushFlag)
                {
                    var showCollection = lviewDrive.ItemsSource as ObservableCollection<DriveShowEtt>;
                    var oldList = _driveEttList;
                    var newList = DriveUtil.GetDriveList();
                    var isDriveListChange = DriveUtil.CompareNewDriveList(newList, oldList);
                    var newObList = DriveUtil.Ett2EttShow(newList);
                    if (isDriveListChange)//驱动器发生改变
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            lviewDrive.ItemsSource = newObList;
                        });
                    }
                    else
                    {
                        foreach (var ett in newObList)
                        {
                            var listEtt = showCollection.Where(v => v.Number == ett.Number).First();
                            listEtt.FreeSize = ett.FreeSize;
                            listEtt.StufferSpeed = ett.StufferSpeed;
                        }
                    }
                    _driveEttList = newList;
                    await Task.Delay(500);
                }
            }).Start();
        }

        public void StartStopUIReflush(bool flag)
        {

            btnStartOrStop.Content = flag ? "开始" : "停止";
            lviewDrive.IsEnabled = flag;
            cboxDeleteFile.IsEnabled = flag;
            btnClearStufferFile.IsEnabled = flag;
        }

        public void WriteLog(string msg, bool? isSuccess = null)
        {
            if (null == isSuccess)
            {
                tboxLog.AppendText($"{msg}\r\n");
            }
            else
            {
                tboxLog.AppendText($"[{((bool)isSuccess ? "成功" : "失败")}] {msg}\r\n");
            }
            tboxLog.ScrollToEnd();
        }
        #endregion

    }
}
