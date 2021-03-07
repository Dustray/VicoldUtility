using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VicoldUtility.PhotoSelector.Entities;
using VicoldUtility.PhotoSelector.Views;

namespace VicoldUtility.PhotoSelector.Project
{
    internal class ProjectHandler
    {
        private SaveKeeper _saveKeeper;


        public ProjectHandler()
        {
            _saveKeeper = new SaveKeeper();
        }


        public void OpenFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new Exception("目标文件夹不存在");
            }
            // todo  先检查目录下是否有工程文件
            _saveKeeper.OpenFolder(folder);
        }

        public void OpenProject(string projectFile)
        {
            if (!File.Exists(projectFile))
            {
                throw new Exception("目标文件夹不存在");
            }
            _saveKeeper.OpenProject(projectFile);
        }

        public void SaveProject()
        {

        }

        public void ResaveProject()
        {

        }

        public void MoveToSaved(int unallocIndex)
        {
            SavedList.Add(UnallocatedList[unallocIndex]);
            UnallocatedList.RemoveAt(unallocIndex);
        }

        public void MoveToDeleted(int unallocIndex)
        {
            DeletedList.Add(UnallocatedList[unallocIndex]);
            UnallocatedList.RemoveAt(unallocIndex);
        }

        public IList<ImageItemEtt> UnallocatedList => _saveKeeper.UnallocatedList;
        public IList<ImageItemEtt> SavedList => _saveKeeper.SavedList;
        public IList<ImageItemEtt> DeletedList => _saveKeeper.DeletedList;
    }
}
