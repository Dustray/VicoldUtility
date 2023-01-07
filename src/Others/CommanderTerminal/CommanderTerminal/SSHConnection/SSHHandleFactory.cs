using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderTerminal.SSHConnection
{
    internal static class SSHHandleFactory
    {
        public static ISSHHandle Create(string ip, int port, LoginKeyType keyType = LoginKeyType.Password)
        {
            return new SSHCore(ip, port, keyType);
        }
    }
}
