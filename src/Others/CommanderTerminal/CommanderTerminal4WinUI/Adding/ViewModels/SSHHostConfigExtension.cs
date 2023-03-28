using CommanderTerminalCore.Configuration.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderTerminal.Adding.ViewModels
{
    internal static class SSHHostConfigExtension
    {
        public static HostListItemVM ToVM(this SSHHostItemConfigEtt sshHostItemConfigEtt)
        {
            return new HostListItemVM
            {
                ID = sshHostItemConfigEtt.ID,
                Name = sshHostItemConfigEtt.Name ?? string.Empty,
                Host = sshHostItemConfigEtt.Host ?? string.Empty,
                Port = sshHostItemConfigEtt.Port ?? string.Empty,
                User = sshHostItemConfigEtt.User ?? string.Empty,
                Password = sshHostItemConfigEtt.RememberedPasswd ?? string.Empty,
                IsSavePassword = !string.IsNullOrWhiteSpace(sshHostItemConfigEtt.RememberedPasswd),
            };
        }
    }
}
