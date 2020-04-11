using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using VicoldUtility.FastTool.Entities;

namespace VicoldUtility.FastTool.Controls
{
    /// <summary>
    /// ButtonList.xaml 的交互逻辑
    /// </summary>
    public partial class ButtonList : UserControl
    {
        public ObservableCollection<ItemEtt> DataSource;
        public ButtonList()
        {
            InitializeComponent();
            DataSource = new ObservableCollection<ItemEtt>();
            aaa.ItemsSource = DataSource;
        }
        private void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (null == btn) return;
            var file = btn.Tag.ToString();
            if (!File.Exists(file)) return;
            var exep = new Process();
            exep.StartInfo.FileName = file;
            exep.EnableRaisingEvents = true;
            exep.Exited += new EventHandler((s, e2) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    btn.IsEnabled = true;
                });
            });
            btn.IsEnabled = false;
            exep.Start();
        }
    }
}
