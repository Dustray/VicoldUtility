using CommanderTerminal.Adding.ViewModels;
using CommanderTerminalCore.Configuration.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderTerminal.CommandPad.ViewModels
{
    internal static class CommandConfigExtension
    {
        public static CommandListItemMV ToVM(this SSHCommandConfigEtt commandItemConfigEtt)
        {
            return new CommandListItemMV
            {
                ID = commandItemConfigEtt.ID,
                Name = commandItemConfigEtt.Name ?? string.Empty,
                Command = commandItemConfigEtt.Command ?? string.Empty,
            };
        }
    }
}
