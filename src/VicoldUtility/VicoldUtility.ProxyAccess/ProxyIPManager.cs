using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ProxyAccess
{
    public class ProxyIPManager
    {

        public List<ProxyIPEtt> GetProxyIPs(int page = 1)
        {
            var _proxyIPList = new List<ProxyIPEtt>();
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
            return _proxyIPList;
        }

        private bool getM(string time)
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
    }
}
