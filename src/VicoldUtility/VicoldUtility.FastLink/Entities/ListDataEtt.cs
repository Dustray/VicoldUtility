using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.FastLink.Entities
{
    internal class ListDataEtt
    {
        /// <summary>
        /// 显示
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// 提示
        /// </summary>
        public string Tint { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 标记类型颜色
        /// </summary>
        public string TagColor { get; set; } = "#fff";

        public string SignalContent { get; set; }
        public string SignalColor { get; set; }
    }
}
