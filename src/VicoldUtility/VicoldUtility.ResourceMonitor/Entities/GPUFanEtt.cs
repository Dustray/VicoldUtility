﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ResourceMonitor.Entities
{
    public class GPUFanEtt
    {
        public float FanLoad { get; set; } = Properties.Settings.Default.InvalidValue;
        public float FanSpeed { get; set; } = Properties.Settings.Default.InvalidValue;
    }
}
