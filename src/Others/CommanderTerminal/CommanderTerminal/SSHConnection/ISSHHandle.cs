﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderTerminal.SSHConnection
{
    internal interface ISSHHandle
    {

        Task<bool> Connect(string user, string? password = null);

        Task<string> execute(string command);

        void Close();
    }
}
