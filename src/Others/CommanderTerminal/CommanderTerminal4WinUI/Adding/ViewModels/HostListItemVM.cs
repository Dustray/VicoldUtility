using CommanderTerminal.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderTerminal.Adding.ViewModels
{
    internal class HostListItemVM : NotifyPropertyHandler
    {
        public int ID { get; set; } = Guid.NewGuid().GetHashCode();

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _host = string.Empty;
        public string Host
        {
            get { return _host; }
            set { SetProperty(ref _host, value); }
        }

        private string _ip = string.Empty;
        public string Port
        {
            get { return _ip; }
            set { SetProperty(ref _ip, value); }
        }

        public string HostPort => $"{Host}:{Port}";

        private string _password = string.Empty;
        public string Password
        {
            get { return _isSavePassword ? _password : string.Empty; }
            set { SetProperty(ref _password, value); }
        }
        public string VisablePassword
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private bool _isSavePassword = false;
        public bool IsSavePassword
        {
            get { return _isSavePassword; }
            set { SetProperty(ref _isSavePassword, value); }
        }

        public void Clear()
        {
            ID = Guid.NewGuid().GetHashCode();
            Name = string.Empty;
            Host = string.Empty;
            Port = string.Empty;
            Password = string.Empty;
            IsSavePassword = false;
        }

        public void CopyFrom(HostListItemVM vm)
        {
            ID = vm.ID;
            Name = vm.Name;
            Host = vm.Host;
            Port = vm.Port;
            Password = vm.Password;
            IsSavePassword = vm.IsSavePassword;
        }

    }
}
