using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.HardDiskStuffer.Entities
{
    public class DriveShowEtt
    {
        /// <summary>
        /// 驱动器号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 驱动器名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 总空间
        /// </summary>
        public float TotalSize { get; set; }
        /// <summary>
        /// 可用空间
        /// </summary>
        public float FreeSize { get; set; }
        /// <summary>
        /// 驱动器可用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChosen { get; set; } = false;
    }
}
