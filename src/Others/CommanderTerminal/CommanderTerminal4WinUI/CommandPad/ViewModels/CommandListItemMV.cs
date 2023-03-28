using CommanderTerminal.Adding.ViewModels;
using CommanderTerminal.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderTerminal.CommandPad.ViewModels
{
    internal class CommandListItemMV : NotifyPropertyHandler
    {
        public int ID { get; set; } = Guid.NewGuid().GetHashCode();
        
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _command = string.Empty;
        public string Command
        {
            get { return _command; }
            set { SetProperty(ref _command, value); }
        }

    }
}
