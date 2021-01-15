﻿using System;
using System.Collections.Generic;
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
        public bool IsCrossoverAutoOffset = true;
        public bool IsSmooth = false;
        public int LineWidth = 1;

        public TempAdapter(string url)
        {
            _url = url;
        }

        public bool IsParsed = false;

        public ContourLine[] Lines;
        public void Parse()
        {
            var data = ReadData();
            if (data == null)
            {
                return;
            }
            analyzeValueSource = new float[(588 - 472) / 4 + 1];// { 560, 564, 570, 574, 578, 582, 588, 590, 594, 598 };
            for (int i = 472, j = 0; i <= 588; i += 4, j++)
            {
                analyzeValueSource[j] = i;
            }

            var startX = 0;
            var startY = 0;
            var width = data.Width;
            var height = data.Height;
            Lines = ContourVicoldAlgo.CreateContourLines(data.Data, startX, startY, width, height, data.Width, analyzeValueSource,
                IsCrossoverAutoOffset, IsSmooth, 9999, 0, 0, 2.5f, 2.5f, false);
            IsParsed = true;

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
