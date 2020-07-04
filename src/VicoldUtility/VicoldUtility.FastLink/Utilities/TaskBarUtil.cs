using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace VicoldUtility.FastLink.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarUtil : IDisposable
    {
        private MainWindow _mainWindow;
        //创建NotifyIcon对象 
        private NotifyIcon _notifyicon = new NotifyIcon();
        //创建托盘菜单对象 
        private ContextMenuStrip _notifyContextMenu = new ContextMenuStrip();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainWindow"></param>
        public TaskBarUtil(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            var debug = "";
#if DEBUG
            debug = "（Debug）";
#endif
            _notifyicon.Text = $"快速链接{debug}";
            Icon ico = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            _notifyicon.Icon = ico;
            _notifyicon.Visible = true;
            _notifyicon.MouseClick += notifyIcon_MouseClick;
            LoadMenu();
        }

        private void LoadMenu()
        {
            var launcherBtn = _notifyContextMenu.Items.Add("开机自启");
            launcherBtn.Click += new EventHandler(AutoLauncher);

            var settingBtn = _notifyContextMenu.Items.Add("编辑目录配置");
            settingBtn.Click += new EventHandler(EditConfig);

            var closeBtn = _notifyContextMenu.Items.Add("退出");
            closeBtn.Click += new EventHandler(Shutdown);
            _notifyicon.ContextMenuStrip = _notifyContextMenu;
        }

        #region 托盘图标事件

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mainWindow.ShowOrHide(true);
                _mainWindow.Activate();
            }
        }

        private void Shutdown(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void AutoLauncher(object sender, EventArgs e)
        {
            DesktopLinkUtil.CreateLauncherLink();
        }

        private void EditConfig(object sender, EventArgs e)
        {
            _mainWindow.EditConfig();
        }

        #endregion

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            _notifyContextMenu?.Dispose();
            _notifyicon?.Dispose();
        }
    }
}
