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
using System.Windows.Shapes;

namespace FileHeadFormatter
{
    /// <summary>
    /// ConfirmWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmWindow : Window
    {

        public ConfirmWindow(string template)
        {
            InitializeComponent();
            ConfirmText.Text = template;
        }
        
        public string TemplateText => ConfirmText.Text;

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
