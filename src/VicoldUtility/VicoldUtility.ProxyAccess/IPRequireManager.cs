using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ProxyAccess
{
    public class IPRequireManager : IDisposable
    {
        public bool IsStarting = false;

        private List<ProxyIPEtt> _proxyIPList = new List<ProxyIPEtt>();
        private List<string> _userAgentList = new List<string>();

        private int _userAgentIndex = 0;
        private Action<int, int, int> _executeCountCallback;
        private Action<bool, string> _logCallback;
        private int _executeSuccessCount = 0;
        private int _executeDidCount = 0;
        private int _executeAllCount = 0;
        private IPRequireMod _iPRequireMod;
        ParallelLoopState _state; 

        public IPRequireManager(IPRequireMod mod)
        {
            _iPRequireMod = mod;
        }

        /// <summary>
        /// 追加资源
        /// </summary>
        /// <param name="proxyIPList"></param>
        /// <param name="userAgentList"></param>
        public void AddResource(List<ProxyIPEtt> proxyIPList, List<string> userAgentList)
        {
            _proxyIPList.AddRange(proxyIPList);
            _executeAllCount = _proxyIPList.Count;
            _userAgentList.AddRange(userAgentList);
        }
        /// <summary>
        /// 追加资源
        /// </summary>
        /// <param name="proxyIPList"></param>
        public void AddResource(List<ProxyIPEtt> proxyIPList)
        {
            _proxyIPList.AddRange(proxyIPList);
            _executeAllCount = _proxyIPList.Count;
        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Start(string address)
        {
            if (_proxyIPList.Count == 0 || _userAgentList.Count == 0) return;
            if (IsStarting) return;
            IsStarting = true;

            new Task(() =>
            {
                Parallel.For(0, _proxyIPList.Count, (k, state) =>
                {
                    _state = state;
                    if (!IsStarting)
                    {
                        state.Break();
                        return;
                    }
                    var ett = _proxyIPList[k];
                    var re = ExcuteRequest(ett.IP, ett.Port, address);

                    if (re) _executeSuccessCount++;
                    _executeDidCount++;
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (_iPRequireMod.IsExecuteOver(_executeSuccessCount, _executeDidCount))
                        {
                            state.Break();
                        }
                        //if (_executeDidCount == _executeAllCount)
                        //{
                        //    AddResource();
                        //}
                        _logCallback?.Invoke(re, $"{ett.IP}:{ett.Port}");
                        _executeCountCallback?.Invoke(_executeSuccessCount, _executeDidCount, _executeAllCount);
                    });
                     //if (!IsStarting) state.Stop();
                });
            }).Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            IsStarting = false;
            //_state?.Break();
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            IsStarting = false;
            //_state?.Break();
            _proxyIPList.Clear();
            _userAgentList.Clear();
            _userAgentIndex = 0;
            _executeSuccessCount = 0;
            _executeDidCount = 0;
            _executeAllCount = 0;
        }

        /// <summary>
        /// 设置执行数据统计回调方法
        /// </summary>
        /// <param name="executeCountCallback"></param>
        public void SetExecuteCountCallback(Action<int, int, int> executeCountCallback)
        {
            _executeCountCallback = executeCountCallback;
        }

        /// <summary>
        /// 设置日志回调方法
        /// </summary>
        /// <param name="logCallback"></param>
        public void SetLogCallback(Action<bool, string> logCallback)
        {
            _logCallback = logCallback;
        }

        #region 私有方法
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="targetAddress"></param>
        /// <returns></returns>
        private bool ExcuteRequest(string ip, string port, string targetAddress)
        {
            int ports;
            if (!int.TryParse(port, out ports)) return false;
            WebProxy proxyObject = new WebProxy(ip, ports);//str为IP地址 port为端口号 代理类
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(targetAddress); // 访问这个网站 ，返回的就是你发出请求的代理ip 这个做代理ip测试非常方便，可以知道代理是否成功
            Req.UserAgent = GetUserAgent();
            Req.Proxy = proxyObject; //设置代理
            Req.Method = "GET";
            try
            {
                using (HttpWebResponse Resp = (HttpWebResponse)Req.GetResponse())
                {

                    Encoding code = Encoding.GetEncoding("UTF-8");
                    using (StreamReader sr = new StreamReader(Resp.GetResponseStream(), code))
                    {
                        //var str = sr.ReadToEnd();//获取得到的网址html返回数据，这里就可以使用某些解析html的dll直接使用了,比如htmlpaser 
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 获取下一个用户代理字段
        /// </summary>
        /// <returns></returns>
        private string GetUserAgent()
        {
            if (_userAgentIndex >= _userAgentList.Count)
                _userAgentIndex = 0;
            return _userAgentList[_userAgentIndex++];
        }


        #endregion

        public void Dispose()
        {
            Clear();
        }
    }
}
