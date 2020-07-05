using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Vicold.Popup;
using VicoldUtility.FastLink.Entities;
using VicoldUtility.FastLink.Properties;
using VicoldUtility.FastLink.Utilities;
using VicoldUtility.FastLink.Views;

namespace VicoldUtility.FastLink
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 内嵌List页面
        /// </summary>
        private ToolListPage toolListPage;
        private ConfigListPage configListPage;
        /// <summary>
        /// 是否正在打开子菜单
        /// </summary>
        private bool _isOpeningChildFolder = false;

        private bool _isActivate = false;

        //设置托盘图标
        private TaskBarUtil taskBarUtil;
        private string _currentConfigFilePath;
        public MainWindow()
        {
            InitializeComponent();
            taskBarUtil = new TaskBarUtil(this);
        }

        #region  窗体事件

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            SetWindowLong(wndHelper.Handle, (-20), 0x80);
            ChooseCinfigFile();

            toolListPage = new ToolListPage((isOpeningChildFolder) =>
            {
                _isOpeningChildFolder = isOpeningChildFolder;
            }, _currentConfigFilePath);
            ToolsBtnFrame.Navigate(toolListPage);
            toolListPage.OnWindowShow();

            configListPage = new ConfigListPage((url) =>
            {
                _currentConfigFilePath = url;
                Settings.Default.CurrentFilePath = url;
                toolListPage.OnWindowClose();
                toolListPage = new ToolListPage((isOpeningChildFolder) =>
                {
                    _isOpeningChildFolder = isOpeningChildFolder;
                }, url);
                ToolsBtnFrame.Navigate(toolListPage);
                toolListPage.OnWindowShow();
                ShowConfigList(null);
            });
            try
            {
                //设置位置、大小
                Rect restoreBounds = Settings.Default.MainWindowPosition;
                Left = restoreBounds.Left;
                Top = restoreBounds.Top < 0 ? 0 : restoreBounds.Top;
            }
            catch { }
        }
        /// <summary>
        /// 窗体关闭完毕事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            taskBarUtil.Dispose();
        }
        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.MainWindowPosition = RestoreBounds;
            Settings.Default.Save();
            toolListPage.OnWindowClose();
        }

        /// <summary>
        /// 鼠标进入窗体事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            if (RestoreBounds.Top < 0)
            {
                ShowConfigList(null);
                toolListPage.OnWindowShow();
                ShowOrHide(true);
            }
            _isOpeningChildFolder = false;// 复位：【防止关闭列表的时候鼠标移出导致窗体隐藏
        }

        /// <summary>
        /// 鼠标离开窗体事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// 左键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion

        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditConfig();
        }

        /// <summary>
        /// 选择配置文件
        /// </summary>
        private void ChooseCinfigFile()
        {
            var configFolder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"Data\");
            var files = Directory.GetFiles(configFolder);
            if (0 == files.Length)
            {
                Alert.Show("没有找到任何配置文件，请添加。", AlertTheme.Error);
                return;
            }
            var settingPath = Settings.Default.CurrentFilePath;
            if (string.IsNullOrWhiteSpace(settingPath))
            {
                _currentConfigFilePath = files[0];
                Settings.Default.CurrentFilePath = files[0];
            }
            else
            {
                if (File.Exists(settingPath))
                {
                    _currentConfigFilePath = settingPath;
                }
                else
                {
                    _currentConfigFilePath = files[0];
                    Settings.Default.CurrentFilePath = files[0];
                }
            }
        }

        /// <summary>
        /// 编辑配置文件
        /// </summary>
        public void EditConfig()
        {
            if (string.IsNullOrWhiteSpace(_currentConfigFilePath))
            {
                Alert.Show("没有找到任何配置文件，请添加。", AlertTheme.Error);
            }
            else
            {
                Process.Start(_currentConfigFilePath);
                Alert.Show("是的，添加就是让你修改配置文件", "修改配置文件后重启应用。", AlertTheme.Default, new AlertConfig() { AlertShowDuration = 7000 });
            }
        }

        /// <summary>
        /// 显示或隐藏窗体
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowOrHide(bool isShow)
        {
            this.UpdateLayout();
            var height = Height;

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(150));//设置动画的持续时间
            animation.From = isShow ? -height + 6 : 0;
            animation.To = isShow ? 0 : -height + 6;
            this.BeginAnimation(TopProperty, animation);//设定动画应用于窗体的Left属性
        }

        /// <summary>
        /// 显示或隐藏配置列表
        /// </summary>
        /// <param name="isShow"></param>
        private void ShowConfigList(ConfigListPage page)
        {
            if (page == null)
            {
                _isActivate = false;
                ConfigListFrame.Navigate(null);
                ChangeConfigBtn.Content = ((char)0xEC12).ToString();
                _isOpeningChildFolder = true;//防止关闭列表的时候鼠标移出导致窗体隐藏
            }
            else
            {
                _isActivate = true;
                ConfigListFrame.Navigate(page);
                ChangeConfigBtn.Content = ((char)0xEC11).ToString();
            }
        }

        #region Window styles

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 SetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        #endregion


        /// <summary>
        /// 切换配置文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isActivate)
            {
                var configFolder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"Data\");
                var files = Directory.GetFiles(configFolder);
                if (0 == files.Length)
                {
                    Alert.Show("没有找到任何配置文件，请添加。", AlertTheme.Error);
                    return;
                }
                var dataList = new List<ConfigListDataEtt>();
                var currentConfigFileName = Path.GetFileName(_currentConfigFilePath);
                foreach (var url in files)
                {
                    var fileName = Path.GetFileName(url);
                    dataList.Add(new ConfigListDataEtt()
                    {
                        Display = fileName,
                        Tint = url,
                        IsCurrent = currentConfigFileName != fileName,
                    });
                }
                configListPage.LoadData(dataList);
                ShowConfigList(configListPage);
            }
            else
            {
                ShowConfigList(null);
            }
        }
    }
}
