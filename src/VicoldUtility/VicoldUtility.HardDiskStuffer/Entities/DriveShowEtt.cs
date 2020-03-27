using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.HardDiskStuffer.Entities
{
    public class DriveShowEtt : INotifyPropertyChanged
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
        public float _freeSize { get; set; }
        /// <summary>
        /// 可用空间
        /// </summary>
        public float FreeSize
        {
            get { return _freeSize; }
            set
            {
                _freeSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeSize"));
            }
        }
        /// <summary>
        /// 驱动器可用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool _isChosen { get; set; } = false;
        public bool IsChosen
        {
            get { return _isChosen; }
            set
            {
                _isChosen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChosen"));
            }
        }
        private float _stufferSpeed = 0;
        /// <summary>
        /// 填充速率
        /// </summary>
        public float StufferSpeed
        {
            get { return _stufferSpeed; }
            set
            {
                _stufferSpeed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StufferSpeed"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
