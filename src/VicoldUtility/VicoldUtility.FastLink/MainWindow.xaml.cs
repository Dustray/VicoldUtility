using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Vicold.Popup;
using VicoldUtility.FastLink.Properties;
using VicoldUtility.FastLink.Views;

namespace VicoldUtility.FastLink
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ToolListPage toolListPage;
        private bool _isOpeningChildFolder = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        #region  窗体事件

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            SetWindowLong(wndHelper.Handle, (-20), 0x80);


            toolListPage = new ToolListPage((isOpeningChildFolder) =>
            {
                _isOpeningChildFolder = isOpeningChildFolder;
            });
            ToolsBtnFrame.Navigate(toolListPage);
            toolListPage.OnWindowShow();
            try
            {
                //设置位置、大小
                Rect restoreBounds = Settings.Default.MainWindowPosition;
                Left = restoreBounds.Left;
                Top = restoreBounds.Top < 0 ? 0 : restoreBounds.Top;
            }
            catch { }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.MainWindowPosition = RestoreBounds;
            Settings.Default.Save();
            toolListPage.OnWindowClose();
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            if (RestoreBounds.Top < 0)
            {
                toolListPage.OnWindowShow();

                ShowOrHide(true);
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (RestoreBounds.Top == 0)
            {
                if (!_isOpeningChildFolder)
                {
                    toolListPage.OnWindowHide();
                    ShowOrHide(false);
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var configPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"Data\LinkSource.xml");
            Process.Start(configPath);
            Alert.Show("是的，添加就是让你修改配置文件", "修改配置文件后重启应用。", AlertTheme.Default, new AlertConfig() { AlertShowDuration = 7000 });
        }

        private void ShowOrHide(bool isShow)
        {
            this.UpdateLayout();
            var height = RestoreBounds.Height;

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(150));//设置动画的持续时间
            animation.From = isShow ? -height + 6 : 0;
            animation.To = isShow ? 0 : -height + 6;
            this.BeginAnimation(TopProperty, animation);//设定动画应用于窗体的Left属性
        }
        #region Window styles

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 SetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        #endregion

    }
}
