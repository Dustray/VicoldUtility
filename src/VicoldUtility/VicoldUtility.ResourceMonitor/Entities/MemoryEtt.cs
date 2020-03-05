using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ResourceMonitor.Entities
{
    public class MemoryEtt
    {
        public float MemoryLoad { get; set; } = Properties.Settings.Default.InvalidValue;
        public float MemoryUsed { get; set; } = Properties.Settings.Default.InvalidValue;
        public float MemoryFree { get; set; } = Properties.Settings.Default.InvalidValue;

    }
}
