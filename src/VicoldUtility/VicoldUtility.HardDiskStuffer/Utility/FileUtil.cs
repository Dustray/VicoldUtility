using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.HardDiskStuffer.Utility
{
    public static class FileUtil
    {
        public static void WriteByteWithEmpty(string path,long size)
        {
            using (var fs = new FileStream(path, FileMode.CreateNew))
            {
                fs.Seek(size, SeekOrigin.Begin);
                fs.WriteByte(0);
                fs.Close();
            }
        }
    }
}
