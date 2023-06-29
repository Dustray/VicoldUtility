using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.AvatarMaker.ViewModels
{
    public class StatusbarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string StatusMessage { get; set; }
    }
}
