using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ResourceMonitor.Entities
{
    public class GPUEtt
    {
        public string Name { get; set; }
        public GPUCoreEtt Core { get; set; }
        public GPUMemoryEtt Memory { get; set; }
        public GPUFanEtt Fan { get; set; }
    }
}
