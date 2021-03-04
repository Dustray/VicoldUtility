using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace VicoldUtility.PhotoSelector.Views
{
    /// <summary>
    /// MenuPage.xaml 的交互逻辑
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void MnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var isOpen = OpenFolder(out string folder);
            if (isOpen)
            {

            }
        }

        private bool OpenFolder(out string folder)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "打开图片集文件夹";
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Personal;
            folderBrowserDialog.ShowDialog();
            if (folderBrowserDialog.SelectedPath == string.Empty)
            {
                folder = null;
                return false;
            }
            folder = folderBrowserDialog.SelectedPath;
            return true;
        }
    }
}
