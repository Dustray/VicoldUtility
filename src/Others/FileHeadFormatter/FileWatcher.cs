using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileHeadFormatter
{
    internal class FileWatcher : IDisposable
    {
        private FileSystemWatcher _fileWatcher;
        private string? _path = null;
        private string[] _filter_array = new string[0];
        public FileWatcher(Action<string> action)
        {
            _fileWatcher = new FileSystemWatcher();
            OnFileUpdate = action;
        }

        public bool EnableCreated { get; set; } = false;

        public bool EnableChanged { get; set; } = false;

        public string Filter { set => _filter_array = value.Split(';'); }

        public Action<string> OnFileUpdate;

        public void Start(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException();
            }

            _path = path;
            _fileWatcher.BeginInit();
            _fileWatcher.Path = path;
            _fileWatcher.Filter = "*.*";
            _fileWatcher.IncludeSubdirectories = true;
            if (EnableCreated)
                _fileWatcher.Created += Watcher_Created;
            if (EnableChanged)
                _fileWatcher.Changed += Watcher_Changed;
            if (false)
            {
                _fileWatcher.Deleted += Watcher_Deleted;
                _fileWatcher.Renamed += Watcher_Renamed;
            }

            // 开启监控
            _fileWatcher.EnableRaisingEvents = true;
            _fileWatcher.EndInit();
        }

        public void Restart(string? path = null)
        {
            Stop();
            Start(path ?? _path ?? throw new InvalidOperationException());
        }

        public void Stop()
        {
            ClearWatcher();
            _fileWatcher.Dispose();
            _fileWatcher = new FileSystemWatcher();
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string strFileExt = Path.GetExtension(e.FullPath).ToLower();
            if (_filter_array.Contains(strFileExt))
            {
                Push(e.FullPath);
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Push(e.FullPath);
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
        }

        private void Push(string path)
        {
            Task.Run(async () =>
            {
                await Task.Delay(10).ConfigureAwait(false);
                OnFileUpdate.Invoke(path);
            });
        }

        public void Dispose()
        {
            ClearWatcher();
            _fileWatcher.Dispose();
        }

        private void ClearWatcher()
        {
            _fileWatcher.EnableRaisingEvents = false;
            _fileWatcher.Created -= new FileSystemEventHandler(Watcher_Created);
            _fileWatcher.Changed -= new FileSystemEventHandler(Watcher_Changed);
            _fileWatcher.Deleted -= new FileSystemEventHandler(Watcher_Deleted);
            _fileWatcher.Renamed -= new RenamedEventHandler(Watcher_Renamed);
        }
    }
}
