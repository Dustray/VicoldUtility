using System;
using System.Collections.Generic;
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
using VicoldUtility.FastLink.Entities;

namespace VicoldUtility.FastLink.Views
{
    /// <summary>
    /// ConfigListPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigListPage : Page
    {
        private Action<string> _onOpeningCallback;
        public ConfigListPage(Action<string> onOpeningCallback)
        {
            InitializeComponent();
            _onOpeningCallback = onOpeningCallback;
        }


        private void lbConfigList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var thisBorder = sender as Border;
            var ett = thisBorder.DataContext as ConfigListDataEtt;
            _onOpeningCallback.Invoke(ett.Tint);
        }

        internal void LoadData(List<ConfigListDataEtt> dataList)
        {
            lbConfigList.ItemsSource = dataList;
        }
    }
}
