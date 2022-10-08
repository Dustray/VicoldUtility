using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FileHeadFormatter.Entities
{
    internal class UserCfgEtt
    {
        private static string _path = System.IO.Path.Combine(Environment.CurrentDirectory, @"data/user.json");
        
        public string WatcherPath { get; set; } = string.Empty;
        public string WatcherFilter { get; set; } = string.Empty;


        public static UserCfgEtt Load()
        {
            try
            {
                var json = System.IO.File.ReadAllText(_path);
                var ett = JsonSerializer.Deserialize<UserCfgEtt>(json);
                return ett ?? new UserCfgEtt();
            }
            catch
            {
                return new UserCfgEtt();
            }
        }

        internal void Save()
        {
            var json = JsonSerializer.Serialize(this);
            System.IO.File.WriteAllText(_path, json);
        }
    }
}
