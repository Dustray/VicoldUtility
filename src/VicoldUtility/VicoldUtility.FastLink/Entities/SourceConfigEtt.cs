using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.FastLink.Entities
{
    public class SourceConfigEtt
    {
        public IList<SourceConfigGroupEtt> Groups { get; set; }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SourceConfigLinkEtt FindLink(string id)
        {
            foreach (var group in Groups)
            {
                foreach (var link in group.Links)
                {
                    if (link.ID == id)
                    {
                        return link;
                    }
                }
            }
            return null;
        }
    }
}