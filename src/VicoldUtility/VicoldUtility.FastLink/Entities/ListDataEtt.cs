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
        private string _tint;
        /// <summary>
        /// 提示
        /// </summary>
        public string Tint { get { return string.IsNullOrEmpty(_tint) ? Display : _tint; } set { _tint = value; } }
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
        public string SignalTime { get; set; } = "6666ms";
    }
}
