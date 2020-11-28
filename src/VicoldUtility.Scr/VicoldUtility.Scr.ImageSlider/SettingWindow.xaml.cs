using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VicoldUtility.Scr.ImageSlider
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            var speed = Properties.Settings.Default.ExchangeSpeed;
            sldSpeed.Value = speed;
            tbSpeed.Text = $"{speed}秒";
            cbRandom.IsChecked = Properties.Settings.Default.IsRandom;
            tbFolderPath.Text = Properties.Settings.Default.FolderPath;
        }

        private void btnSpeed_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var speed = int.Parse(btn.Tag.ToString());
            sldSpeed.Value = speed;
        }

        private void sldSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (tbSpeed != null)
            {
                var speed = (int)sldSpeed.Value;
                Properties.Settings.Default.ExchangeSpeed = speed;
                Properties.Settings.Default.Save();
                tbSpeed.Text = $"{speed}秒";
            }
        }

        private void cbRandom_Checked(object sender, RoutedEventArgs e)
        {
            bool isCheck = (bool)cbRandom.IsChecked;
            Properties.Settings.Default.IsRandom = isCheck;
            Properties.Settings.Default.Save();
        }

        private void btnChoosePath_Click(object sender, RoutedEventArgs e)
        {
            var dia = new System.Windows.Forms.FolderBrowserDialog();
            if (dia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.FolderPath = dia.SelectedPath;
                Properties.Settings.Default.Save();
                tbFolderPath.Text = dia.SelectedPath;
            }
        }
    }
}
