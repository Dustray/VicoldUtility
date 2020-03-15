using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ProxyAccess.Core
{
    public class IPPoolCreator
    {
        private List<ProxyIPEtt> _proxyIPList ;

        private int _proxyIPIndex = 0;

        private IPRequireQueueProvider _provider;


        public IPPoolCreator( IPRequireQueueProvider provider)
        {
            _provider = provider;
        }


        public  void CreatorStart()
        {
            new Task(async () =>
            {
                while (_provider?.IsStarting ?? false)
                {
                    lock (_provider?.Locker)
                    {
                        if (_provider?.CheckQueueResource() ?? false)
                        {
                            _provider?.QueueResourceComplement(CreatorResourceGetter());
                        }

                    }
                    await Task.Delay(200);
                }
            }).Start();
        }

        private List<ProxyIPEtt> CreatorResourceGetter()
        {
            var newList = new List<ProxyIPEtt>();
            for (var i = 0; i < 50; i++)
            {
                if (_proxyIPIndex >= _proxyIPList.Count)
                {
                    //Alert.Show("资源不足");
                    break;
                }
                newList.Add(_proxyIPList[_proxyIPIndex++]);
            }
            return newList;
        }
    }
}
