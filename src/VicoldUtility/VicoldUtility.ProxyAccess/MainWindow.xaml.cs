using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Vicold.Popup;
using Newtonsoft.Json;
using System.IO;

namespace VicoldUtility.ProxyAccess
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ProxyIPEtt> _proxyIPList = new List<ProxyIPEtt>();
        private IPRequireManager _manager;
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 1; i <= 50; i++)
            {
                _proxyIPList.AddRange(new ProxyIPManager().GetProxyIPs(i));
            }
            var proxy_json = JsonConvert.SerializeObject(_proxyIPList);
            var path = Path.GetFullPath(@"data");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, "proxy_ip_data.json");
            System.IO.File.WriteAllText(filePath, proxy_json);
        }

        #region 成员事件

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            var targetIP = TboxTargetIP.Text.ToString();
            if (string.IsNullOrWhiteSpace(targetIP))
            {
                Alert.Show("目标地址不能为空", AlertTheme.Error);
                return;
            }
            if (_manager == null)
            {
                IPRequireMod mod = new IPRequireMod();
                if ((bool)RdForSuccessCount.IsChecked)
                {
                    if (int.TryParse(TboxSuccessCount.Text.ToString(), out var c1) && c1 > 0)
                    {
                        mod.ChangeToSuccessMod(c1);
                    }
                    else
                    {
                        Alert.Show("格式不正确", "执行规则按访问次数无效", AlertTheme.Error);
                        return;
                    }
                }
                else
                {
                    if (int.TryParse(TboxTimesCount.Text.ToString(), out var c2) && c2 > 0)
                    {
                        mod.ChangeToTimesMod(c2);
                    }
                    else
                    {
                        Alert.Show("格式不正确", "执行规则按成功次数无效", AlertTheme.Error);
                        return;
                    }
                }
                _manager = new IPRequireManager(mod);
                _manager.AddResource(_proxyIPList, CreateUserAgent());
                _manager.SetExecuteCountCallback(Statistic);
                _manager.SetLogCallback(WriteLog);
            }
            ChangeState(_manager.IsStarting, targetIP);
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            ChangeState(true);
            _manager?.Dispose();
            _manager = null;
            RdForVisitCount.IsEnabled = true;
            RdForSuccessCount.IsEnabled = true;
            TboxTimesCount.IsEnabled = true;
            TboxSuccessCount.IsEnabled = true;
            TbExecuteCountSuccess.Text = "0";
            TbExecuteCountDid.Text = "0";
            TbExecuteCountAll.Text = "0";
            TboxLog.Clear();
        }

        #endregion

        public List<string> CreateUserAgent()
        {
            return new List<string> {
                "Lynx/2.8.5rel.1 libwww-FM/2.14 SSL-MM/1.4.1 GNUTLS/1.2.9",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
                "Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.9.168 Version/11.50",
                "Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11",
            };
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="targetIP"></param>
        private void ChangeState(bool flag, string targetIP = "")
        {
            if (flag)
            {
                _manager?.Stop();
                BtnStart.Background = new SolidColorBrush(Color.FromRgb(25, 128, 218));
                BtnStart.Content = "开始";
                BtnReload.Visibility = Visibility.Visible;
            }
            else
            {
                _manager?.Start(targetIP);
                BtnStart.Background = new SolidColorBrush(Color.FromRgb(125, 128, 218));
                BtnStart.Content = "停止";
                BtnReload.Visibility = Visibility.Hidden;
            }

            RdForVisitCount.IsEnabled = false;
            RdForSuccessCount.IsEnabled = false;
            TboxTimesCount.IsEnabled = false;
            TboxSuccessCount.IsEnabled = false;
        }

        #region 回调

        /// <summary>
        /// 统计
        /// </summary>
        /// <param name="success"></param>
        /// <param name="times"></param>
        /// <param name="all"></param>
        public void Statistic(int success, int times, int all)
        {
            TbExecuteCountSuccess.Text = success.ToString();
            TbExecuteCountDid.Text = times.ToString();
            TbExecuteCountAll.Text = all.ToString();
            if (times == all)
            {
                ChangeState(true);
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="text"></param>
        public void WriteLog(bool flag, string text)
        {
            Console.WriteLine($"{text}");
            TboxLog.AppendText($"[{(flag ? "成功" : "失败")}] {text}\r\n");
            TboxLog.ScrollToEnd();
        }

        #endregion

    }
}
