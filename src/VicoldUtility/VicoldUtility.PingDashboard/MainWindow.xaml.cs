using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vicold.Popup;
using VicoldUtility.PingDashboard.Properties;

namespace VicoldUtility.PingDashboard
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Ping _ping;
        //private FontEtt _fontSuccess = new FontEtt() {   FontContent=((char)0xEA3B).ToString()};
        //private FontEtt _fontFaild = new FontEtt() { FontContent = ((char)0xEA3A).ToString()};
        private char[] _font = new char[] { ((char)0xEA3B), ((char)0xEA3A) };
        private string _ip;
        /// <summary>
        /// 1成功0失败
        /// </summary>
        #region 计数参数

        private byte[] _historyQueue100 = new byte[100];
        private byte[] _historyQueue50 = new byte[50];
        private byte[] _historyQueue10 = new byte[10];
        private int _historyQueueAllCount = 0;
        private int _historyQueueAllSuccessCount = 0;

        private int _historyIndex100 = 0;
        private int _historyIndex50 = 0;
        private int _historyIndex10 = 0;

        private int _continuousSuccessCount = 0;
        private int _continuousFailedCount = 0;

        #endregion

        #region Flag

        private bool _loadOver = false;
        private bool isStart = false;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            gridTool.Visibility = Visibility.Collapsed;
            _ping = new Ping();
            tboxIP.Text = _ip = Settings.Default.IP;
            if (CheckIP(_ip))
            {
                Start();
            }
        }

        #region 窗体事件

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _loadOver = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _ping?.Dispose();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            gridTool.Visibility = Visibility.Visible;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            gridTool.Visibility = Visibility.Collapsed;
            var ip = tboxIP.Text.ToString();
            if (_ip == ip) return;
            if (CheckIP(ip))
            {
                _ip = ip;
                Settings.Default.IP = ip;
                Settings.Default.Save();
                Alert.Show("修改IP成功", AlertTheme.Success);
                ReloadData();
                Start();
            }
            else
            {
                Alert.Show("IP格式不规范", AlertTheme.Error);
                tboxIP.Text = _ip;
            }
        }

        private void tboxIP_MouseEnter(object sender, MouseEventArgs e)
        {
            Window_MouseEnter(sender, e);
        }
        #endregion

        #region 统计执行

        /// <summary>
        /// 开始
        /// </summary>
        private void Start()
        {
            if (isStart) return;
            isStart = true;
            new Task(async () =>
            {
                while (isStart)
                {
                    _historyQueueAllCount++;
                    var p = _ping.Send(_ip);

                    var flag = false;

                    switch (p.Status)
                    {
                        case IPStatus.Success:
                            //成功
                            flag = true;
                            _historyQueueAllSuccessCount++;
                            _continuousSuccessCount++;
                            _continuousFailedCount = 0;
                            break;
                        case IPStatus.TimedOut:
                        //超时
                        default:
                            //失败
                            flag = false;
                            _continuousFailedCount++;
                            _continuousSuccessCount = 0;
                            break;
                    }
                    AddHistory(ref _historyQueue100, ref _historyIndex100, flag);
                    AddHistory(ref _historyQueue50, ref _historyIndex50, flag);
                    AddHistory(ref _historyQueue10, ref _historyIndex10, flag);
                    var p100 = GetPercent(_historyQueue100);
                    var p50 = GetPercent(_historyQueue50);
                    var p10 = GetPercent(_historyQueue10);
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        if (p.Status == IPStatus.Success)
                        {
                            tbActualByte.Text = p.Buffer.Length.ToString();
                            tbActualTime.Text = p.RoundtripTime.ToString();
                            tbActualTTL.Text = p.Options.Ttl.ToString();
                            tbContinuousCount.Text = $"已连续Ping成功{_continuousSuccessCount}次";
                        }
                        else
                        {
                            tbActualByte.Text = "*";
                            tbActualTime.Text = "*";
                            tbActualTTL.Text = "*";
                            tbContinuousCount.Text = $"已连续Ping失败{_continuousFailedCount}次";
                            if (_continuousFailedCount == 20)
                            {
                                Alert.Show("警告", $"Ping{_ip}已连续失败{_continuousFailedCount}次", AlertTheme.Warning, new AlertConfig() { AlertShowDuration = -1, OnlyOneWindowAllowed = true });
                            }
                        }
                        UpdatePercentUI(tbPercent100, tbPercentText100, p100);
                        UpdatePercentUI(tbPercent50, tbPercentText50, p50);
                        UpdatePercentUI(tbPercent10, tbPercentText10, p10);
                        UpdatePercentUI(tbPercentAll, tbPercentTextAll, Convert.ToInt16(_historyQueueAllCount == 0 ? 0 : (double)_historyQueueAllSuccessCount / _historyQueueAllCount * 100));
                    }));
                    await Task.Delay(1000);
                }
            }).Start();
        }
        /// <summary>
        /// 停止
        /// </summary>
        private void Stop()
        {
            isStart = false;
        }

        /// <summary>
        /// 添加历史记录
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="index"></param>
        /// <param name="isSuccess"></param>
        public void AddHistory(ref byte[] queue, ref int index, bool isSuccess)
        {
            var value = (byte)(isSuccess ? 1 : 0);
            queue[index] = value;
            index++;
            if (index >= queue.Length)
                index = 0;
        }

        /// <summary>
        /// 计算百分比0-100
        /// </summary>
        /// <param name="queue"></param>
        /// <returns>0 - 100</returns>
        private int GetPercent(byte[] queue)
        {
            var successCount = 0;
            foreach (var re in queue)
            {
                if (1 == re)
                    successCount++;
            }
            return Convert.ToInt16((double)successCount / queue.Length * 100);
        }

        /// <summary>
        /// 修改进度条UI
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="tbText"></param>
        /// <param name="percent"></param>
        private void UpdatePercentUI(TextBlock tb, TextBlock tbText, int percent)
        {
            var successPoint = Convert.ToInt32((double)percent / 100 * 10);
            tb.Text = GetContent(successPoint);
            tbText.Text = $"{percent}%";
            tb.Foreground = GetColor(10 - successPoint);

            string GetContent(int successlength)
            {
                var result = "";
                for (int i = 0; i < successlength; i++)
                {
                    result += _font[0] + " ";
                }
                for (int i = 0; i < 10 - successlength; i++)
                {
                    result += _font[1] + " ";
                }
                //result.PadRight(10, _font[1]);
                return result;
            }
            Brush GetColor(float val)
            {
                float one = (255 + 255) / 6;
                int r = 0, g = 0, b = 0;
                if (val < 3)
                {
                    r = (int)(one * val);
                    g = 255;
                }
                else if (val >= 3 && val < 6)
                {
                    r = 255;
                    g = 255 - (int)((val - 3) * one);
                }
                else { r = 255; }
                return new SolidColorBrush(Color.FromArgb((byte)200, (byte)r, (byte)g, (byte)b));
            }
        }

        #endregion

        #region 成员方法

        /// <summary>
        /// 重置参数
        /// </summary>
        private void ReloadData()
        {
            _historyQueue100 = new byte[100];
            _historyQueue50 = new byte[50];
            _historyQueue10 = new byte[10];
            _historyQueueAllCount = 0;
            _historyQueueAllSuccessCount = 0;

            _historyIndex100 = 0;
            _historyIndex50 = 0;
            _historyIndex10 = 0;

            _continuousSuccessCount = 0;
            _continuousFailedCount = 0;
        }


        #endregion
        #region 成员事件

        private void btnStartOrPause_Click(object sender, RoutedEventArgs e)
        {
            if (isStart)
            {
                btnStartOrPause.Content = "开始";
                Stop();
            }
            else
            {
                Start();
                btnStartOrPause.Content = "停止";
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region tool

        /// <summary>
        /// 检查ip格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool CheckIP(string ip)
        {
            var check = new System.Text.RegularExpressions.Regex(@"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
            return check.IsMatch(ip);
        }

        #endregion
    }

}
