using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.FastLink.Entities
{
    internal class ListDataEtt : INotifyPropertyChanged
    {
        internal string ID { get; set; }

        /// <summary>
        /// 显示
        /// </summary>
        public string Display { get; set; }
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
        /// 标记类型颜色
        /// </summary>
        public string TagColor { get; set; } = "#fff";
        /// <summary>
        /// 标记类型图标
        /// </summary>
        public string LinkTypeIcon { get; set; }
        /// <summary>
        /// 标记类型内容
        /// </summary>
        public string LinkTypeContent { get; set; }



        private string _signalContent { get; set; }
        public string SignalContent
        {
            get { return _signalContent; }
            set
            {
                _signalContent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SignalContent"));
            }
        }
        private string _signalColor { get; set; }
        public string SignalColor
        {
            get { return _signalColor; }
            set
            {
                _signalColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SignalColor"));
            }
        }
        private string _signalTime { get; set; } = "6666ms";
        public string SignalTime
        {
            get { return _signalTime; }
            set
            {
                _signalTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SignalTime"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
