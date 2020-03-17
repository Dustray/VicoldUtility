using VicoldUtility.Image2ASCII.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace VicoldUtility.Image2ASCII
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isWindowNormal = true;
        public MainWindow()
        {
            InitializeComponent();
        }


        #region 窗体事件
        /// <summary>
        /// 窗体状态变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            var w = sender as Window;
            var state = w.WindowState;
            if (state == WindowState.Normal)
            {
                BtnMaxi.Content = ((char)0xEF2E).ToString();
                BtnMaxi.ToolTip = "最大化";
                _isWindowNormal = true;
            }
            else if (state == WindowState.Maximized)
            {
                BtnMaxi.Content = ((char)0xEF2F).ToString();
                BtnMaxi.ToolTip = "向下还原";
                _isWindowNormal = false;
            }
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, System.EventArgs e)
        {

        }

        #endregion

        #region 成员事件
        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// 最大化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMaxi_Click(object sender, RoutedEventArgs e)
        {
            if (_isWindowNormal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// 最小化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMini_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void tboxResult_PreviewDragOver(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = false;
            }
        }

        private void tboxResult_PreviewDrop(object sender, DragEventArgs e)
        {
            Array file = (Array)e.Data.GetData(DataFormats.FileDrop);
            var ff = new FileFormatUtil();
            foreach (object I in file)
            {
                var path = I.ToString();
                var isPic = ff.IsPicture(path);
                if (isPic)
                {
                    Change(path);
                    return;
                }
            }
            //不是图片
        }

        private void BtnSaveTo_Click(object sender, RoutedEventArgs e)
        {
            if ("" == tboxResult.Text.ToString())
            {
                //无有效字段
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "文本文件|*.txt",
                Title = "Save File"
            };
            if (sfd.ShowDialog() == true)
            {
                var path = sfd.FileName;
                //MessageBox.Show("保存成功");
                using (var sr = new StreamWriter(path, true))
                {
                    sr.Write(tboxResult.Text.ToString());
                }
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            //创建一个打开文件式的对话框
            OpenFileDialog ofd = new OpenFileDialog {
                InitialDirectory = @"D:\",
                Filter = "PNG图片|*.png|JPG图片|*.jpg"
            };
            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮
            if (ofd.ShowDialog() == true)
            {
                var path = ofd.FileName;
                Change( path);
            }
        }
        #endregion

        #region 成员方法

        private void Change(string path)
        {
            var util = new ImageToCode(OnProgressChange);
            string result = util.ToCodeFromBitmap(new System.Drawing.Bitmap(path));
            tboxResult.Text = result;
        }

        private void OnProgressChange(int progress)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                pgChange.Value = progress;
            }));
        }
        #endregion
    }
}
