using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VicoldUtility.HardDiskStuffer.Entities;

namespace VicoldUtility.HardDiskStuffer.Utility
{
    public class FileStufferUtil:IDisposable
    {
        private const string _lastFilePath = @"StufferFolder\";
        private bool _isStarting = false;
        private static CancellationTokenSource _cts;

        public  List<DriveShowEtt> DriveNumList;
        public bool IsDeleteAfterStuffer = true;
        public Action OnStarted;
        public Action OnStopped;
        public Action<string> OnDriveStart;
        public Action<string> OnDriveComplete;
        public Action<string, bool> OnDeleteStufferFile;
        public FileStufferUtil(List<DriveShowEtt> driveNumList)
        {
            DriveNumList = driveNumList;
        }

        public void Start()
        {
            if (_isStarting|| DriveNumList.Count == 0) return;
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            _cts = new CancellationTokenSource();
            var ct = _cts.Token;
            new Task(() =>
            {
                OnStarted?.Invoke();
                _isStarting = true;
                foreach (var num in DriveNumList)
                {
                    if (!_isStarting) break;
                    if (!num.IsChosen || !num.IsEnable) continue;
                    OnDriveStart?.Invoke(num.Number);
                    try
                    {
                        DoStufferInDrive(num.Number, ct);
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    OnDriveComplete?.Invoke(num.Number);
                }
                _isStarting = false;
                OnStopped?.Invoke();
            }).Start();
        }
        public void Stop()
        {
            _isStarting = false;
            _cts?.Cancel();
        }


        private void DoStufferInDrive(string driveNum, CancellationToken cToken)
        {
            while (_isStarting)
            {
                var folderPath = $"{driveNum}{_lastFilePath}";
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var fPath = Path.Combine(folderPath, Guid.NewGuid().ToString("N"));
                try
                {
                    // File.AppendAllText(fPath, content.ToString());
                    FileUtil.WriteByteWithEmpty(fPath, 1024L * 1024 * 50);
                    cToken.ThrowIfCancellationRequested();
                }
                catch (IOException e)
                {//填充完后必进
                    var lastSize = DriveUtil.GetDriveFree(driveNum);
                    if (lastSize < 9)
                    {//完整做完

                    }
                    else
                    {
                        try
                        {
                            fPath = Path.Combine(folderPath, Guid.NewGuid().ToString("N"));
                            FileUtil.WriteByteWithEmpty(fPath, lastSize - 8);
                        }
                        catch (Exception es) { }
                    }
                    OnDriveComplete?.Invoke(driveNum);
                    if (IsDeleteAfterStuffer) DoDeleteInDrive(driveNum);
                    break;
                }
            }
        }

        /// <summary>
        /// 删除所有或指定驱动器的填充文件
        /// </summary>
        /// <param name="driveNum"></param>
        public void DoDeleteInDrive(string driveNum = null)
        {
            if (null != driveNum)
            {
                doDelete(driveNum);
            }
            else
            {
                foreach (var num in DriveNumList)
                {
                    if (!num.IsChosen || !num.IsEnable) continue;
                    doDelete(num.Number);
                }
            }
            void doDelete(string num)
            {
                var fPath = $"{num}{_lastFilePath}";
                try
                {
                    FileUtil.DeleteFolder(fPath);
                    OnDeleteStufferFile?.Invoke(num, true);
                }
                catch
                {
                    OnDeleteStufferFile?.Invoke(num, false);
                }
            }
        }

        public void StartOrStop()
        {
            if (_isStarting)
            {
                Stop();
            }
            else
            {
                Start();
            }
            //_isStarting = !_isStarting;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
