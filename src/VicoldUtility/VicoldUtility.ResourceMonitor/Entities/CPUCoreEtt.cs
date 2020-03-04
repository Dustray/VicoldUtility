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
        public float Load { get; set; }
        /// <summary>
        /// 核心频率
        /// </summary>
        public float Clock { get; set; }
        /// <summary>
        /// 核心电耗
        /// </summary>
        public float Power { get; set; }
    }
}
