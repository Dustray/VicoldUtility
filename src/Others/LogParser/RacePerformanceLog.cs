using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser
{
    internal class RacePerformanceLog
    {
        public RacePerformanceLog()
        {
            Exec();
        }

        private void Exec()
        {
            // 


            // 源路径
            string logFile = @"C:\Users\yiny\Desktop\hls.txt";
            // 保存路径
            string resultFile = Path.GetFullPath(@"result.csv");

            // 判断阶段的关键字
            string func = "PerformanceTest";
            string rc = "Current row count";
            // 把需要提取数据的关键字保存
            string[] partTimes = { "calc total", "data transfer out", "data transfer back", "param transfer", "calculate", "total(during exec)" };

            // 当前正在提取的测试名称
            string currentFunc = string.Empty;
            // 当前正在提取的行数名称
            string currentRowCount = string.Empty;

            bool isCombinedFunction = false;
            // 这是用来处理combine测试结果的map
            Dictionary<string, int> combinedTimeCount = new();
            // 存结果数据map
            // 最内部的List<float>是指多次执行ptest后的同一位置的结果，方便做平均
            Dictionary<string, Dictionary<string, Dictionary<string, List<float>>>> result = new();
            // 遍历日志每一行
            foreach (string line in File.ReadLines(logFile))
            {
                var lineSpan = line.AsSpan();
                // 判断是否包含关键字

                if (IsIndexOf(func, out var index0))
                {
                // 提取测试名称
                    Console.WriteLine(line);
                    var start = lineSpan.IndexOf('/') + 1;
                    var newSpan = lineSpan.Slice(start);
                    var end = newSpan.IndexOf('/');
                    if (end == -1)
                    {
                        continue;
                    }
                    var functionName = newSpan.Slice(0, end);
                    currentFunc = functionName.ToString();

                    #region Combined情况
                    isCombinedFunction = currentFunc.Contains("Combined");
                    #endregion

                    if (!result.TryGetValue(currentFunc, out _))
                    {
                        result.Add(currentFunc, new());
                    }
                }
                else if (IsIndexOf(rc, out var index1))
                {
                // 提取测试行数信息
                    Console.WriteLine(line);

                    var start = index1 + rc.Length + 1;
                    var newSpan = lineSpan.Slice(start, lineSpan.Length - start);
                    var end = newSpan.IndexOf(' ');
                    var rowCount = newSpan.Slice(0, end);
                    currentRowCount = rowCount.ToString();
                    if (!result[currentFunc].TryGetValue(currentRowCount, out _))
                    {
                        result[currentFunc].Add(currentRowCount, new());
                    }

                    #region Combined情况
                    if (isCombinedFunction)
                    {
                        if (!combinedTimeCount.TryGetValue(currentFunc + currentRowCount, out var times))
                        {
                            combinedTimeCount[currentFunc + currentRowCount] = 0;
                        }
                        else
                        {
                            combinedTimeCount[currentFunc + currentRowCount]++;
                        }
                    }
                    #endregion
                }
                else if (IsTimeIndexOf(out var part, out var index2))
                {
                // 提取测试时间关键字
                    Console.WriteLine(line);

                    var start = index2 + part.Length + 1;
                    var newSpan = lineSpan.Slice(start, lineSpan.Length - start);
                    var end = newSpan.IndexOf("ms");
                    var rowCount = newSpan.Slice(0, end).ToString();
                    float time = float.Parse(rowCount);
                    if (!result[currentFunc][currentRowCount].TryGetValue(part, out var valueList))
                    {
                        result[currentFunc][currentRowCount][part] = new List<float>();
                    }

                    if (isCombinedFunction)
                    {
                        var count = result[currentFunc][currentRowCount][part].Count;
                        var times = combinedTimeCount[currentFunc + currentRowCount];// result[currentFunc][currentRowCount][part].Count;
                        if (count <= times)
                        {
                            result[currentFunc][currentRowCount][part].Add(time);
                        }
                        else
                        {

                            result[currentFunc][currentRowCount][part][times] += time;
                        }

                    }
                    else
                    {
                        result[currentFunc][currentRowCount][part].Add(time);
                    }
                }

                // 判断key是否在字符串line中，是的话返回位置到index
                bool IsIndexOf(string key, out int index)
                {
                    index = line.IndexOf(key);
                    return index > 0;
                }

                // 判断当前行中是否存在需要统计的关键字，是的话返回关键字和字符的索引位
                bool IsTimeIndexOf(out string part, out int index)
                {
                    foreach (var partT in partTimes)
                    {
                        var isIndexOf = IsIndexOf(partT, out index);
                        if (isIndexOf)
                        {
                            part = partT;
                            return true;
                        }
                    }

                    part = string.Empty;
                    index = -1;
                    return false;
                }
            }

            // 统计完毕，开始写csv文件
            var builder = new StringBuilder();

            builder.Append(',');
            builder.Append(',');

            foreach (var partTimeKey in partTimes)
            {
                builder.Append(partTimeKey);
                builder.Append(',');
            }
            builder.AppendLine();

            foreach (var function in result)
            {
                foreach (var rowCount in function.Value)
                {
                    builder.Append(function.Key);
                    builder.Append(',');
                    builder.Append(rowCount.Key);
                    builder.Append(',');

                    foreach (var partTimeKey in partTimes)
                    {
                        if (rowCount.Value.TryGetValue(partTimeKey, out var values))
                        {
                            if (values.Count == 0)
                            {
                                continue;
                            }

                            float sumValue = 0;

                            foreach (var value in values)
                            {
                                sumValue += value;
                            }

                            var v = Math.Round(sumValue / values.Count, 2, MidpointRounding.AwayFromZero);
                            builder.Append(v);
                            builder.Append(',');
                        }
                    }

                    builder.AppendLine();
                }
            }

            File.WriteAllText(resultFile, builder.ToString());

        }
    }
}
