﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VicoldUtility.FastLink.Utilities
{
    internal static class XMLUtil
    {
        /// <summary>
        /// 加载指定XML文件并序列化为指定泛型对象
        /// <para>异步的</para>
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="xmlFilePath">XML文件路径(绝对路径)</param>
        /// <returns>加载结果</returns>
        public static Task<T> LoadXMLToAsync<T>(string xmlFilePath)
        {
            var tcsResult = new TaskCompletionSource<T>();
            try
            {
                if (!File.Exists(xmlFilePath))
                {
                    var ex = new Exception("对不起！未能找到需要加载的XML文件；请确认文件是否存在，及路径是否正确。");
                    tcsResult.SetException(ex);
                }
                else
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlFilePath);
                    var root = xmlDoc.DocumentElement;
                    var tmpString = JsonConvert.SerializeXmlNode(root, Newtonsoft.Json.Formatting.None, true);
                    var result = JsonConvert.DeserializeObject<T>(tmpString);
                    tcsResult.SetResult(result);
                }
            }
            catch (Exception ex)
            {
                tcsResult.SetException(ex);
            }
            return tcsResult.Task;
        }

    }
}
