using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VicoldUtility.MockLens.WriteableBuffer.OperatorControls
{
    public class BaseControl : UserControl, INotifyPropertyChanged
    {
        #region [BaseControl]

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        protected void SetProperty([CallerMemberName] string? propertyName = null)
        {
            NotifyPropertyChanged(propertyName);
        }

        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
