﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.Image2ASCII.Utils
{
    public class FileFormatUtil
    {
        /// <summary>
        /// 根据文件头判断上传的文件类型
        /// </summary>
        /// <param name="filePath">filePath是文件的完整路径 </param>
        /// <returns>返回true或false</returns>
        public bool IsPicture(string filePath)
        {
            try
            {
                string fileClass;
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        byte buffer;
                        buffer = reader.ReadByte();
                        fileClass = buffer.ToString();
                        buffer = reader.ReadByte();
                        fileClass += buffer.ToString();
                    }
                }

                if (fileClass == "255216" || fileClass == "7173" || fileClass == "13780" || fileClass == "6677")
                {//255216是jpg;7173是gif;6677是BMP,13780是PNG;7790是exe,8297是rar
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
