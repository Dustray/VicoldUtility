using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogParser
{
    
    internal class RaceAppendBufferLog
    {
        public RaceAppendBufferLog()
        {
            Exec();
        }

        private void Exec()
        {
            string logFile = $@"C:\Users\yiny\Desktop\appendbuffer 测试\int32.txt";
            string resultFile = Path.GetFullPath(@"RaceAppendBufferLog.txt");


            Dictionary<string, float> data = new();
            // 遍历日志每一行
            foreach (string line in File.ReadLines(logFile))
            {
                var lineSpan = line.AsSpan();
                // 判断是否包含关键字

                if (IsIndexOf("#", out var index0))
                {
                   bool isTwoSharp = IsIndexOf("# ", out var index1);
                    if (isTwoSharp)
                    {
                        Console.WriteLine(line);
                        // split by ': '
                        // 获取两个“#”之间的字符串
                        var name = lineSpan.Slice(index0 + 1, index1 - index0 - 1).ToString();
                        var valueStr = lineSpan.Slice(index1 + 2, lineSpan.Length - index1 - 2);
                        var value = float.Parse(valueStr);
                        if (!data.ContainsKey(name))
                        {
                            data.Add(name, value);
                        }
                        else
                        {
                            data[name] += value;
                        }
                    }
                }
              

                bool IsIndexOf(string key, out int index)
                {
                    index = line.IndexOf(key);
                    return index >= 0;
                }

                bool isKeyValue()
                {
                    if (line.Length > 0 && line[0] == '[')
                    {
                        return false;
                    }

                    var index = line.IndexOf(": ");
                    return index >= 0;
                }

                bool isTaskKeyValue()
                {
                    if (line.Length > 0 && line[0] == '[')
                    {
                        return false;
                    }

                    var index = line.IndexOf(": ");
                    var indexTask = line.IndexOf("*");
                    return index >= 0 && indexTask >= 0;
                }

                (string, float) GetNameValue(ReadOnlySpan<char> lineSpan)
                {
                    IsIndexOf(": ", out var index0);
                    var name = lineSpan.Slice(0, index0);
                    var value = lineSpan.Slice(index0 + 2, lineSpan.Length - index0 - 2);
                    return (name.ToString(), float.Parse(value));

                }

            }


            var builder = new StringBuilder();

            foreach (var item in data)
            {
                    builder.AppendLine($"{item.Key}: {Math.Round(item.Value, 3, MidpointRounding.AwayFromZero)}");
            }

            File.WriteAllText(resultFile, builder.ToString());

            //open the file with vscode
            Process.Start(@"C:\Users\yiny\AppData\Local\Programs\Microsoft VS Code\Code.exe", resultFile);
            //Process.Start(new ProcessStartInfo(resultFile) { UseShellExecute = true }); 



        }
    }
}
