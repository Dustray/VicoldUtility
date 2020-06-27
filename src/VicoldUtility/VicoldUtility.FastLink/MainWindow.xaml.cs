using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        #region  窗体事件

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            toolListPage = new ToolListPage();
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
                toolListPage.OnWindowClose();
                ShowOrHide(false);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowOrHide(bool isShow)
        {
            this.UpdateLayout();
            var height = RestoreBounds.Height;

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(150));//设置动画的持续时间
            animation.From = isShow ? -height + 4 : 0;
            animation.To = isShow ? 0 : -height + 4;
            this.BeginAnimation(TopProperty, animation);//设定动画应用于窗体的Left属性
        }
    }
}
