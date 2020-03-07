using System.Windows.Controls;
using System.Windows.Media;
using VicoldUtility.ResourceMonitor.Entities;
using VicoldUtility.ResourceMonitor.Properties;
using VicoldUtility.ResourceMonitor.Utilities;

namespace VicoldUtility.ResourceMonitor.Components
{
    /// <summary>
    /// MemoryPage.xaml 的交互逻辑
    /// </summary>
    public partial class MemoryMiniPage : Page
    {
        private float _inv;
        public MemoryMiniPage()
        {
            InitializeComponent();
            _inv = Settings.Default.InvalidValue;
        }
        public void ImportData(MemoryEtt ett)
        {
            Dispatcher.Invoke(() =>
            {
                RecLoad.Fill = ColorUtil.GetColor(ett.MemoryLoad / 10);
                TbLoad.Text = ett.MemoryLoad == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.MemoryLoad)}%";
                TbFree.Text = ett.MemoryFree == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.MemoryFree)}GB";
            });
        }
    }
}
