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
    public partial class MemoryPage : Page
    {
        private float _inv;
        private bool _loadedCoreGrid = false;
        public MemoryPage()
        {
            InitializeComponent();
            _inv = Settings.Default.InvalidValue;
            TbTitle.Foreground = new SolidColorBrush(Settings.Default.TitleColor);
        }
        public void ImportData(MemoryEtt ett)
        {
            Dispatcher.Invoke(() =>
            {
                var total = (ett.MemoryUsed == _inv|| ett.MemoryFree == _inv)?_inv: ett.MemoryUsed + ett.MemoryFree;
                TbLoad.Text = ett.MemoryLoad == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.MemoryLoad)}%";
                TbUsed.Text = ett.MemoryUsed == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.MemoryUsed)}GB";
                TbFree.Text = ett.MemoryFree == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.MemoryFree)}GB";
                TbTotal.Text = total == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(total)}GB";
            });
        }
    }
}
