using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VicoldUtility.ProxyAccess.Core;

namespace VicoldUtility.ProxyAccess
{
    public class IPRequireQueueProvider :IProvider
    {
        public bool IsStarting { get; set; } = false;
        public Queue<ProxyIPEtt> ProxyIPQueue { get; private set; } = new Queue<ProxyIPEtt>();
        public object Locker { get; private set; } = new object();
        public IPSender Sender { get; set; }

        private static CancellationTokenSource _cts;

        private string _address = "";

        private int _userAgentIndex = 0;
        public Action<int, int, int> ExecuteCountCallback { get; set; }
        public Action<bool, string> LogCallback { get; set; }
        public Action OnStartCallback { get; set; }
        public Action OnStoppedCallback { get; set; }
        private int _executeSuccessCount = 0;
        private int _executeDidCount = 0;
        private int _executeAllCount = 0;
        private int _threadCount = 0;
        private int _successThreadCount = 0;

        private IPRequireMod _iPRequireMod;
        ParallelLoopState _state;

        public IPRequireQueueProvider(string address, IPRequireMod mod)
        {
            _address = address;
            _iPRequireMod = mod;
        }

        #region 资源

        /// <summary>
        /// 追加资源
        /// </summary>
        /// <param name="proxyIPList"></param>
        /// <param name="userAgentList"></param>
        public void AddResource(List<ProxyIPEtt> proxyIPList, List<string> userAgentList)
        {
            QueueResourceComplement(proxyIPList);
            Sender = new IPSender(userAgentList);
        }
        ///// <summary>
        ///// 追加资源
        ///// </summary>
        ///// <param name="proxyIPList"></param>
        //public void AddResource(List<ProxyIPEtt> proxyIPList)
        //{
        //    _proxyIPList.AddRange(proxyIPList);
        //    _executeAllCount = _proxyIPList.Count;
        //}

        /// <summary>
        /// 检查队列资源是否需要补充
        /// </summary>
        /// <param name="proxyIPList"></param>
        public bool CheckQueueResource()
        {
            lock (Locker)
            {
                return ProxyIPQueue.Count < 30;
            }
        }

        /// <summary>
        /// 队列资源补充
        /// </summary>
        /// <param name="proxyIPList"></param>
        public void QueueResourceComplement(List<ProxyIPEtt> proxyIPList)
        {
            lock (Locker)
            {
                foreach (var ett in proxyIPList)
                {
                    ProxyIPQueue.Enqueue(ett);
                }
                _executeAllCount += proxyIPList.Count;
                Console.WriteLine($"补充资源数量{proxyIPList.Count}");
            }
        }

        #endregion

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            if (ProxyIPQueue.Count == 0 /*|| _userAgentList.Count == 0*/) return;
            if (IsStarting) return;
            IsStarting = true;


            if (_cts == null) _cts = new CancellationTokenSource();
            var ct = _cts.Token;
            List<Action> listTask = new List<Action>();
            _threadCount = 15;
            for (int i = 0; i < 15; i++)
            {
                new Task(new Action(() => ParallelRun(ct))).Start();
            }
            //Parallel.For(0, listTask.Count, new Action<int>(i => listTask[i].Invoke()));

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            IsStarting = false;
            //_state?.Break();
            _cts?.Cancel();
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            IsStarting = false;
            //_state?.Break();
            ProxyIPQueue.Clear();
            //_userAgentList.Clear();
            _userAgentIndex = 0;
            _executeSuccessCount = 0;
            _executeDidCount = 0;
            _executeAllCount = 0;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        #region 核心方法

        /// <summary>
        /// 异步跑和新方法
        /// </summary>
        /// <param name="cToken"></param>
        public void ParallelRun(CancellationToken cToken)
        {
            try
            {
                while (IsStarting)
                {
                    cToken.ThrowIfCancellationRequested();
                    ProxyIPEtt ett;
                    lock (Locker)
                    {
                        if (ProxyIPQueue.Count > 0)
                        {
                            ett = ProxyIPQueue.Dequeue();
                        }
                        else
                        {
                            Thread.Sleep(500);
                            continue;
                        }
                        Console.WriteLine($"资源池当前数量{ProxyIPQueue.Count}");
                    }
                    var re = Sender.ExcuteRequest(ett.IP, ett.Port, _address);

                    if (re) _executeSuccessCount++;
                    _executeDidCount++;
                    if (_iPRequireMod.IsExecuteOver(_executeSuccessCount, _executeDidCount))
                    {
                        IsStarting = false;
                        _cts?.Cancel();
                    }
                    LogCallback?.Invoke(re, $"{ett.IP}:{ett.Port}");
                    ExecuteCountCallback?.Invoke(_executeSuccessCount, _executeDidCount, _executeAllCount);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                lock (Locker)
                {
                    _successThreadCount++;
                    if (_successThreadCount == _threadCount)
                        OnStoppedCallback?.Invoke();
                }
            }
        }

        #endregion

        #region 回调

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

        /// <summary>
        /// 执行结束回调事件
        /// </summary>
        /// <param name="logCallback"></param>
        public void SetOnStoppedCallback(Action onStoppedCallback)
        {
            OnStoppedCallback = onStoppedCallback;
        }
        #endregion

        

        public void Dispose()
        {
            Clear();
        }
    }
}
