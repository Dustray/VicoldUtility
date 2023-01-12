using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommanderTerminalCore.SSHConnection
{
    internal class SSHCore : ISSHHandle, IDisposable
    {
        private readonly string _host;
        private readonly int _port;
        private bool _has_connected = false;
        private LoginKeyType _loginKeyType;
        private SshClient? _client;

        public SSHCore(string host, int port, LoginKeyType loginKeyType)
        {
            this._host = host;
            this._port = port;
            _loginKeyType = loginKeyType;
        }

        public Task<bool> Connect(string user, string? password = null)
        {
            if (_has_connected)
            {
                return Task.FromResult(true);
            }

            if (_loginKeyType == LoginKeyType.Password && password == null)
            {
                return Task.FromResult(false);
            }

            return Task.Run(() =>
            {
                _client = new SshClient(_host, _port, user, password);
                try
                {
                    _client.Connect();
                }
                catch (Renci.SshNet.Common.SshAuthenticationException ex)
                {
                    return false;
                }
                
                return true;
            });
        }

        public Task<string> execute(string command)
        {
            if (_client is not { })
            {
                return Task.FromResult("Execute command failed.");
            }

            return Task.Run(() =>
            {
                return _client.CreateCommand(command).Execute();
            });
        }

        public void Close()
        {
            _client?.Disconnect();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

    }
}
