using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OpenHardwareMonitor.Hardware;
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
        public MainWindow()
        {
            InitializeComponent();
            _cpuPage = new CPUPage();
            _gpuPage = new GPUPage();
            _memoryPage = new MemoryPage();
            FrameCPU.Navigate(_cpuPage);
            FrameGPU.Navigate(_gpuPage);
            FrameMemory.Navigate(_memoryPage);
            CheckIsFirstStartup();
            Start();
        }

        private void Start()
        {
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
            new Task(async () =>
            {
                do
                {
                    computer.Accept(manager);
                    //Console.Clear();
                    foreach (var hardwareItem in computer.Hardware)
                    {
                        switch (hardwareItem.HardwareType)
                        {
                            case HardwareType.CPU:
                                _cpuPage.ImportData(CPUDataFill(hardwareItem.Sensors));
                                break;
                            case HardwareType.GpuAti:
                            case HardwareType.GpuNvidia:
                                _gpuPage.ImportData(GPUDataFill(hardwareItem.Sensors));
                                break;
                            case HardwareType.RAM:
                                _memoryPage.ImportData(MemoryDataFill(hardwareItem.Sensors));
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
                        //Dispatcher.Invoke(() =>
                        //{
                        //    Console.WriteLine(hardwareItem.Name);
                        //    tbCPU.Text = hardwareItem.Name;
                        //});
                        //foreach (var sensor in hardwareItem.Sensors)
                        //{
                        //    Dispatcher.Invoke(() =>
                        //    {

                        //        Console.WriteLine(sensor.Name + "的" + sensor.SensorType + "是" + sensor.Value);
                        //        tbCPU.Text = hardwareItem.Name;
                        //    });
                        //    //Console.WriteLine(sensor.Name + "的" + sensor.SensorType + "是" + sensor.Value);
                        //}
                    }
                    await Task.Delay(1000);
                } while (true);
            }).Start();
        }

        #region 成员事件
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

        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {

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
                    case SensorType.Data:
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

        private void btnStartOrPause_Click(object sender, RoutedEventArgs e)
        {

        }

        private void sldBgTrans_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
