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
using VicoldUtility.FastLink.Views;

namespace VicoldUtility.FastLink
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region  窗体事件

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var toolListPage = new ToolListPage();
            ToolsBtnFrame.Navigate(toolListPage);
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            if (RestoreBounds.Top < 0)
            {
                var self = sender as MainWindow;
                if (self != null)
                {
                    self.UpdateLayout();
                    var height = RestoreBounds.Height;

                    DoubleAnimation animation = new DoubleAnimation();
                    animation.Duration = new Duration(TimeSpan.FromMilliseconds(150));//设置动画的持续时间
                    animation.From =  -height + 4;
                    animation.To = 0;
                    self.BeginAnimation(TopProperty, animation);//设定动画应用于窗体的Left属性
                }
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (RestoreBounds.Top == 0)
            {
                var self = sender as MainWindow;
                if (self != null)
                {
                    self.UpdateLayout();
                    var height = RestoreBounds.Height;

                    DoubleAnimation animation = new DoubleAnimation();
                    animation.Duration = new Duration(TimeSpan.FromMilliseconds(150));//设置动画的持续时间
                    animation.From = 0;
                    animation.To = -height + 4;
                    self.BeginAnimation(TopProperty, animation);//设定动画应用于窗体的Left属性
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
        }

        #endregion

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
