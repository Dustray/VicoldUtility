using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ProxyAccess.Core
{
    public class IPSender
    {
        private List<string> _userAgentList = new List<string>();
        public IPSender(List<string> agentlist)
        {
            _userAgentList = agentlist;
    }
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="targetAddress"></param>
        /// <returns></returns>
        public bool ExcuteRequest(string ip, string port, string targetAddress)
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
            var ran = new Random();

            //if (_userAgentIndex >= _userAgentList.Count)
            //    _userAgentIndex = 0;
            return _userAgentList[ran.Next(_userAgentList.Count - 1)/*_userAgentIndex++*/];
        }
    }
}
