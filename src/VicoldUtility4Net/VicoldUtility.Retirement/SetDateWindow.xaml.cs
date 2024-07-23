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

namespace VicoldUtility.Retirement
{
    /// <summary>
    /// SetDateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetDateWindow : Window
    {
        public DateTime? SelectedDate { get; private set; }

        public SetDateWindow()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (TargetDatePicker.SelectedDate.HasValue)
            {
                SelectedDate = TargetDatePicker.SelectedDate.Value.Date.AddYears(65);
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("请选择一个有效的日期。");
            }
        }
    }
}
