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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VicoldUtility.FastTool.Entities;

namespace VicoldUtility.FastTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ItemEtt> DataSource;
        public MainWindow()
        {
            InitializeComponent();
            DataSource = new ObservableCollection<ItemEtt>();
            aaa.ItemsSource = DataSource;
            InitData();
        }

        private void InitData()
        {
            var forderPath = System.IO.Path.GetFullPath("Tools");
            if (!Directory.Exists(forderPath)) return;
            string[] fileSystemEntries = Directory.GetFileSystemEntries(forderPath);
            for (int i = 0; i < fileSystemEntries.Length; i++)
            {
                string file = fileSystemEntries[i];
                var extension = System.IO.Path.GetExtension(file);
                if (extension != ".bat" && extension != ".exe") continue;
                if (File.Exists(file))
                {
                    DataSource.Add(new ItemEtt()
                    {
                        Content = System.IO.Path.GetFileNameWithoutExtension(file),
                        FilePath = file
                    });
                }
            }
        }

        private void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (null == btn) return;
            var file = btn.Tag.ToString();
            if (!File.Exists(file)) return;
            var exep = new Process();
            exep.StartInfo.FileName = file;
            exep.EnableRaisingEvents = true;
            exep.Exited += new EventHandler(exep_Exited);
            exep.Start();


        }

        private void exep_Exited(object sender, EventArgs e)
        {
            
        }
    }
}
