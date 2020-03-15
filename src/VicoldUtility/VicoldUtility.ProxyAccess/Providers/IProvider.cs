using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VicoldUtility.ProxyAccess.Core;

namespace VicoldUtility.ProxyAccess
{
    internal interface IProvider:IDisposable
    {
        bool IsStarting { get; set; }
        IPSender Sender { get; set; }
        Action<int, int, int> ExecuteCountCallback { get; set; }
        Action<bool, string> LogCallback { get; set; }
        Action OnStartCallback { get; set; }
        Action OnStoppedCallback { get; set; }
        void AddResource(List<ProxyIPEtt> proxyIPList, List<string> userAgentList);
        void Start();
        void Stop();
    }
}
