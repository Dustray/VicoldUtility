using HighContour;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using VicoldGis.Algorithms;
using VicoldGis.Algorithms.Entities;
using VicoldGis.Entities;
using VicoldGis.VMap.Projections;
using VicoldGis.VMap.Symbols;

namespace VicoldGis.Adapters
{
    internal class TempAdapter : IAdapter
    {
        private string _url;
        private SpaceData _spaceData;
        private float[] analyzeValueSource;

        public TempAdapter(string url)
        {
            _url = url;
        }

        public bool IsParsed { get; set; } = false;
        public bool IsCrossoverAutoOffset { get; set; } = true;
        public byte SmoothCount { get; set; } = 0;
        public int LineWidth { get; set; } = 1;

        public ContourLine[] Lines;
        public void Parse()
        {
            _spaceData = ReadData();
            if (_spaceData == null)
            {
                return;
            }
            //analyzeValueSource = new float[(588 - 472) / 4 + 1];// { 560, 564, 570, 574, 578, 582, 588, 590, 594, 598 };
            //for (int i = 472, j = 0; i <= 588; i += 4, j++)
            //{
            //    analyzeValueSource[j] = i;
            //}
            //analyzeValueSource = /*new float[] { 785 };//*/ new float[(1000 - 460) / 25 + 1];// { 560, 564, 570, 574, 578, 582, 588, 590, 594, 598 };
            //for (int i = 460, j = 0; i <= 1000; i += 25, j++)
            //{
            //    analyzeValueSource[j] = i;
            //}
            analyzeValueSource = _spaceData.CreateAnaValue(20);
            //analyzeValueSource = new float[] { 635 };
            var startX = 0;
            var startY = 0;
            var width = _spaceData.Width;
            var height = _spaceData.Height;

            Stopwatch sw = new Stopwatch();
            sw.Start();
             ContourVicoldAlgo.CreateContourLines(_spaceData.Data, startX, startY, width, height, _spaceData.Width, analyzeValueSource,
                IsCrossoverAutoOffset, SmoothCount, 9999, 0, 80, 0.25f, -0.25f, false);
            //Lines = ContourVicoldAlgo.CreateContourLines(_spaceData.Data, startX, startY, width, height, _spaceData.Width, analyzeValueSource,
            //    IsCrossoverAutoOffset, SmoothCount, 9999, 0, 0, 2.5f, 2.5f, false);
            sw.Stop();
            var old = sw.Elapsed;// Console.WriteLine($"旧算法{sw.Elapsed}");
            sw.Restart();
            var sl = ContourCreator.Excute(_spaceData.Data, startX, startY, width, height, _spaceData.Width, analyzeValueSource,
                new ContourOptions()
                {
                    InvalidValue = 9999,
                    Longitude = 0,
                    Latitude = 80,
                    LonInterval = 0.25f,
                    LatInterval = -0.25f,
                    SmoothInterpolationCount = SmoothCount,
                    ComputeDevice = ComputeDevice.Cuda
                });
            sw.Stop();
            MessageBox.Show($"旧算法{old}   新算法{sw.Elapsed}");
            //Console.WriteLine($"新算法{sw.Elapsed}");
            Lines = new ContourLine[sl.Length];
            for (var l = 0; l < sl.Length; l++)
            {
                Lines[l] = new ContourLine()
                {
                    LinePoints = sl[l].LinePoints,
                    Value = sl[l].Value,
                };
            }
            IsParsed = true;

            //List<ContourLine> ds = new List<ContourLine>();
            //for (var i = Lines.Length - 1; i >= 0; i--)
            //{
            //    for(var j=0;j< Lines[i].LinePoints.Length ; j+=2)
            //    {
            //        var x = Lines[i].LinePoints[j];
            //        var y = Lines[i].LinePoints[j+1];
            //        if (x < 117.5 && x > 115.3 && y < 41.5 && y > 39)
            //        {
            //            ds.Add(Lines[i]);
            //            break;
            //        }
            //    }

            //}

            //Lines = ds.ToArray();
            //var te = JsonConvert.SerializeObject(ds);
            //File.WriteAllText(@"d:\isoline.json", te);

        }

        private SpaceData ReadData()
        {
            var path = Path.GetFullPath(_url);
            if (!File.Exists(path))
            {
                return null;
            }
            //var path = @"D:\1301_601.source";
            var str = File.ReadAllText(path);
            //var str = File.ReadAllText(Path.GetFullPath(@"Data\53_33.source"));
            var wh = Path.GetFileNameWithoutExtension(path).Split('_');
            var width = int.Parse(wh[0]);
            var height = int.Parse(wh[1]);
            var fstrArray = str.Split(' ');
            var ar = new float[fstrArray.Length];
            for (var i = 0; i < fstrArray.Length; i++)
            {
                ar[i] = float.Parse(fstrArray[i]);
            }
            return new SpaceData()
            {
                Width = width,
                Height = height,
                Data = ar,
            };
        }

        public List<FrameworkElement> GetViewData()
        {
            Parse();
            var lines = ToPointList(Lines);
            var renderPaths = SymbolFactory.MakeMiltiLine(new MuiltiLineInfo() //Visual
            {
                Lines = lines,
                IsAutoClose = false,
                LineWidth = LineWidth,
                LineColor = Colors.Red,
            });
            return renderPaths;
        }

        private List<System.Windows.Point[]> ToPointList(ContourLine[] lines)
        {
            var list = new List<System.Windows.Point[]>();

            if (lines == null)
            {
                return list;
            }

            foreach (var line in lines)
            {
                var length = line.LinePoints.Length / 2;
                var points = new System.Windows.Point[length];
                var newIndex = 0;
                for (var i = 0; i < length; i++)
                {
                    var proj = new MercatorProj();
                    points[newIndex] = proj.Project(line.LinePoints[2 * i], line.LinePoints[2 * i + 1]);
                    points[newIndex].X *= App.Current.Map2.Scale;
                    points[newIndex].Y *= App.Current.Map2.Scale;
                    newIndex++;
                }
                list.Add(points);
            }
            return list;
        }
    }
}
