using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ResourceMonitor.Entities
{
    public class GPUCoreEtt
    {
        public float CoreLoad { get; set; } = Properties.Settings.Default.InvalidValue;
        public float CoreClock { get; set; } = Properties.Settings.Default.InvalidValue;
        public float CoreTemperature { get; set; } = Properties.Settings.Default.InvalidValue;
    }
}
