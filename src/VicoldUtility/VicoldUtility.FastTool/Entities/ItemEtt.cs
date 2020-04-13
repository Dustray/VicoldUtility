using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.FastTool.Entities
{
    public class ItemEtt
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 是否需要管理权权限
        /// </summary>
        public bool IsNeedAdmin { get; set; }
    }
}
