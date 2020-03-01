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
        private char[] _font = new char[] { ((char)0xEA3B), ((char)0xEA3A) };
        private string _ip;
        private int _reflushTime = 0;
        private double _delayActualWidth = 0;

        #region 计数参数


        //历史连通率计数表（有限次）
        private byte[] _historyQueue100 = new byte[100];//1成功0失败
        private byte[] _historyQueue50 = new byte[50];//1成功0失败
        private byte[] _historyQueue10 = new byte[10];//1成功0失败

        //历史连通率计数表索引（有限次）
        private int _historyIndex100 = 0;
        private int _historyIndex50 = 0;
        private int _historyIndex10 = 0;

        //历史连通率计数（无限次）
        private int _historyQueueAllCount = 0;
        private int _historyQueueAllSuccessCount = 0;

        //连续成功和失败计数
        private int _continuousSuccessCount = 0;
        private int _continuousFailedCount = 0;

        //联通延迟统计   <60   <120   <460   <1000   <3000   >3000
        private int[] _delayTimesCount = new int[6];

        #endregion

        #region Flag

        private bool _loadOver = false;
        private bool isStart = false;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            CheckIsFirstStartup();
            InitUI();

            _ping = new Ping();
            tboxIP.Text = _ip = Settings.Default.IP;
            if (CheckIP(_ip))
            {
                Start();
            }
            _reflushTime = Settings.Default.ReflushTime;
            tboxReflushTime.Text = _reflushTime.ToString();

        }

        #region 窗体事件

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _loadOver = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.MainWindowPosition = this.RestoreBounds;
            Settings.Default.Save();
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
            var alert = "";
            var changedFlag = false;
            var ip = tboxIP.Text.ToString();
            if (_ip != ip)
            {
                if (CheckIP(ip))
                {
                    _ip = ip;
                    Settings.Default.IP = ip;
                    Settings.Default.Save();
                    ReloadData();
                    Start();
                }
                else
                {
                    alert = "IP格式不规范";
                    tboxIP.Text = _ip;
                }
                changedFlag = true;
            }
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
                    if (_delayActualWidth == 0)
                        _delayActualWidth = spDelay.ActualWidth;
                    _historyQueueAllCount++;
                    if (null == _ping) return;
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
                            StatisticsDelayCount(p.RoundtripTime);
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
                        tbCountAll.Text = _historyQueueAllCount.ToString();
                        tbCountSuccess.Text = _historyQueueAllSuccessCount.ToString();
                        tbCountFailed.Text = (_historyQueueAllCount - _historyQueueAllSuccessCount).ToString();
                        UpdatePercentUI(tbPercent100, tbPercentText100, p100);
                        UpdatePercentUI(tbPercent50, tbPercentText50, p50);
                        UpdatePercentUI(tbPercent10, tbPercentText10, p10);
                        UpdatePercentUI(tbPercentAll, tbPercentTextAll, Convert.ToInt16(_historyQueueAllCount == 0 ? 0 : (double)_historyQueueAllSuccessCount / _historyQueueAllCount * 100));
                        
                    }));
                    await Task.Delay(_reflushTime);
                }
            }).Start();
        }

        /// <summary>
        /// 联通延迟统计
        /// </summary>
        /// <param name="roundtripTime"></param>
        private void StatisticsDelayCount(long roundtripTime)
        {
            if (roundtripTime <= 60)
            {
                _delayTimesCount[0]++;
                tbDelay60.Text = _delayTimesCount[0].ToString();
            }
            else if (roundtripTime <= 120)
            {
                _delayTimesCount[1]++;
                tbDelay120.Text = _delayTimesCount[1].ToString();
            }
            else if (roundtripTime <= 460)
            {
                _delayTimesCount[2]++;
                tbDelay460.Text = _delayTimesCount[2].ToString();
            }
            else if (roundtripTime <= 1000)
            {
                _delayTimesCount[3]++;
                tbDelay1000.Text = _delayTimesCount[3].ToString();
            }
            else if (roundtripTime <= 3000)
            {
                _delayTimesCount[4]++;
                tbDelay3000.Text = _delayTimesCount[4].ToString();
            }
            else
            {
                _delayTimesCount[5]++;
                tbDelay10000.Text = _delayTimesCount[5].ToString();
            }
            if (_historyQueueAllSuccessCount != 0)
            {
                bdrDelay60.Width = (double)_delayTimesCount[0] / _historyQueueAllSuccessCount * _delayActualWidth;
                bdrDelay120.Width = (double)_delayTimesCount[1] / _historyQueueAllSuccessCount * _delayActualWidth;
                bdrDelay460.Width = (double)_delayTimesCount[2] / _historyQueueAllSuccessCount * _delayActualWidth;
                bdrDelay1000.Width = (double)_delayTimesCount[3] / _historyQueueAllSuccessCount * _delayActualWidth;
                bdrDelay3000.Width = (double)_delayTimesCount[4] / _historyQueueAllSuccessCount * _delayActualWidth;
                bdrDelay10000.Width = (double)_delayTimesCount[5] / _historyQueueAllSuccessCount * _delayActualWidth;
            }
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
                var count = 0;
                for (int i = 0; i < successlength; i++)
                {
                    count++;
                    result = $"{result}{_font[0]}{(count == 10 ? "" : " ")}";
                }
                for (int i = 0; i < 10 - successlength; i++)
                {
                    count++;
                    result = $"{result}{_font[1]}{(count == 10 ? "" : " ")}";
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

        private void InitUI()
        {
            //读取配置文件
            try
            {
                //设置位置、大小
                Rect restoreBounds = Settings.Default.MainWindowPosition;
                this.Left = restoreBounds.Left;
                this.Top = restoreBounds.Top;
            }
            catch { }
            ShowInTaskbar = false;
            gridTool.Visibility = Visibility.Collapsed;
        }


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
