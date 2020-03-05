using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ResourceMonitor.Entities
{
    public class GPUMemoryEtt
    {
        public float MemoryClock { get; set; }
        public float MemoryLoad { get; set; }
        public float MemoryTemperature { get; set; }
        public float MemoryTotal { get; set; }
    }
}
