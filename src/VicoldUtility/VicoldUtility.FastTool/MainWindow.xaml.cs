using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VicoldUtility.FastTool.Controls;
using VicoldUtility.FastTool.Entities;

namespace VicoldUtility.FastTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] _canExecuteExtension = new string[] { ".bat", ".exe", ".msi", "reg", "msc" };
        public MainWindow()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            var forderPath = System.IO.Path.GetFullPath("Tools");
            CheckFolder(forderPath, "默认");
        }

        private void CheckFolder(string forderPath, string tabName = null)
        {
            if (!Directory.Exists(forderPath)) return;
            string[] fileSystemEntries = Directory.GetFileSystemEntries(forderPath);
            var folderName = System.IO.Path.GetFileName(forderPath);
            var ettList = new ObservableCollection<ItemEtt>();
            for (int i = 0; i < fileSystemEntries.Length; i++)
            {
                string path = fileSystemEntries[i];
                if (File.Exists(path))
                {
                    var extension = System.IO.Path.GetExtension(path);
                    if (Array.IndexOf(_canExecuteExtension, extension.ToLower()) == -1) continue;
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    var isNeedAdmin = fileName.StartsWith("[admin]");
                    if (isNeedAdmin)
                    {
                        fileName = fileName.Replace("[admin]", "");
                    }
                    ettList.Add(new ItemEtt()
                    {
                        Content = fileName,
                        FilePath = path,
                        IsNeedAdmin = isNeedAdmin
                    });
                }
                else if (Directory.Exists(path))
                {
                    CheckFolder(path);
                }
            }
            CreateNewListBox(tabName ?? folderName, ettList);
        }

        private void CreateNewListBox(string tabName, ObservableCollection<ItemEtt> content)
        {
            if (0 == content.Count) return;
            var listBox = new ButtonList(WriteLog);
            foreach (var con in content)
            {
                listBox.DataSource.Add(con);
            }
            var tabItem = new TabItem();
            tabItem.Header = tabName;
            tabItem.Content = listBox;
            TabMain.Items.Add(tabItem);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg"></param>
        public void WriteLog(string msg)
        {
            tboxLog.AppendText($"{DateTime.Now:yy-MM-dd HH:mm:ss}：{msg}\r\n");
            tboxLog.ScrollToEnd();
        }

    }
}
