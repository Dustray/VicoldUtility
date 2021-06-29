using System.Windows.Controls;
using VicoldUtility.PhotoSelector.Entities;

namespace VicoldUtility.PhotoSelector.Views
{
    /// <summary>
    /// ListPage.xaml 的交互逻辑
    /// </summary>
    public partial class FileListPage : Page
    {
        private FileListPageModel _viewModel;

        public FileListPage(FileListPageModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        public int SelectedIndex
        {
            get
            {
                var currentSelect = ListFile.SelectedIndex;
                if (_viewModel.FileList.Count > 0)
                {
                    if (currentSelect == _viewModel.FileList.Count - 1)
                    {
                        ListFile.SelectedIndex--;
                    }
                    else
                    {
                        ListFile.SelectedIndex++;
                    }
                }

                return currentSelect;
            }
        }



        private void ListFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ett = ListFile.SelectedItem as ImageItemEtt;
            if (ett != null)
            {
                App.Current.SZM.Preview(ett);
            }
        }
    }
}
