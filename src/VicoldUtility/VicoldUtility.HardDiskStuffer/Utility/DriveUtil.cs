using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        public static ObservableCollection<DriveShowEtt> Ett2EttShow(List<DriveEtt> etts)
        {
            var newList = new ObservableCollection<DriveShowEtt>();
            foreach (var ett in etts)
            {
                newList.Add(new DriveShowEtt()
                {
                    Number = ett.Number,
                    Name = ett.Name,
                    TotalSize = (float)Math.Round((float)ett.TotalSize / 1024 / 1024 / 1024, 2, MidpointRounding.AwayFromZero),
                    FreeSize = (float)Math.Round((float)ett.FreeSize / 1024 / 1024 / 1024, 2, MidpointRounding.AwayFromZero),
                    IsEnable = ett.IsEnable,
                    StufferSpeed = (float)Math.Round((float)ett.StufferSpeed / 1024 / 1024, 1, MidpointRounding.AwayFromZero),
                });
            }
            return newList;
        }

        /// <summary>
        /// 比较并调整新驱动器列表（计算存储改变速度）
        /// </summary>
        /// <param name="newList"></param>
        /// <param name="oldList"></param>
        /// <returns></returns>
        public static bool CompareNewDriveList(List<DriveEtt> newList, List<DriveEtt> oldList)
        {
            bool isDriveListChange = false;
            foreach (var newEtt in newList)
            {
                var oldEtt = oldList.Find(a => a.Number == newEtt.Number);
                if (null == oldEtt)
                {
                    isDriveListChange = true;
                    continue;
                }
                newEtt.StufferSpeed = oldEtt.FreeSize - newEtt.FreeSize;
            }
            if (newList.Count != oldList.Count) isDriveListChange = true;
            return isDriveListChange;
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
