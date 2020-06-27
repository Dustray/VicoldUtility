using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.FastLink.Entities
{
    public class SourceConfigGroupEtt
    {
        public string Display { get; set; }
        public IList<SourceConfigLinkEtt> Links { get; set; }
    }
}
