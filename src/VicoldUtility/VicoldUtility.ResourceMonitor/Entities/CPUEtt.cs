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
        public float TotalLoad { get; set; } = Properties.Settings.Default.InvalidValue;
        /// <summary>
        /// 核心功耗*
        /// </summary>
        public float CorePower { get; set; } = Properties.Settings.Default.InvalidValue;
        /// <summary>
        /// 整体功耗*
        /// </summary>
        public float PackagePower { get; set; } = Properties.Settings.Default.InvalidValue;
        /// <summary>
        /// 图形功耗
        /// </summary>
        public float GraphicsPower { get; set; } = Properties.Settings.Default.InvalidValue;
        /// <summary>
        /// DRAM功耗
        /// </summary>
        public float DRAMPower { get; set; } = Properties.Settings.Default.InvalidValue;
        /// <summary>
        /// 整体温度*
        /// </summary>
        public float PackageTemperature { get; set; } = Properties.Settings.Default.InvalidValue;
        /// <summary>
        /// 电荷耦合器件温度列表
        /// </summary>
        public Dictionary<string, float> CCDTemperatureDic { get; set; } 
    }
}
