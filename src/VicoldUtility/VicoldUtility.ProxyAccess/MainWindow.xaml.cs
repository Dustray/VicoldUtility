using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace VicoldUtility.ProxyAccess
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ProxyIPEtt> _proxyIPList = new List<ProxyIPEtt>();
        private int _maxCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 1; i <= 5; i++)
            {
                GetProxyIPs(i);
            }

        }

        public void GetProxyIPs(int page = 1)
        {
            string url = $@"https://www.xicidaili.com/nt/{page}";
            var webGet = new HtmlWeb();
            webGet.UserAgent = "'Opera/9.25 (Windows NT 5.1; U; en)','Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)','Mozilla/5.0 (compatible; Konqueror/3.5; Linux) KHTML/3.5.5 (like Gecko) (Kubuntu)','Mozilla/5.0 (X11; U; linux i686; en-US; rv:1.8.0.12) Gecko/20070731 Ubuntu/dapper-security Firefox/1.5.0.12','Lynx/2.8.5rel.1 libwww-FM/2.14 SSL-MM/1.4.1 GNUTLS/1.2.9'\"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; AcooBrowser; .NET CLR 1.1.4322; .NET CLR 2.0.50727)\",\"Mozilla/4.0 (compatible; MSIE 7.0; AOL 9.5; AOLBuild 4337.35; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)\"";
            var document = webGet.Load(url);
            var div = document.DocumentNode.SelectNodes("//table[@id='ip_list']/tr");
            var i = 0;
            foreach (HtmlNode node in div)
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }
                var tmpNode = node.SelectNodes("td");
                var da = getM(tmpNode[8].InnerText);
                if (!da) continue;
                //Console.WriteLine(tmpNode[8].InnerText);
                _proxyIPList.Add(new ProxyIPEtt()
                {
                    IP = tmpNode[1].InnerText,
                    Port = tmpNode[2].InnerText,
                    //Address = tmpNode[3].ChildNodes[0]("a").InnerText,
                    Type = tmpNode[5].InnerText,
                    //Speed = tmpNode[6].InnerText,
                    LiveDate = tmpNode[8].InnerText,
                    VerificationTime = tmpNode[9].InnerText,
                });

            }
        }



        public bool getM(string time)
        {
            var day = "";
            var result = 0f;
            if (time.EndsWith("天"))
            {
                day = time.Substring(0, time.Length - 1);
                if (int.TryParse(day, out var d))
                {
                    //result = d * 24 * 60;
                    if (d <= 2) return false;
                }
            }
            else
            {
                return false;
            }
            //else if (time.EndsWith("小时"))
            //{
            //    day = time.Substring(0, time.Length - 2);
            //    if (int.TryParse(day, out var d2))
            //    {
            //        result = d2 * 60;
            //    }
            //}
            //else if (time.EndsWith("分钟"))
            //{
            //    day = time.Substring(0, time.Length - 2);
            //    if (int.TryParse(day, out var d3))
            //    {
            //        result = d3;
            //    }
            //}
            //if (result / 60 / 24 <= 2) return false;

            return true;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {

            //if (int.TryParse(TboxVisitCount.Text.ToString(),out ))
            //_maxCount = (int)TboxVisitCount.Text;

            var ad = TboxTargetIP.Text.ToString();
            int c = 0;
            int cEx = 0;
            if (string.IsNullOrWhiteSpace(ad)) return;
            new Task(() =>
            {
                Parallel.For(0, _proxyIPList.Count, (k, state) =>
                {
                    var ett = _proxyIPList[k];
                    var re = ExcuteIn(ett.IP, ett.Port, ad);

                    if (re) c++;
                    cEx++;
                    this.Dispatcher.Invoke(() =>
                    {
                        if (c == 10)
                        {
                            MessageBox.Show("成功");
                            state.Break();
                        }
                        WriteLog($"{ett.IP}:{ett.Port}");
                        if (re)
                            WriteLog("爬成功");
                        else
                            WriteLog("爬失败");

                        TbExecuteCountSuccess.Text = c.ToString();
                        TbExecuteCountDid.Text = cEx.ToString();
                        TbExecuteCountAll.Text = _proxyIPList.Count.ToString();
                    });
                });
                //foreach (var ett in _proxyIPList)
                //{
                //    //if (i >= _maxCount) break;
                //    WriteLog($"{ett.IP}:{ett.Port}");
                //    ExcuteIn(ett.IP, ett.Port, ad);
                //    i++;
                //    this.Dispatcher.Invoke(() =>
                //    {


                //    });

                //    await Task.Delay(1);
                //}
            }).Start();
        }


        private bool ExcuteIn(string ip, string port, string ipd)
        {
            int ports;
            if (!int.TryParse(port, out ports)) return false;
            WebProxy proxyObject = new WebProxy(ip, ports);//str为IP地址 port为端口号 代理类
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(ipd); // 访问这个网站 ，返回的就是你发出请求的代理ip 这个做代理ip测试非常方便，可以知道代理是否成功
            Req.UserAgent = RandomBrowserUa();
            //WriteLog($"{Req.UserAgent}");
            Req.Proxy = proxyObject; //设置代理
            Req.Method = "GET";
            try
            {
                using (HttpWebResponse Resp = (HttpWebResponse)Req.GetResponse())
                {

                    Encoding code = Encoding.GetEncoding("UTF-8");
                    using (StreamReader sr = new StreamReader(Resp.GetResponseStream(), code))
                    {
                        //WriteLog("爬成功");
                        var str = sr.ReadToEnd();//获取得到的网址html返回数据，这里就可以使用某些解析html的dll直接使用了,比如htmlpaser 
                        return true;
                    }
                }
            }
            catch (Exception e)
            {

                //WriteLog("爬失败");
                Console.WriteLine($"{e.ToString()}");
                return false;
            }
        }

        private static int co = 0;
        public static string RandomBrowserUa()
        {

            string[] ua = new string[] {
                "Lynx/2.8.5rel.1 libwww-FM/2.14 SSL-MM/1.4.1 GNUTLS/1.2.9",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
                "Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.9.168 Version/11.50",
                "Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11",
            };
            //Random rd = new Random();
            //int index = rd.Next(0, ua.Length);
            //return ua[index];
            if (co >= ua.Length)
                co = 0;
            return ua[co++];
        }

        private void TboxVisitCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        public void WriteLog(string text)
        {

            Console.WriteLine($"{text}");
            TboxLog.AppendText($"{text}\r\n");
            TboxLog.ScrollToEnd();

        }
    }
}
