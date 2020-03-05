using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VicoldUtility.ResourceMonitor.Entities;
using VicoldUtility.ResourceMonitor.Properties;
using VicoldUtility.ResourceMonitor.Utilities;

namespace VicoldUtility.ResourceMonitor.Components
{
    /// <summary>
    /// CPUPage.xaml 的交互逻辑
    /// </summary>
    public partial class CPUMiniPage : Page
    {
        private float _inv;
        public CPUMiniPage()
        {
            InitializeComponent();
            _inv = Settings.Default.InvalidValue;
        }

        public void ImportData(CPUEtt ett)
        {
            if (ett == null) return;
            Dispatcher.Invoke(() =>
            {
                TbMainLoad.Text = ett.TotalLoad == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.TotalLoad)}%";
                TbMainTemp.Text = ett.PackageTemperature == _inv ? "*" : $"{PointRoundUtil.ToVision1Point(ett.PackageTemperature)}℃";
            });
        }
    }
}
