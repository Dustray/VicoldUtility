using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.AvatarMaker.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<object> Pages { get; } = new ObservableCollection<object>();
        public object ToolbarViewModel { get; set; }
        public object StatusbarViewModel { get; set; }

        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPage))); }
        }
    }
}
