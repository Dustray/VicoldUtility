using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VicoldUtility.PhotoSelector.Entities;
using VicoldUtility.PhotoSelector.Views;

namespace VicoldUtility.PhotoSelector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.Current.SZM.MainWindow = this;
            var _unallocatedPage = new FileListPage(GetPageModel("未分配文件",App.Current.SZM.ProjectHandler.UnallocatedList));
            var _savedPage = new FileListPage(GetPageModel("选中文件",App.Current.SZM.ProjectHandler.SavedList));
            var _deletedPage = new FileListPage(GetPageModel("待删除文件",App.Current.SZM.ProjectHandler.DeletedList));

            FrmUnallocated.Navigate(_unallocatedPage);
            FrmSaved.Navigate(_savedPage);
            FrmDeleted.Navigate(_deletedPage);

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                var page = (PreviewPage)PreviewPage.Content;
                page.OnKeyDown(e.Key);
            }
        }

        private FileListPageModel GetPageModel(string title , IList<ImageItemEtt> imageItemEtts)
        {
            var model = new FileListPageModel(imageItemEtts as ObservableCollection<ImageItemEtt>);
            model.Title = title;
            return model;
        }

        public void SetImage(BitmapImage im)
        {
            ds.Source = im;
        }
    }
}
