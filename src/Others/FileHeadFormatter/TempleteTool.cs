using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace FileHeadFormatter
{
    [ Serializable]
    internal static class TemplateTool
    {

        public static void Process(string fileName, string templateText, string parameterTest, bool isConfirm = true)
        {
            if (!File.Exists(fileName))
            {
                MessageBox.Show("File not found");
                return;
            }

            var encoding = FileEncoding.GetType(fileName);
            var content = File.ReadAllText(fileName, encoding);

            // 打开文件流，按行读取文件
            using var reader = new StreamReader(fileName, encoding);
            string? line = null;
            long unWhiteIndex = -1;
            bool contentWhiteOrEmpty = IsContentWhiteOrEmpty(content);
            while ((line = reader.ReadLine()) != null)
            {
                var firstNonSpaceIndex = line.TakeWhile(c => char.IsWhiteSpace(c)).Count();
                if (firstNonSpaceIndex != line.Length)
                {
                    unWhiteIndex = firstNonSpaceIndex;
                    //从索引位开始判断content是不是以/**开头
                    if (line.Substring(firstNonSpaceIndex, 3) == "/**")
                    {
                        return;
                    }

                    break;
                }
            }
            reader.Close();

            // 检测文本文件的编码格式
            var template = templateText;

            template = FilterLines(template, fileName, !contentWhiteOrEmpty);
            var parameterStr = parameterTest;
            // parameterStr 转行数组
            var parameters = parameterStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (var param in parameters)
            {
                var pair = param.Split('=');
                if (pair.Length != 2)
                {
                    continue;
                }

                var paramName = pair[0].Replace(" ", "");
                paramName = paramName.Replace("\t", "");
                var paramValue = pair[1].Trim();
                paramValue = paramValue.Replace("\t", "");


                //var paramStr = new Regex("[\\s]+").Replace(param, " ");
                //// 以多个空格分割参数名称和参数值
                //var paramArr = paramStr.Split(' ');
                //// 参数名称
                //var paramName = paramArr[0];
                //// 参数值
                //var paramValue = paramArr[1];
                if (paramValue.StartsWith('%') && paramValue.EndsWith('%'))
                {
                    paramValue = GetParamValue(paramValue);
                }

                template = template.Replace(paramName, paramValue);
            }
            //模板确认

            var dialog = new ConfirmWindow(template);
            if (!isConfirm || dialog.ShowDialog() == true)
            {
                template = dialog.TemplateText;
                dialog.Close();

                // 将template写入content的头部
                var newContent = template + content;
                File.WriteAllText(fileName, newContent, encoding);
            }
            else
            {
                return;
            }


            string GetParamValue(string presetName)
            {
                switch (presetName)
                {
                    case "%currentfile%":
                        return System.IO.Path.GetFileName(fileName);
                    case "%currentpath%":
                        return GetRelativePath(fileName);
                    case "%pathupper%":
                        return GetUpperPath(fileName);
                    case "%datetime.now%":
                        return DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    default:
                        return "";
                }
            }

            bool IsContentWhiteOrEmpty(string content)
            {
                for (var i = 0; i < content.Length; i++)
                {
                    switch (content[i])
                    {
                        case ' ':
                        case '\r':
                        case '\n':
                        case '\t':
                            continue;
                        default:
                            return false;
                    }
                }

                return true;
            }
        }

        private static string FilterLines(string template, string fileName, bool contentNotWhiteOrEmpty)
        {
            // 将template以换行符为单位分割成数组

            var lines = template.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var resultLine = new List<string>(lines.Length);
            foreach (var line in lines)
            {
                var lineSpan = new Span<char>(line.ToCharArray());

                string? newLine = null;

                if (lineSpan.StartsWith("$$"))
                {
                    newLine = Filter(lineSpan);
                }
                else
                {
                    newLine = line;
                }

                if (newLine is { })
                {
                    resultLine.Add(newLine);
                }
            }

            return string.Join("\r\n", resultLine);

            // 递归遍历参数
            string? Filter(Span<char> lineSpan)
            {
                var pIndex = lineSpan.IndexOf(' ');
                if (pIndex != -1)
                {
                    var param = lineSpan.Slice(0, pIndex);

                    if (param.StartsWith("$$if"))
                    {
                        bool param_if_not_not = (param.Length <= 4 || param[4] != '!');
                        if (param.Length <= 4)
                        {
                            // 纯$$if，todo: 判断后面参数
                        }
                        else
                        {

                            lineSpan = lineSpan.Slice(pIndex + 1);
                            var vIndex = lineSpan.IndexOf(' ');

                            param = param.Slice(param_if_not_not ? 4 : 5, param.Length - (param_if_not_not ? 4 : 5));
                            switch (param.ToString())
                            {
                                case "emptyfile":
                                    if (contentNotWhiteOrEmpty)
                                    {
                                        //不为空
                                        if (param_if_not_not)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            //非
                                            return null;
                                        }
                                    }
                                    else
                                    {
                                        //为空
                                        if (param_if_not_not)
                                        {
                                            return null;
                                        }
                                        else
                                        {
                                            //非
                                            break;
                                        }
                                    }

                                case "ext":
                                    // 不带点的扩展名
                                    var extension = System.IO.Path.GetExtension(fileName);
                                    if (extension.StartsWith('.'))
                                    {
                                        extension = extension.Substring(1);
                                    }




                                    Span<char> value;
                                    if (vIndex != -1)
                                    {
                                        value = lineSpan.Slice(0, vIndex);
                                        lineSpan = lineSpan.Slice(vIndex + 1);
                                    }
                                    else
                                    {
                                        value = lineSpan;
                                        lineSpan = new Span<char>(new char[0]);
                                    }


                                    var extensions = value.ToString().Split('|');
                                    if (!extensions.Contains(extension))
                                    {
                                        return null;
                                    }

                                    break;
                            }
                        }
                    }

                    if (lineSpan.StartsWith("$$"))
                    {
                        return Filter(lineSpan);
                    }

                }
                else
                {
                    // 表达式不合法
                    return null;
                }


                return lineSpan.ToString();

            }

        }

        private static string GetUpperPath(string path)
        {
            path = path.Replace("\\", "/");
            // path 移除扩展名
            var extIndex = path.LastIndexOf('.');
            if (extIndex != -1)
            {
                path = path.Substring(0, extIndex);
            }

            var pathArray = path.Split('/');
            var includeIndex = Array.FindIndex(pathArray, s => s.ToLower() == "include");
            includeIndex += includeIndex != -1 ? 1 : 0;
            var srcIndex = Array.FindIndex(pathArray, s => s.ToLower() == "src");
            srcIndex += srcIndex != -1 ? 1 : 0;
            var index = includeIndex > srcIndex ? includeIndex : srcIndex;
            if (index != -1)
            {
                var span = new Span<string>(pathArray, index, pathArray.Length - index);
                return string.Join('_', span.ToArray()).ToUpper();
            }
            else
            {
                return string.Join('_', pathArray).ToUpper();
            }
        }

        /// <summary>
        /// 提取相对路径 至include或src
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetRelativePath(string path)
        {
            path = path.Replace("\\", "/");
            var pathArray = path.Split('/');
            var includeIndex = Array.FindIndex(pathArray, s => s.ToLower() == "include") - 1;
            var srcIndex = Array.FindIndex(pathArray, s => s.ToLower() == "src") - 1;
            var index = includeIndex > srcIndex ? includeIndex : srcIndex;
            if (index != -1)
            {
                var span = new Span<string>(pathArray, index, pathArray.Length - index);
                return string.Join('/', span.ToArray());
            }
            else
            {
                return path;
            }
        }
    }
}
