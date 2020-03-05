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
    public partial class CPUPage : Page
    {
        private float _inv;
        private bool _loadedCoreGrid = false;
        private Dictionary<string, TextBlock> _textBlockDic;
        public CPUPage()
        {
            InitializeComponent();
            _inv = Settings.Default.InvalidValue;
            _textBlockDic = new Dictionary<string, TextBlock>();
            TbTitle.Foreground = new SolidColorBrush(Settings.Default.TitleColor);
        }

        public void ImportData(CPUEtt ett)
        {
            if (ett == null) return;
            Dispatcher.Invoke(() =>
            {
                elpStabilityBreath.Fill = ColorUtil.GetColor(ett.TotalLoad/10);
                TbMainLoad.Text = ett.TotalLoad == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(ett.TotalLoad)}%";
                TbMainCorePower.Text = ett.CorePower == _inv ? "*" : $"{PointRoundUtil.ToVision1Point(ett.CorePower)}W";
                TbMainPackagePower.Text = ett.PackagePower == _inv ? "*" : $"{PointRoundUtil.ToVision1Point(ett.PackagePower)}W";
                TbMainTemp.Text = ett.PackageTemperature == _inv ? "*" : $"{PointRoundUtil.ToVision1Point(ett.PackageTemperature)}℃";
                if (!_loadedCoreGrid)
                {
                    for(int i=1;i<= ett.CoreDic.Count;i++)
                    {
                        var row = new RowDefinition();
                        GridCore.RowDefinitions.Add(row);

                        var tbCoreTitle = CreateTitleTextBlock($"Core {i}");
                        tbCoreTitle.SetValue(Grid.RowProperty, i);
                        GridCore.Children.Add(tbCoreTitle);

                        var tbLoadTitle = CreateValueTextBlock();
                        tbLoadTitle.SetValue(Grid.RowProperty, i);
                        tbLoadTitle.SetValue(Grid.ColumnProperty, 1);
                        GridCore.Children.Add(tbLoadTitle);
                        _textBlockDic[$"Load{i}"] = tbLoadTitle;

                        var tbClockTitle = CreateValueTextBlock();
                        tbClockTitle.SetValue(Grid.RowProperty, i);
                        tbClockTitle.SetValue(Grid.ColumnProperty, 2);
                        GridCore.Children.Add(tbClockTitle);
                        _textBlockDic[$"Clock{i}"] = tbClockTitle;

                        var tbTemperatureTitle = CreateValueTextBlock();
                        tbTemperatureTitle.SetValue(Grid.RowProperty, i);
                        tbTemperatureTitle.SetValue(Grid.ColumnProperty, 3);
                        GridCore.Children.Add(tbTemperatureTitle);
                        _textBlockDic[$"Temperature{i}"] = tbTemperatureTitle;
                    }
                    _loadedCoreGrid = true;
                }
                int j = 1;
                foreach (var core in ett.CoreDic)
                {
                    if (_textBlockDic.TryGetValue($"Load{j}", out var tb1))
                    {
                        tb1.Text = core.Value.Load == _inv ? "*" : $"{PointRoundUtil.ToVision2Point(core.Value.Load)}%";
                    }
                    if (_textBlockDic.TryGetValue($"Clock{j}", out var tb2))
                    {
                        tb2.Text = core.Value.Clock == _inv ? "*" : $"{PointRoundUtil.ToVision0Point(core.Value.Clock)}MHz";
                    }
                    if (_textBlockDic.TryGetValue($"Temperature{j}",out var tb3))
                    {
                        tb3.Text = core.Value.Temperature == _inv ? "*" : $"{PointRoundUtil.ToVision1Point(core.Value.Temperature)}℃";
                    }
                    j++;
                }
            });
        }

        public TextBlock CreateTitleTextBlock(string title)
        {
            var tbCoreTitle = new TextBlock();
            tbCoreTitle.Text = title;
            tbCoreTitle.VerticalAlignment = VerticalAlignment.Center;
            tbCoreTitle.HorizontalAlignment = HorizontalAlignment.Center;
            tbCoreTitle.FontSize = 10;
            tbCoreTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 238, 238, 238));
            tbCoreTitle.FontWeight = FontWeights.Bold;
            return tbCoreTitle;
        }
        public TextBlock CreateValueTextBlock()
        {
            var tbCoreTitle = new TextBlock();
            tbCoreTitle.VerticalAlignment = VerticalAlignment.Center;
            tbCoreTitle.HorizontalAlignment = HorizontalAlignment.Center;
            tbCoreTitle.FontSize = 10;
            tbCoreTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 221,221,221));
            return tbCoreTitle;
        }
    }
}
