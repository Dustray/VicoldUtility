using CommanderTerminalCore.Configuration.Entities;

namespace CommanderTerminalCore.Configuration
{
    public class SSHHostConfiguration
    {
        private const string _baseHostConfigPath = "config/SSHHostConfig.json";
        private SSHHostConfigEtt? _sshHostConfigEtt;

        public SSHHostConfigEtt GetConfigEtt()
        {
            if (_sshHostConfigEtt is null)
            {
                var fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _baseHostConfigPath);
                _sshHostConfigEtt = SSHHostConfigEtt.Read(fullpath) ?? new SSHHostConfigEtt(fullpath);
            }

            return _sshHostConfigEtt;
        }


    }
}
