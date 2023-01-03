using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Shapes;

namespace CommanderTerminal.Configuration.Entities
{
    internal class SSHCommandConfigEtt
    {
        public string? Name { get; set; }

        public string? Command { get; set; }

        public List<KeyValuePair<string, string>>? ResultFilterCallback { get; set; }
    }

    internal class SSHHostItemConfigEtt
    {
        public string? Name { get; set; }

        public string? Host { get; set; }

        public string? Port { get; set; }

        public string? RememberedPasswd { get; set; }

        public List<SSHCommandConfigEtt>? Commands { get; set; }
    }

    internal class SSHHostConfigEtt
    {
        private string? _configPath;

        public List<SSHHostItemConfigEtt>? HostList { get; set; }

        public void Save()
        {
            var dir = System.IO.Path.GetDirectoryName(_configPath);
            if (dir is not { } || _configPath is not { })
            {
                return;
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            try
            {
                var json = JsonSerializer.Serialize(this);
                File.WriteAllText(_configPath, json);
            }
            catch
            {
            }
        }

        public static SSHHostConfigEtt? Read(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(path);
                var ett = JsonSerializer.Deserialize<SSHHostConfigEtt>(json);
                if (ett is { })
                {
                    ett._configPath = path;
                }
                return ett;
            }
            catch
            {
                return null;
            }
        }
    }
}
