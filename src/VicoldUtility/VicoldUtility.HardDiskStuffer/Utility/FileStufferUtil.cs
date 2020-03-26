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
    public class FileStufferUtil
    {
        private string lastFilePath = @"StufferFolder\";
        private bool _isStarting = false;
        private List<DriveShowEtt> _driveNumList;
        private static CancellationTokenSource _cts;
        public FileStufferUtil(List<DriveShowEtt> driveNumList)
        {
            _driveNumList = driveNumList;
        }

        public void Start()
        {
            if (_isStarting) return;
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            _cts = new CancellationTokenSource();
            var ct = _cts.Token;
            new Task(() =>
            {
                _isStarting = true;
                foreach (var num in _driveNumList)
                {
                    if (!_isStarting) break;
                    if (!num.IsChosen || !num.IsEnable) continue;
                    try
                    {
                        DoStufferInDrive(num.Number, ct);
                    }
                    catch (OperationCanceledException)
                    {

                    }
                }
                _isStarting = false;
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
                var folderPath = $"{driveNum}{lastFilePath}";
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var fPath = Path.Combine(folderPath, Guid.NewGuid().ToString("N"));
                try
                {
                    // File.AppendAllText(fPath, content.ToString());
                    FileUtil.WriteByteWithEmpty(fPath, 1024L * 1024 * 1);
                    cToken.ThrowIfCancellationRequested();
                }
                catch (IOException)
                {
                    var lastSize = DriveUtil.GetDriveFree(driveNum);
                    try
                    {
                        FileUtil.WriteByteWithEmpty(fPath, lastSize);
                    }
                    catch { }
                    break;
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
            _isStarting = !_isStarting;
        }
    }
}
