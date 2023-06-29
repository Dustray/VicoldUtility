using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VicoldUtility.FlashAutoBackup.FlashShriek
{
    /// <summary>
    /// 声明一个委托,用于代理一系列自定义方法
    /// </summary>
    public delegate void FlashDiskDelegate(string[] ReturnValue);
    /// <summary>
    /// U盘检测 
    /// </summary>
    internal class DiskListener
    {
        /// <summary>
        /// 声明一个绑定于上行所定义的委托的事件
        /// </summary>
        public event FlashDiskDelegate FlashDiskEvent;
        private const int WmDeviceChange = 0x219;//U盘插入后，OS的底层会自动检测到，然后向应用程序发送“硬件设备状态改变“的消息
        private const int DbtDeviceArrival = 0x8000;  //就是用来表示U盘可用的。一个设备或媒体已被插入一块，现在可用。
        private const int DbtConfigChangeCanceled = 0x0019;  //要求更改当前的配置（或取消停靠码头）已被取消。
        private const int DbtConfigchanged = 0x0018;  //当前的配置发生了变化，由于码头或取消固定。
        private const int DbtCustomEvent = 0x8006; //自定义的事件发生。 的Windows NT 4.0和Windows 95：此值不支持。
        private const int DbtDeviceQueryRemove = 0x8001;  //审批要求删除一个设备或媒体作品。任何应用程序也不能否认这一要求，并取消删除。
        private const int DbtDeviceQueryRemoveFailed = 0x8002;  //请求删除一个设备或媒体片已被取消。
        private const int DbtDeviceRemoveComplete = 0x8004;  //一个设备或媒体片已被删除。
        private const int DbtDeviceRemovePending = 0x8003;  //一个设备或媒体一块即将被删除。不能否认的。
        private const int DbtDeviceTypeSpecific = 0x8005;  //一个设备特定事件发生。
        private const int DbtDevNodesChanged = 0x0007;  //一种设备已被添加到或从系统中删除。
        private const int DbtQueryChangeConfig = 0x0017;  //许可是要求改变目前的配置（码头或取消固定）。
        private const int DbtUserDefined = 0xFFFF;  //此消息的含义是用户定义的
        public string[] GetRemovableDrivers(int msg, IntPtr wParam)
        {
            try
            {
                if (msg != WmDeviceChange)
                {
                    return null;
                }
                switch (wParam.ToInt32())
                {
                    case WmDeviceChange:
                        break;
                    case DbtDeviceArrival://检测到U盘插入
                        {
                            var driveInfos = DriveInfo.GetDrives();
                            var flashDisks = from driveInfo in driveInfos
                                             where driveInfo.DriveType == DriveType.Removable
                                             select driveInfo.Name;
                            return flashDisks.ToArray();
                        }
                    case DbtDeviceRemoveComplete: //检测到U盘拔出
                        {
                            Trace.Write("U盘拔出");
                            var driveInfos = DriveInfo.GetDrives();
                            var flashDisks = from driveInfo in driveInfos
                                             where driveInfo.DriveType == DriveType.Removable
                                             select driveInfo.Name;
                            if (FlashDiskEvent != null)
                            {
                                FlashDiskEvent(flashDisks.ToArray());//先隐藏图标，在判定是否还有U盘存在
                            }
                            return flashDisks.ToArray();
                        }
                    case DbtConfigChangeCanceled:
                        break;
                    case DbtConfigchanged:
                        break;
                    case DbtCustomEvent:
                        break;
                    case DbtDeviceQueryRemove:
                        break;
                    case DbtDeviceQueryRemoveFailed:
                        break;
                    case DbtDeviceRemovePending:
                        break;
                    case DbtDeviceTypeSpecific:
                        break;
                    case DbtQueryChangeConfig:
                        break;
                    case DbtUserDefined:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }
    }
}
