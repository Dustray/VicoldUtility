using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VicoldUtility.PhotoSelector.Entities;
using VicoldUtility.PhotoSelector.Utilities;

namespace VicoldUtility.PhotoSelector.Project
{
    internal class SaveKeeper
    {
        private bool _isChanged = true;
        private bool _isInit = false;

        public SaveKeeper()
        {
            UnallocatedList = new ObservableCollection<ImageItemEtt>();
            SavedList = new ObservableCollection<ImageItemEtt>();
            DeletedList = new ObservableCollection<ImageItemEtt>();
        }

        public string ForderPath { get; set; }
        public IList<ImageItemEtt> UnallocatedList { get; set; }
        public IList<ImageItemEtt> SavedList { get; set; }
        public IList<ImageItemEtt> DeletedList { get; set; }

        public void OpenFolder(string folder)
        {
            ForderPath = folder;
            // todo  先检查目录下是否有工程文件
            LoadListToUnallocated(folder);
            _isInit = true;
        }

        public bool OpenProject(string projectFile)
        {
            var projectJson = File.ReadAllText(projectFile);
            var s = JsonConvert.DeserializeObject<SaveKeeper>(projectJson);


            _isInit = true;
            return true;
        }

        private void LoadListToUnallocated(string folder)
        {
            var files = Directory.GetFiles(folder);
            var canShow = FilePathUtility.PresetExtensionCanReview;
            var cannotShow = FilePathUtility.PresetExtensionCannotReview;

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file).ToLower();
                if (!canShow.Any(v => v == extension) && !cannotShow.Any(v => v == extension))
                {
                    continue;
                }

                var fileNoExtension = Path.GetFileNameWithoutExtension(file);
                var sameFiles = UnallocatedList.Where(v => v.FileNameWithoutExtension == fileNoExtension);
                if (sameFiles.Count() > 0)
                {
                    var imageItem = sameFiles.First();
                    imageItem.SameFileExtensions.Add(extension);
                }
                else
                {
                    var imageItem = new ImageItemEtt()
                    {
                        FileNameWithoutExtension = fileNoExtension,
                        FolderPath = folder,
                    };
                    imageItem.SameFileExtensions.Add(extension);
                    UnallocatedList.Add(imageItem);
                }
            }

            Task.Run(async () =>
            {
                foreach (var fileEtt in UnallocatedList)
                {
                    if (fileEtt.FileCanShowingFullPath != null)
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.None;
                        bitmapImage.UriSource = new Uri(fileEtt.FileCanShowingFullPath);
                        bitmapImage.DecodePixelHeight = 35;   //缩略图高度            
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                        App.Current.SZM.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                        {
                            fileEtt.BitmapImage = bitmapImage;
                        }));
                    }

                    await Task.Delay(1);
                }
            });
        }


        public void Save()
        {

        }
    }
}
