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
        private FileListPage _unallocatedPage;
        private FileListPage _savedPage;
        private FileListPage _deletedPage;
        private PreviewPage _previewPage;
        public MainWindow()
        {
            InitializeComponent();
            App.Current.SZM.MainWindow = this;
            _unallocatedPage = new FileListPage(GetPageModel("未分配文件", App.Current.SZM.ProjectHandler.UnallocatedList));
            _savedPage = new FileListPage(GetPageModel("选中文件", App.Current.SZM.ProjectHandler.SavedList));
            _deletedPage = new FileListPage(GetPageModel("待删除文件", App.Current.SZM.ProjectHandler.DeletedList));
            _previewPage = new PreviewPage();

            FrmUnallocated.Navigate(_unallocatedPage);
            FrmSaved.Navigate(_savedPage);
            FrmDeleted.Navigate(_deletedPage);
            FrmPreview.Navigate(_previewPage);
            InputMethod.SetIsInputMethodEnabled(this, false);
            InputMethod.SetPreferredImeState(this, InputMethodState.Off);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    var page = (PreviewPage)FrmPreview.Content;
                    page.OnKeyDown(e.Key);
                    break;
                case Key.A:
                    var index = _unallocatedPage.SelectedIndex;
                    if (index != -1)
                    {
                        App.Current.SZM.ProjectHandler.MoveToSaved(index);
                    }
                    break;
                case Key.D:
                    var index2 = _unallocatedPage.SelectedIndex;
                    if (index2 != -1)
                    {
                        App.Current.SZM.ProjectHandler.MoveToDeleted(index2);
                    }
                    break;
            }
            e.Handled = true;
        }

        private FileListPageModel GetPageModel(string title, IList<ImageItemEtt> imageItemEtts)
        {
            var model = new FileListPageModel(imageItemEtts as ObservableCollection<ImageItemEtt>);
            model.Title = title;
            return model;
        }

        public void Preview(ImageItemEtt imageItemEtt)
        {
            _previewPage.Import(imageItemEtt);
        }

    }
}
