using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ResourceMonitor.Entities
{
    public class CPUEtt
    {
        /// <summary>
        /// 核心列表*
        /// </summary>
        public Dictionary<string,CPUCoreEtt> CoreDic { get; set; }
        /// <summary>
        /// 总利用率*
        /// </summary>
        public float TotalLoad { get; set; }
        /// <summary>
        /// 核心功耗*
        /// </summary>
        public float CorePower { get; set; }
        /// <summary>
        /// 整体功耗*
        /// </summary>
        public float PackagePower { get; set; }
        /// <summary>
        /// 图形功耗
        /// </summary>
        public float GraphicsPower { get; set; }
        /// <summary>
        /// DRAM功耗
        /// </summary>
        public float DRAMPower { get; set; }
        /// <summary>
        /// 整体温度*
        /// </summary>
        public float PackageTemperature { get; set; }
        /// <summary>
        /// 电荷耦合器件温度列表
        /// </summary>
        public Dictionary<string, float> CCDTemperatureDic { get; set; }
    }
}
