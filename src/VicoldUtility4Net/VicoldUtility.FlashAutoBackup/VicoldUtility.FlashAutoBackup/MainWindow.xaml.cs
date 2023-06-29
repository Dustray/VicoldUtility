using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VicoldUtility.FlashAutoBackup.FlashShriek;
using VicoldUtility.FlashAutoBackup.SystemTools;

namespace VicoldUtility.FlashAutoBackup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DiskListener _diskListener = new();

        public MainWindow()
        {
            InitializeComponent();

            _diskListener.FlashDiskEvent += FlashDiskShow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*WPF中处理消息首先要获取窗口句柄，创建HwndSource对象 通过HwndSource对象添加
             * 消息处理回调函数.HwndSource类: 实现其自己的窗口过程。 创建窗口之后使用 AddHook 
             * 和 RemoveHook 来添加和移除挂钩，接收所有窗口消息。*/
            HwndSource? hwndSource = PresentationSource.FromVisual(this) as HwndSource;//窗口过程
            if (hwndSource is { })
            {
                hwndSource.AddHook(new HwndSourceHook(WndProc));//挂钩
            }
        }

        private void FlashDiskShow(string[] ReturnValue)
        {
            //lblUDick.Visibility = Visibility.Hidden;
            //lblUDick.Content = "";
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
                var aa = _diskListener.GetRemovableDrivers(msg, wParam);
                if (aa == null || aa.Length < 1)
                {
                    return IntPtr.Zero;
                }
                var ds = string.Empty;
                for (int i = 0; i < aa.Length; i++)
                {
                    ds += aa[i] + " ";//aa[i] + Environment.NewLine;
                }
                NotificationTool.ShowNotification("FlashAutoBackup", "U盘(" + ds + ")" + "已连接");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return IntPtr.Zero;
        }

    }
}
