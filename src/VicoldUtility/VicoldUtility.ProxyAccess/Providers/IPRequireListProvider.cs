using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VicoldUtility.ProxyAccess.Core;

namespace VicoldUtility.ProxyAccess
{
    public class IPRequireListProvider : IProvider
    {
        public bool IsStarting { get; set; } = false;
        public IPSender Sender { get; set; }

        private List<ProxyIPEtt> _proxyIPList = new List<ProxyIPEtt>();
        private object _locker = new object();
        private string _address = "";

        private int _userAgentIndex = 0;
        public Action<int, int, int> ExecuteCountCallback { get; set; }
        public Action<bool, string> LogCallback { get; set; }
        public Action OnStartCallback { get; set; }
        public Action OnStoppedCallback { get; set; }
        private int _executeSuccessCount = 0;
        private int _executeDidCount = 0;
        private int _executeAllCount = 0;
        private IPRequireMod _iPRequireMod;
        ParallelLoopState _state;

        public IPRequireListProvider(string address, IPRequireMod mod)
        {
            _address = address;
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
            Sender = new IPSender(userAgentList);
        }
        /// <summary>
        /// 追加资源
        /// </summary>
        /// <param name="proxyIPList"></param>
        public void AddResource(List<ProxyIPEtt> proxyIPList)
        {
            _proxyIPList.AddRange(proxyIPList);
            //_executeAllCount = _proxyIPList.Count;
        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            if (_proxyIPList.Count == 0 /*|| _userAgentList.Count == 0*/) return;
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
                    lock (_locker)
                    {
                        _executeAllCount++;
                    }
                    var ett = _proxyIPList[k];
                    var re = Sender.ExcuteRequest(ett.IP, ett.Port, _address);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (re) _executeSuccessCount++;
                        _executeDidCount++;
                        if (_iPRequireMod.IsExecuteOver(_executeSuccessCount, _executeDidCount))
                        {
                            state.Break();
                        }
                        LogCallback?.Invoke(re, $"{ett.IP}:{ett.Port}");
                        ExecuteCountCallback?.Invoke(_executeSuccessCount, _executeDidCount, _executeAllCount);
                    });
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
            //_userAgentList.Clear();
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
            ExecuteCountCallback = executeCountCallback;
        }

        /// <summary>
        /// 设置日志回调方法
        /// </summary>
        /// <param name="logCallback"></param>
        public void SetLogCallback(Action<bool, string> logCallback)
        {
            LogCallback = logCallback;
        }

        #region 私有方法



        #endregion

        public void Dispose()
        {
            Clear();
        }
    }
}
