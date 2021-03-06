using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VicoldUtility.PhotoSelector.Utilities;

namespace VicoldUtility.PhotoSelector.Entities
{

    public sealed class ImageItemEtt : BaseProperty
    {
        private string _fileNameWithoutExtension;
        private BitmapImage _bitmapImage;

        public ImageItemEtt()
        {
            SameFileExtensions = new List<string>();
        }

        /// <summary>
        /// 文件全路径
        /// </summary>
        public string FileCanShowingFullPath
        {
            get
            {
                foreach (var extension in SameFileExtensions)
                {
                    if (FilePathUtility.ExtensionImageType(extension) == Entities.ImageFileType.CanReview)
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

        public List<string> SameFileExtensions { get; set; }


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

    }
}
