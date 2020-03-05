using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ResourceMonitor.Entities
{
    public class GPUCoreEtt
    {
        public float CoreClock { get; set; }
        public float CoreTemperature { get; set; }
        public float CoreLoad { get; set; }
    }
}
