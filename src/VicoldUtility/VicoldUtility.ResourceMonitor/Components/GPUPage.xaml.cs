using System.Windows.Controls;
using System.Windows.Media;
using VicoldUtility.ResourceMonitor.Entities;
using VicoldUtility.ResourceMonitor.Properties;
using VicoldUtility.ResourceMonitor.Utilities;

namespace VicoldUtility.ResourceMonitor.Components
{
    /// <summary>
    /// GPUPage.xaml 的交互逻辑
    /// </summary>
    public partial class GPUPage : Page
    {
        private float _inv;
        private bool _loadedCoreGrid = false;
        public GPUPage()
        {
            InitializeComponent();
            _inv = Settings.Default.InvalidValue;
            TbTitle.Foreground = new SolidColorBrush(Settings.Default.TitleColor);
        }
        public void ImportData(GPUEtt ett)
        {
            if (ett == null) return;
            Dispatcher.Invoke(() =>
            {
                elpStabilityBreath.Fill = ColorUtil.GetColor(ett.Core.CoreLoad / 10);
                TbCoreLoad.Text = ett.Core.CoreLoad == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.Core.CoreLoad)}%";
                TbCoreClock.Text = ett.Core.CoreClock == _inv ? "*" : $"{PointRoundUtil.ToVision0Point(ett.Core.CoreClock)}MHz";
                TbCoreTemp.Text = ett.Core.CoreTemperature == _inv ? "*" : $"{PointRoundUtil.ToVision1Point(ett.Core.CoreTemperature)}℃";

                TbMemoryLoad.Text = ett.Memory.MemoryLoad == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.Memory.MemoryLoad)}%";
                TbMemoryClock.Text = ett.Memory.MemoryClock == _inv ? "*" : $"{PointRoundUtil.ToVision0Point(ett.Memory.MemoryClock)}MHz";
                TbMemoryTemp.Text = ett.Memory.MemoryTemperature == _inv ? "*" : $"{PointRoundUtil.ToVision1Point(ett.Memory.MemoryTemperature)}℃";

                if (ett.Memory.MemoryTotal != _inv)
                {
                    var used = ett.Memory.MemoryLoad / 100 * ett.Memory.MemoryTotal;
                    var free = ett.Memory.MemoryTotal - used;
                    TbMemoryUsed.Text = $"{PointRoundUtil.ToVision1Point(used)}MB";
                    TbMemoryFree.Text = $"{PointRoundUtil.ToVision1Point(free)}MB";
                    TbMemoryTotal.Text = $"{PointRoundUtil.ToVision1Point(ett.Memory.MemoryTotal)}MB";
                }
                else
                {
                    TbMemoryUsed.Text =  "*";
                    TbMemoryFree.Text =  "*";
                    TbMemoryTotal.Text = "*";
                }
                TbFanLoad.Text = ett.Fan.FanLoad == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.Fan.FanLoad)}%";
                TbFanSpeed.Text = ett.Fan.FanSpeed == _inv ? "*" : $"{PointRoundUtil.ToVision0Point(ett.Fan.FanSpeed)}RPM";
            });
        }
    }
}
