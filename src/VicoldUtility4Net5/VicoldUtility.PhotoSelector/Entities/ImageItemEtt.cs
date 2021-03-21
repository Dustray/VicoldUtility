using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VicoldUtility.PhotoSelector.Utilities;

namespace VicoldUtility.PhotoSelector.Entities
{

    public sealed class ImageItemEtt : BaseProperty
    {
        private List<string> _sameFileExtensions { get; set; }
        private string _fileNameWithoutExtension;
        private BitmapImage _bitmapImage;

        public ImageItemEtt()
        {
            _sameFileExtensions = new List<string>();
            ImageLabels = new ObservableCollection<ImageLabelViewModel>();
        }

        /// <summary>
        /// 文件全路径
        /// </summary>
        public string FileCanShowingFullPath
        {
            get
            {
                foreach (var extension in _sameFileExtensions)
                {
                    if (FilePathUtility.ExtensionImageType(extension) == ImageFileType.CanReview)
                    {
                        return Path.Combine(FolderPath, $"{FileNameWithoutExtension}{extension}");
                    }
                }

                return null;
            }
        }

        public string FolderPath { get; set; }

        public string FileNameWithoutExtension
        {
            get
            {
                return _fileNameWithoutExtension;
            }
            set
            {
                _fileNameWithoutExtension = value;
            }
        }


        public ObservableCollection<ImageLabelViewModel> ImageLabels { get; set; }

        public BitmapImage BitmapImage
        {
            get
            {
                return _bitmapImage;
            }
            set
            {
                _bitmapImage = value;
                OnPropertyChanged("BitmapImage");
            }
        }

        public void AddExtension(string extension)
        {
            _sameFileExtensions.Add(extension);
            ImageLabels.Add(new ImageLabelViewModel()
            {
                 Text = extension,
            });
        }

    }
}
