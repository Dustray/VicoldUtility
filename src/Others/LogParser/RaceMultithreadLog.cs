using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogParser
{

    class BiuBiuBiu
    {
        public Dictionary<string, float> Items { get; set; } = new();
        public Dictionary<int, Dictionary<string, float>> TaskItems { get; set; } = new();

    }
    internal class RaceMultithreadLog
    {
        public RaceMultithreadLog()
        {
            Exec();
        }

        private void Exec()
        {
            int threadCount =5;
            string logFile = $@"C:\Users\yiny\Desktop\hls\p.txt";
            string resultFile = Path.GetFullPath(@"RaceMultithreadLog.txt");


            Dictionary<int, BiuBiuBiu> data = new();
            int thread = 0;
            bool ps = false;
            // 遍历日志每一行
            foreach (string line in File.ReadLines(logFile))
            {
                var lineSpan = line.AsSpan();
                // 判断是否包含关键字

                if (IsIndexOf("#", out var index0))
                {
                    Console.WriteLine(line);
                    // split by ': '
                    var split = lineSpan.Slice(index0 + 3, lineSpan.Length - index0 - 3);
                    var key = int.Parse(split);
                    if (!data.ContainsKey(key))
                    {
                        data.Add(key, new());
                    }
                    thread = key;
                }
                else if (isTaskKeyValue())
                {
                    (string name, float value) = GetNameValue(lineSpan);
                    var newName = name.Split('*')[0];
                    var taskId = int.Parse(name.Split('*')[1]);
                    if (data[thread].TaskItems.ContainsKey(taskId))
                    {
                        if (data[thread].TaskItems[taskId].ContainsKey(newName))
                        {
                            data[thread].TaskItems[taskId][newName] += value;
                        }
                        else
                        {
                            data[thread].TaskItems[taskId].Add(newName, value);
                        }
                    }
                    else
                    {
                        data[thread].TaskItems[taskId] = new();
                        data[thread].TaskItems[taskId].Add(newName, value);
                    }
                }
                else if (isKeyValue())
                {
                    (string name, float value) = GetNameValue(lineSpan);
                    if (data[thread].Items.ContainsKey(name))
                    {
                        data[thread].Items[name] += value;
                    }
                    else
                    {
                        data[thread].Items.Add(name, value);
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
                builder.AppendLine($"\r\n =========================================");
                builder.AppendLine($"thread: {item.Key}");
                foreach (var item2 in item.Value.Items)
                {
                    builder.AppendLine($"{item2.Key}: {Math.Round(item2.Value, 3, MidpointRounding.AwayFromZero)}");
                }

                Dictionary<string, float> max = new();
                Dictionary<string, float> min = new();
                Dictionary<string, float> total = new();
                foreach (var item2 in item.Value.TaskItems)
                {
                    //  builder.AppendLine($"== task: {item2.Key} ==");
                    foreach (var itemInTask in item2.Value)
                    {
                        // builder.AppendLine($"{itemInTask.Key}: {Math.Round(itemInTask.Value, 3, MidpointRounding.AwayFromZero)}");

                        if (total.TryGetValue(itemInTask.Key, out var value))
                        {
                            total[itemInTask.Key] = value + itemInTask.Value;
                        }
                        else
                        {
                            total.Add(itemInTask.Key, itemInTask.Value);
                        }

                        if (max.TryGetValue(itemInTask.Key, out var maxValue))
                        {
                            if (maxValue < itemInTask.Value)
                            {
                                max[itemInTask.Key] = itemInTask.Value;
                            }
                        }
                        else
                        {
                            max.Add(itemInTask.Key, itemInTask.Value);
                        }

                        if (min.TryGetValue(itemInTask.Key, out var minValue))
                        {
                            if (minValue > itemInTask.Value)
                            {
                                min[itemInTask.Key] = itemInTask.Value;
                            }
                        }
                        else
                        {
                            min.Add(itemInTask.Key, itemInTask.Value);
                        }
                    }

                    var calc = "task_calc";
                    var taskCalc = item2.Value["binary-once"] - item2.Value["purewait"];
                    // builder.AppendLine($"task_calc: {Math.Round(taskCalc, 3, MidpointRounding.AwayFromZero)}");
                    if (total.TryGetValue(calc, out var calcValue))
                    {
                        total[calc] = calcValue + taskCalc;
                    }
                    else
                    {
                        total.Add(calc, taskCalc);
                    }

                    if (max.TryGetValue(calc, out var maxCValue))
                    {
                        if (maxCValue < taskCalc)
                        {
                            max[calc] = taskCalc;
                        }
                    }
                    else
                    {
                        max.Add(calc, taskCalc);
                    }

                    if (min.TryGetValue(calc, out var minCValue))
                    {
                        if (minCValue > taskCalc)
                        {
                            min[calc] = taskCalc;
                        }
                    }
                    else
                    {
                        min.Add(calc, taskCalc);
                    }

                }


                foreach (var itemInTotal in total)
                {
                    builder.AppendLine($"{itemInTotal.Key} total: {Math.Round(itemInTotal.Value, 3, MidpointRounding.AwayFromZero)}");
                }
                foreach (var itemInTotal in total)
                {
                    var avgValue = itemInTotal.Value / item.Value.TaskItems.Count;
                    builder.AppendLine($"{itemInTotal.Key} avg: {Math.Round(avgValue, 4, MidpointRounding.AwayFromZero)}");
                }
                foreach (var itemInMax in max)
                {
                    builder.AppendLine($"{itemInMax.Key} max: {Math.Round(itemInMax.Value, 3, MidpointRounding.AwayFromZero)}");
                }

                foreach (var itemInMin in min)
                {
                    builder.AppendLine($"{itemInMin.Key} min: {Math.Round(itemInMin.Value, 3, MidpointRounding.AwayFromZero)}");
                }



            }

            File.WriteAllText(resultFile, builder.ToString());

            //open the file with vscode
            Process.Start(@"C:\Users\yiny\AppData\Local\Programs\Microsoft VS Code\Code.exe", resultFile);
            //Process.Start(new ProcessStartInfo(resultFile) { UseShellExecute = true }); 



        }
    }
}
