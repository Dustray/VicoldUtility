using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ResourceMonitor.Entities
{
    public class CPUCoreEtt
    {
        /// <summary>
        /// 核心利用率
        /// </summary>
        public float Load { get; set; } = Properties.Settings.Default.InvalidValue;
        /// <summary>
        /// 核心频率
        /// </summary>
        public float Clock { get; set; } = Properties.Settings.Default.InvalidValue;
        /// <summary>
        /// 核心功耗
        /// </summary>
        public float Power { get; set; } = Properties.Settings.Default.InvalidValue;

        /// <summary>
        /// 核心温度
        /// </summary>
        public float Temperature { get; set; } = Properties.Settings.Default.InvalidValue;
    }
}
