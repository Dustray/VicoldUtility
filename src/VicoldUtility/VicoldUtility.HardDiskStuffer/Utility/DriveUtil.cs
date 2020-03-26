using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VicoldUtility.HardDiskStuffer.Entities;

namespace VicoldUtility.HardDiskStuffer.Utility
{
    public static class DriveUtil
    {
        public static List<DriveEtt> GetDriveList()
        {
            var driveList = new List<DriveEtt>();
            var drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                var d = new DriveEtt();
                d.Number = drive.Name;
                d.IsEnable = drive.IsReady;
                if (drive.IsReady)
                {
                    d.Name = string.IsNullOrEmpty(drive.VolumeLabel) ? "本地磁盘" : drive.VolumeLabel;
                    d.TotalSize = drive.TotalSize;
                    d.FreeSize = drive.AvailableFreeSpace;
                }
                driveList.Add(d);
            }
            return driveList;
        }

        public static List<DriveShowEtt> Ett2EttShow(List<DriveEtt> etts)
        {
            var newList = new List<DriveShowEtt>();
            foreach (var ett in etts)
            {
                newList.Add(new DriveShowEtt()
                {
                    Number = ett.Number,
                    Name = ett.Name,
                    TotalSize = (float)Math.Round((float)ett.TotalSize / 1024 / 1024 / 1024, 2, MidpointRounding.AwayFromZero),
                    FreeSize = (float)Math.Round((float)ett.FreeSize / 1024 / 1024 / 1024, 2, MidpointRounding.AwayFromZero),
                    IsEnable = ett.IsEnable
                });
            }
            return newList;
        }

        /// <summary>
        /// 查询驱动器空闲空间
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static long GetDriveFree(string num)
        {
            var driveList = new List<DriveEtt>();
            var drives = DriveInfo.GetDrives();
            var dr = drives.Where(a => a.Name == num).First();
            if (null == dr) return 0;
            return dr.AvailableFreeSpace;
        }
    }
}
