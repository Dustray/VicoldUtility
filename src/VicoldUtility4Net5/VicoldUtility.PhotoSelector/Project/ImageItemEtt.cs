using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using VicoldUtility.PhotoSelector.Utilities;

namespace VicoldUtility.PhotoSelector.Project
{

    public sealed class ImageItemEtt: INotifyPropertyChanged
    {
        public ImageItemEtt()
        {

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
                        return Path.Combine(FolderPath, FileNameWithoutExtension, extension);
                    }
                }

                return null;
            }
        }

        public string FolderPath { get; set; }

        public string FileNameWithoutExtension { get; set; }

        public string[] SameFileExtensions { get; set; }

        public BitmapImage bitmapImage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CreateThumbnail(int height)
        {
            if (File.Exists(FileCanShowingFullPath))
            {
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.None;
                bitmapImage.UriSource = new Uri(FileCanShowingFullPath);
                bitmapImage.DecodePixelHeight = height;   //缩略图高度            
                bitmapImage.EndInit();
            }
        }
    }
}
