using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OpenHardwareMonitor.Hardware;
using Vicold.Popup;
using VicoldUtility.ResourceMonitor.Components;
using VicoldUtility.ResourceMonitor.Entities;
using VicoldUtility.ResourceMonitor.Properties;

namespace VicoldUtility.ResourceMonitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private CPUPage _cpuPage;
        private GPUPage _gpuPage;
        private MemoryPage _memoryPage;
        private CPUMiniPage _cpuMiniPage;
        private GPUMiniPage _gpuMiniPage;
        private MemoryMiniPage _memoryMiniPage;
        private Task _runTask;

        private int _reflushTime = 1000;
        private bool _isStart = false;
        private bool _isSimpleMode = false;

        public MainWindow()
        {
            InitializeComponent();
            CheckIsFirstStartup();
            InitUI();
            _cpuPage = new CPUPage();
            _gpuPage = new GPUPage();
            _memoryPage = new MemoryPage();

            _cpuMiniPage = new CPUMiniPage();
            _gpuMiniPage = new GPUMiniPage();
            _memoryMiniPage = new MemoryMiniPage();

            FrameCPU.Navigate(_cpuPage);
            FrameGPU.Navigate(_gpuPage);
            FrameMemory.Navigate(_memoryPage);

            Start();

            _reflushTime = Settings.Default.ReflushTime;
            tboxReflushTime.Text = _reflushTime.ToString();
        }

        private void Start()
        {
            if (_isStart) return;
            if (_runTask != null) return;
            btnStartOrPause.Content = "暂停";
            _isStart = true;

            var manager = new ResourceManager();
            var computer = new Computer();
            computer.Open();
            //启动主板监测
            computer.MainboardEnabled = false;
            //启动CPU监测
            computer.CPUEnabled = true;
            //启动内存监测
            computer.RAMEnabled = true;
            //启动GPU监测
            computer.GPUEnabled = true;
            //启动风扇监测
            computer.FanControllerEnabled = true;
            //启动硬盘监测
            computer.HDDEnabled = false;

            _runTask = new Task(async () =>
            {
                while (_isStart) 
                {
                    computer.Accept(manager);
                    //Console.Clear();
                    foreach (var hardwareItem in computer.Hardware)
                    {
                        switch (hardwareItem.HardwareType)
                        {
                            case HardwareType.CPU:
                                _cpuPage.ImportData(CPUDataFill(hardwareItem.Sensors));
                                _cpuMiniPage.ImportData(CPUDataFill(hardwareItem.Sensors));
                                break;
                            case HardwareType.GpuAti:
                            case HardwareType.GpuNvidia:
                                _gpuPage.ImportData(GPUDataFill(hardwareItem.Sensors));
                                _gpuMiniPage.ImportData(GPUDataFill(hardwareItem.Sensors));
                                break;
                            case HardwareType.RAM:
                                _memoryPage.ImportData(MemoryDataFill(hardwareItem.Sensors));
                                _memoryMiniPage.ImportData(MemoryDataFill(hardwareItem.Sensors));
                                break;
                            case HardwareType.SuperIO:
                                break;
                            case HardwareType.Mainboard:
                                break;
                            case HardwareType.HDD:
                                break;
                            case HardwareType.Heatmaster:
                                break;
                            case HardwareType.TBalancer:
                                break;
                        }
                        Dispatcher.Invoke(() =>
                        {
                            Console.WriteLine(hardwareItem.Name);
                            //tbCPU.Text = hardwareItem.Name;
                        });
                        foreach (var sensor in hardwareItem.Sensors)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                if (sensor.Name.Contains("Total"))
                                {

                                }
                                Console.WriteLine(sensor.Name + "的" + sensor.SensorType + "是" + sensor.Value);
                                //tbCPU.Text = hardwareItem.Name;
                            });
                            //Console.WriteLine(sensor.Name + "的" + sensor.SensorType + "是" + sensor.Value);
                        }
                    }
                    await Task.Delay(_reflushTime);
                }
                _runTask.Dispose();
                _runTask = null;
                Dispatcher.Invoke(new Action(() =>
                {
                    btnStartOrPause.Content = "开始";
                }));
            });
            _runTask.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        private void Stop()
        {
            _isStart = false;
        }

        #region 窗体事件

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.MainWindowPosition = RestoreBounds;
            Settings.Default.Save();
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {

        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (RestoreBounds.Height - e.GetPosition(this).Y < 30)
            {
                gridTool.Visibility = Visibility.Visible;
            }

            double formLeft = Left;//窗口右边缘=窗口左上角x+窗口宽度
            double formRight = Left + RestoreBounds.Width;//窗口右边缘=窗口左上角x+窗口宽度
            double formCenter = (formRight + formLeft) / 2;
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                int screenLeft = screen.Bounds.Left;//屏幕右边缘
                int screenRight = screen.Bounds.Right;//屏幕右边缘
                if (formCenter >= screenLeft && formCenter <= screenRight)
                {
                    if (screenRight - formRight <= 30) //往右靠
                        Left = screenRight - RestoreBounds.Width - 5;
                    if (formLeft - screenLeft <= 30)//往左靠
                        Left = screenLeft + 5;
                    int screenTop = screen.Bounds.Top;//屏幕下边缘
                    int screenBottom = screen.Bounds.Bottom;//屏幕下边缘
                    double formTop = Top;//窗口下边缘
                    double formBottom = Top + RestoreBounds.Height;//窗口下边缘
                    if (screenBottom - formBottom <= 30)//往下靠
                        Top = screenBottom - RestoreBounds.Height - 5;
                    if (formTop - screenTop <= 30)//往上靠
                        Top = screenTop + 5;
                    break;
                }
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            gridTool.Visibility = Visibility.Collapsed;
            var alert = "";
            var changedFlag = false;

            var timeStr = tboxReflushTime.Text.ToString();
            var reflushTime = int.TryParse(timeStr, out int time);
            if (_reflushTime != time)
            {
                if (reflushTime && time >= 10 && time <= 3600000)
                {
                    _reflushTime = time;
                    Settings.Default.ReflushTime = time;
                    Settings.Default.Save();
                }
                else
                {
                    if (alert != "") alert = $"{alert}\r\n刷新时间间隔格式不准确";
                    tboxReflushTime.Text = timeStr;
                }
                changedFlag = true;
            }
            if (changedFlag)
            {
                if (alert == "")
                {
                    Alert.Show("修改成功", AlertTheme.Success);
                }
                else
                {
                    Alert.Show("IP格式不规范", AlertTheme.Error);
                }
            }
        }
        #endregion

        #region 成员事件

        private void btnStartOrPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isStart)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void sldBgTrans_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var sl = sender as Slider;
            Background = new SolidColorBrush(Color.FromArgb(Settings.Default.BgTrans, 40, 40, 40));
            Settings.Default.BgTrans = (byte)sl.Value;
            Settings.Default.Save();
        }

        private void btnSimple_Click(object sender, RoutedEventArgs e)
        {
            if (_isSimpleMode)
            {
                FrameCPU.Navigate(_cpuPage);
                FrameGPU.Navigate(_gpuPage);
                FrameMemory.Navigate(_memoryPage);
                btnSimple.Content = "极简";
            }
            else
            {
                FrameCPU.Navigate(_cpuMiniPage);
                FrameGPU.Navigate(_gpuMiniPage);
                FrameMemory.Navigate(_memoryMiniPage);
                btnSimple.Content = "详细";
            }
            _isSimpleMode = !_isSimpleMode;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region 成员方法


        private bool CheckIsFirstStartup()
        {

            if (Settings.Default.IsFirstStartup)
            {
                Settings.Default.Upgrade();
                Settings.Default.IsFirstStartup = false;
                Settings.Default.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void InitUI()
        {
            //读取配置文件
            try
            {
                //设置位置、大小
                Rect restoreBounds = Settings.Default.MainWindowPosition;
                Left = restoreBounds.Left;
                Top = restoreBounds.Top;
            }
            catch { }
            ShowInTaskbar = false;
            gridTool.Visibility = Visibility.Collapsed;

            tbMyLogo.ToolTip = $"Version {Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
            Background = new SolidColorBrush(Color.FromArgb(Settings.Default.BgTrans, 40, 40, 40));
            sldBgTrans.Value = Settings.Default.BgTrans;
        }

        #endregion

        #region 数据填充

        public CPUEtt CPUDataFill(ISensor[] sensors)
        {
            var cpuEtt = new CPUEtt();
            cpuEtt.CoreDic = new System.Collections.Generic.Dictionary<string, CPUCoreEtt>();
            cpuEtt.CCDTemperatureDic = new System.Collections.Generic.Dictionary<string, float>();
            for (int i=0;i< sensors.Length;i++)
            {
                var sensor = sensors[i];
                var value = sensor.Value ?? Settings.Default.InvalidValue;
                switch (sensor.SensorType)
                {
                    case SensorType.Load:
                        if (sensor.Name.Contains("Total"))
                        {
                            cpuEtt.TotalLoad = value;
                        }
                        else if (sensor.Name.Contains("#"))
                        {
                            if (cpuEtt.CoreDic.TryGetValue(sensor.Name, out CPUCoreEtt ett1))
                            {
                                ett1.Load = value;
                            }
                            else
                            {
                                cpuEtt.CoreDic[sensor.Name] = new CPUCoreEtt() { Load = value };
                            }
                        }
                        break;
                    case SensorType.Power:

                        if (sensor.Name.Contains("Cores"))
                        {
                            cpuEtt.CorePower = value;
                        }
                        else if (sensor.Name.Contains("Package"))
                        {
                            cpuEtt.PackagePower = value;
                        }
                        else if (sensor.Name.Contains("Graphics"))
                        {
                            cpuEtt.GraphicsPower = value;
                        }
                        else if (sensor.Name.Contains("DRAM"))
                        {
                            cpuEtt.DRAMPower = value;
                        }
                        else if (sensor.Name.Contains("#"))
                        {
                            if (cpuEtt.CoreDic.TryGetValue(sensor.Name, out CPUCoreEtt ett2))
                            {
                                ett2.Power = value;
                            }
                            else
                            {
                                cpuEtt.CoreDic[sensor.Name] = new CPUCoreEtt() { Power = value };
                            }
                        }
                        break;
                    case SensorType.Temperature:
                        if (sensor.Name.Contains("Package"))
                        {
                            cpuEtt.PackageTemperature = value;
                        }
                        else if (sensor.Name.Contains("CCD"))
                        {
                            cpuEtt.CCDTemperatureDic[sensor.Name] = value;
                        }
                        else if (sensor.Name.Contains("#"))
                        {
                            if (cpuEtt.CoreDic.TryGetValue(sensor.Name, out CPUCoreEtt ett2))
                            {
                                ett2.Temperature = value;
                            }
                            else
                            {
                                cpuEtt.CoreDic[sensor.Name] = new CPUCoreEtt() { Temperature = value };
                            }
                        }
                        break;
                    case SensorType.Clock:
                        if (cpuEtt.CoreDic.TryGetValue(sensor.Name, out CPUCoreEtt ett3))
                        {
                            ett3.Clock = value;
                        }
                        else if (sensor.Name.Contains("#"))
                        {
                            cpuEtt.CoreDic[sensor.Name] = new CPUCoreEtt() { Clock = value };
                        }
                        break;

                }
            }
            return cpuEtt;
        }

        public GPUEtt GPUDataFill(ISensor[] sensors)
        {
            var gpuEtt = new GPUEtt();
            gpuEtt.Core = new GPUCoreEtt();
            gpuEtt.Memory = new GPUMemoryEtt();
            gpuEtt.Fan = new GPUFanEtt();
            foreach (var sensor in sensors)
            {
                var value = sensor.Value ?? Settings.Default.InvalidValue;
                switch (sensor.SensorType)
                {

                    case SensorType.Load:
                        if (sensor.Name.Contains("Core"))
                        {
                            gpuEtt.Core.CoreLoad = value;
                        }
                        else if (sensor.Name.Contains("Memory"))
                        {
                            gpuEtt.Memory.MemoryLoad = value;
                        }
                        break;
                    case SensorType.Power:

                        break;
                    case SensorType.Temperature:
                        if (sensor.Name.Contains("Core"))
                        {
                            gpuEtt.Core.CoreTemperature = value;
                        }
                        else if (sensor.Name.Contains("Memory"))
                        {
                            gpuEtt.Memory.MemoryTemperature = value;
                        }
                        break;
                    case SensorType.Clock:
                        if (sensor.Name.Contains("Core"))
                        {
                            gpuEtt.Core.CoreClock = value;
                        }
                        else if (sensor.Name.Contains("Memory"))
                        {
                            gpuEtt.Memory.MemoryClock = value;
                        }
                        break;
                    case SensorType.SmallData:
                        if (sensor.Name.Contains("Memory Total"))
                        {
                            gpuEtt.Memory.MemoryTotal = value;
                        }
                        break;
                    case SensorType.Control:
                        if (sensor.Name.Contains("Fan"))
                        {
                            gpuEtt.Fan.FanLoad = value;
                        }
                        break;
                    case SensorType.Fan:
                        if (sensor.Name.Contains("Fan"))
                        {
                            gpuEtt.Fan.FanSpeed = value;
                        }
                        break;
                }
            }
            return gpuEtt;
        }

        public MemoryEtt MemoryDataFill(ISensor[] sensors)
        {
            var memoryEtt = new MemoryEtt();

            foreach (var sensor in sensors)
            {
                var value = sensor.Value ?? Settings.Default.InvalidValue;
                switch (sensor.SensorType)
                {
                    case SensorType.Load:
                        memoryEtt.MemoryLoad = value;
                        break;
                    case SensorType.Data:
                        if (sensor.Name.Contains("Used"))
                        {
                            memoryEtt.MemoryUsed = value;
                        }
                        else if (sensor.Name.Contains("Available"))
                        {
                            memoryEtt.MemoryFree = value;
                        }
                        break;
                }
            }
            return memoryEtt;
        }

        #endregion

        
    }
}
