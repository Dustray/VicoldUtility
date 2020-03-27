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
        private static string _stufferStr = "We Don't Talk Anymore. ";
        public static void WriteByteWithEmpty(string path,long size)
        {
            var byteStr = Encoding.UTF8.GetBytes(_stufferStr);
            var stufferStrSize = byteStr.Length;
            var str = new byte[size];
            for(long i = 0,j=0; i < size; i++,j++)
            {
                if (j == stufferStrSize) j = 0;
                str[i] = byteStr[j];
            }
            using (var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite, 1024, true))
            {
                fs.BeginWrite(str, 0, str.Length, new AsyncCallback((asr) => {
                    using (Stream st = (Stream)asr.AsyncState)
                    {
                        st.EndWrite(asr);
                        Console.WriteLine("异步写入结束");
                    }
                }), fs);
                //fs.Close();
            }
        }

        public static void DeleteFolder(string forderPath)
        {
            if (Directory.Exists(forderPath))
            {
                string[] fileSystemEntries = Directory.GetFileSystemEntries(forderPath);
                for (int i = 0; i < fileSystemEntries.Length; i++)
                {
                    string text = fileSystemEntries[i];
                    if (File.Exists(text))
                    {
                        File.Delete(text);
                    }
                    else
                    {
                        DeleteFolder(text);
                    }
                }
                Directory.Delete(forderPath);
            }
        }
    }
}
