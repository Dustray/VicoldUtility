﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.FastLink.Entities
{
    public class SourceConfigLinkEtt
    {
        public SourceConfigLinkEtt()
        {
            ID = Guid.NewGuid().ToString("N");
        }
        internal string ID { get; set; }
        private string _display;
        /// <summary>
        /// 显示
        /// </summary>
        public string Display { get { return string.IsNullOrEmpty(_display) ? Url : _display; } set { _display = value; } }

        private string _tint;
        /// <summary>
        /// 提示
        /// </summary>
        public string Tint { get { return string.IsNullOrEmpty(_tint) ? Url : _tint; } set { _tint = value; } }
        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 子成员
        /// </summary>
        public IList<SourceConfigLinkEtt> Links { get; set; }

    }
}
