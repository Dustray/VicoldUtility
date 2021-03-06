

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VicoldUtility.PhotoSelector.Entities
{
    public sealed class FileListPageModel : BaseProperty
    {
        private ObservableCollection<ImageItemEtt> _fileList;
        private string _title;

        public FileListPageModel(ObservableCollection<ImageItemEtt> fileList)
        {
            _fileList = fileList;
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public ObservableCollection<ImageItemEtt> FileList
        {
            get
            {
                return _fileList;
            }
            set
            {
                _fileList = value;
                OnPropertyChanged("FileList");
            }
        }


    }
}
