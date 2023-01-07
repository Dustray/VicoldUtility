using CommanderTerminalCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderTerminal
{
    internal class TerminalCore
    {
        #region singleton
        
        private static readonly Lazy<TerminalCore> _instance = new(() => new TerminalCore());
        
        public static TerminalCore Current => _instance.Value;

        #endregion

        public SSHHostConfiguration HostConfig { get; } = new SSHHostConfiguration();


        public void Init()
        {
            
        }
        
    }
}
