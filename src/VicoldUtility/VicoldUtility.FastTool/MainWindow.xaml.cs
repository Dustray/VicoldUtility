using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VicoldUtility.FastTool.Controls;
using VicoldUtility.FastTool.Entities;

namespace VicoldUtility.FastTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            var forderPath = System.IO.Path.GetFullPath("Tools");
            CheckFolder(forderPath,"默认");
        }

        private void CheckFolder(string forderPath,string tabName=null)
        {
            if (!Directory.Exists(forderPath)) return;
            string[] fileSystemEntries = Directory.GetFileSystemEntries(forderPath);
            var folderName = System.IO.Path.GetFileName(forderPath);
            var DataSource = CreateNewListBox(tabName??folderName).DataSource;
            for (int i = 0; i < fileSystemEntries.Length; i++)
            {
                string path = fileSystemEntries[i];
                if (File.Exists(path))
                {
                    var extension = System.IO.Path.GetExtension(path);
                    if (extension != ".bat" && extension != ".exe") continue;
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    var isNeedAdmin = fileName.StartsWith("[admin]");
                    if (isNeedAdmin)
                    {
                        fileName= fileName.Replace("[admin]", "");
                    }
                    DataSource.Add(new ItemEtt()
                    {
                        Content = fileName,
                        FilePath = path,
                        IsNeedAdmin = isNeedAdmin
                    }) ;
                }
                else if (Directory.Exists(path))
                {
                    CheckFolder(path);
                }
            }
        }

        private ButtonList CreateNewListBox(string tabName)
        {
            var listBox = new ButtonList();
            var tabItem = new TabItem();
            tabItem.Header = tabName;
            tabItem.Content = listBox;
            TabMain.Items.Add(tabItem);
            return listBox;
        }



    }
}
