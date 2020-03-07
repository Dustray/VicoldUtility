using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
using Vicold.Popup;
using VicoldUtility.ToIconTool.utilities;

namespace VicoldUtility.ToIconTool
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
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

        private void Window_Drop(object sender, DragEventArgs e)
        {
            Array file = (Array)e.Data.GetData(DataFormats.FileDrop);

            var ff = new FileUtil();
            foreach (object I in file)
            {
                var filePath = I.ToString();
                var isPic = ff.IsImage(filePath);
                if (isPic)
                {
                    change(filePath);
                }
            }
        }

        private void change(string filePath)
        {
            var size = GetChooseSize();
            var newFilePath = Path.Combine(Path.GetDirectoryName(filePath), $"{Path.GetFileNameWithoutExtension(filePath)}.ico");

            var tool = new IconUtil();
            using (var bit = new Bitmap(filePath))
            {
                tool.SaveToIcon(bit, size, newFilePath);
                Alert.Show("转换完成", AlertTheme.Success);
            }
        }

        private System.Drawing.Size GetChooseSize()
        {
            if ((bool)Rbtn32.IsChecked)
            {
                return new System.Drawing.Size(32, 32);
            }
            else if ((bool)Rbtn64.IsChecked)
            {
                return new System.Drawing.Size(64, 64);
            }
            else if ((bool)Rbtn128.IsChecked)
            {
                return new System.Drawing.Size(128, 128);
            }
            else if ((bool)Rbtn256.IsChecked)
            {
                return new System.Drawing.Size(256, 256);
            }
            else if ((bool)Rbtn512.IsChecked)
            {
                return new System.Drawing.Size(512, 512);
            }
            else if ((bool)RbtnCus.IsChecked)
            {
                if (int.TryParse(TbCusX.Text.ToString(), out int x) && int.TryParse(TbCusY.Text.ToString(), out int y))
                {
                    if (x >= 1 && y >= 1 && x <= 3000 && y <= 3000)
                    {
                        return new System.Drawing.Size(x, y);
                    }
                }
            }
            Alert.Show("自定义尺寸格式不正确，将以64*64尺寸执行", AlertTheme.Warning);
            return new System.Drawing.Size(64, 64);
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Title = "请选择视频文件";
            if ((bool)ofd.ShowDialog())
            {
                foreach (var filePath in ofd.FileNames)
                {
                    var isPic = new FileUtil().IsImage(filePath);
                    if (isPic)
                    {
                        change(filePath);
                    }
                }
            }
        }
    }
}
