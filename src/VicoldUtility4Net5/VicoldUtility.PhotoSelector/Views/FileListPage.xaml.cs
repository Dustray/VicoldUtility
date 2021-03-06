using System.Windows.Controls;
using VicoldUtility.PhotoSelector.Entities;

namespace VicoldUtility.PhotoSelector.Views
{
    /// <summary>
    /// ListPage.xaml 的交互逻辑
    /// </summary>
    public partial class FileListPage : Page
    {
        public FileListPageModel ViewModel;
        public FileListPage(FileListPageModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            this.DataContext = ViewModel;
        }
    }
}
