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
        private Action<string> _logAction;
        public ButtonList(Action<string> logAction)
        {
            InitializeComponent();
            _logAction = logAction;
            DataSource = new ObservableCollection<ItemEtt>();
            lboxMain.ItemsSource = DataSource;
        }
        private void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (null == btn) return;
            var curItem = ((ListBoxItem)lboxMain.ContainerFromElement(btn))?.Content;
            if (null == curItem) return;
            var ett = curItem as ItemEtt;

            var file = ett.FilePath;
            if (!File.Exists(file)) return;
            var exep = new Process();
            exep.StartInfo.FileName = file;
            if (ett.IsNeedAdmin)
                exep.StartInfo.Verb = "RunAs";
            exep.EnableRaisingEvents = true;
            exep.Exited += new EventHandler((s, e2) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    _logAction?.Invoke($"[{ett.Content}]执行完毕");
                    btn.IsEnabled = true;
                });
            });
            btn.IsEnabled = false;
            try
            {
                _logAction?.Invoke($"[{ett.Content}]开始执行{(ett.IsNeedAdmin?"（管理员）":"")}");
                exep.Start();
            }
            catch
            {
                _logAction?.Invoke($"[{ett.Content}]执行失败，原因：取消了管理员执行");
                btn.IsEnabled = true;
            }


        }

    }
}
