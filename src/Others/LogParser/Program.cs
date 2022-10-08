// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");



string logFile = @"C:\Users\yinxi\Desktop\ptest_log.txt";
string resultFile = Path.GetFullPath(@"result.csv");

string func = "PerformanceTest";
string rc = "Current row count";
string[] partTimes = { "calc total", "data transfer", "param transfer", "calculate", "total(during exec)" };

string currentFunc = string.Empty;
string currentRowCount = string.Empty;

Dictionary<string, Dictionary<string, Dictionary<string, float>>> result = new();
// 遍历日志每一行
foreach (string line in File.ReadLines(logFile))
{
    var lineSpan = line.AsSpan();
    // 判断是否包含关键字

    if (IsIndexOf(func, out var index0))
    {
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
        if (!result.TryGetValue(currentFunc, out _))
        {
            result.Add(currentFunc, new());
        }
    }
    else if (IsIndexOf(rc, out var index1))
    {
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
    }
    else if (IsTimeIndexOf(out var part, out var index2))
    {
        Console.WriteLine(line);

        var start = index2 + part.Length + 1;
        var newSpan = lineSpan.Slice(start, lineSpan.Length - start);
        var end = newSpan.IndexOf("ms");
        var rowCount = newSpan.Slice(0, end).ToString();
        float time = float.Parse(rowCount);
        result[currentFunc][currentRowCount][part] = time;
    }

    bool IsIndexOf(string key, out int index)
    {
        index = line.IndexOf(key);
        return index > 0;
    }

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
            if (rowCount.Value.TryGetValue(partTimeKey, out var value))
            {
                var v = Math.Round(value, 2, MidpointRounding.AwayFromZero);
                builder.Append(v);
                builder.Append(',');
            }
        }

        builder.AppendLine();
    }
}

File.WriteAllText(resultFile, builder.ToString());
