using System;
using System.Collections.Generic;
using System.Text;

namespace VicoldUtility.PhotoSelector.Project
{
    internal class SaveKeeper
    {
        private bool _isChanged = true;

        public string ForderPath { get; set; }
        public List<ImageItemEtt> UnallocatedList { get; set; }
        public List<ImageItemEtt> SavedList { get; set; }
        public List<ImageItemEtt> DeletedList { get; set; }


        public void Save()
        {

        }
    }
}
