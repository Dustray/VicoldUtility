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
    public partial class GPUMiniPage : Page
    {
        private float _inv;
        public GPUMiniPage()
        {
            InitializeComponent();
            _inv = Settings.Default.InvalidValue;
        }
        public void ImportData(GPUEtt ett)
        {
            if (ett == null) return;
            Dispatcher.Invoke(() =>
            {
                TbCoreLoad.Text = ett.Core.CoreLoad == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.Core.CoreLoad)}%";
                TbCoreTemp.Text = ett.Core.CoreTemperature == _inv ? "*" : $"{PointRoundUtil.ToVision1Point(ett.Core.CoreTemperature)}℃";
            });
        }
    }
}
