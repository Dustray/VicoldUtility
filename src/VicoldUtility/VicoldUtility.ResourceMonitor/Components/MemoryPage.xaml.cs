using System.Windows.Controls;
using VicoldUtility.ResourceMonitor.Entities;

namespace VicoldUtility.ResourceMonitor.Components
{
    /// <summary>
    /// MemoryPage.xaml 的交互逻辑
    /// </summary>
    public partial class MemoryPage : Page
    {
        public MemoryPage()
        {
            InitializeComponent();
        }
        public void ImportData(MemoryEtt ett)
        {
            Dispatcher.Invoke(() =>
            {

            });
        }
    }
}
