using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ResourceMonitor.Entities
{
    public class GPUMemoryEtt
    {
        public float MemoryLoad { get; set; }
        public float MemoryClock { get; set; }
        public float MemoryTemperature { get; set; } = Properties.Settings.Default.InvalidValue;
        public float MemoryTotal { get; set; } = Properties.Settings.Default.InvalidValue;
    }
}
