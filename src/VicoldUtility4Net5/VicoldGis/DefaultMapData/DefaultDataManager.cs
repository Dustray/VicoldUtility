using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using VicoldGis.VMap.Projections;
using VicoldGis.VMap.Symbols;

namespace VicoldGis.DefaultMapData
{
    internal class DefaultDataManager
    {
        private const string _provinceControl = @"Data\map\provinces2b.ctl";
        private const string _provinceData = @"Data\map\provinces2b.adr";
        private const string _cityControl = @"Data\map\shp2b_city_full.ctl";
        private const string _cityData = @"Data\map\shp2b_city_full.adr";
        private const string _countyControl = @"Data\map\countiesb2b.ctl";
        private const string _countyData = @"Data\map\countiesb2b.adr";
        private const string _globalControl = @"Data\map\global2b.ctl";
        private const string _globalData = @"Data\map\global2b.adr";

        public void LoadProvinceLine()
        {
            var lineDic = LoadAdrShape(_globalControl, _globalData);
            LoadToMap(lineDic);
        }

        public void LoadCityLine()
        {
            var lineDic = LoadAdrShape(_cityControl, _cityData);
            LoadToMap(lineDic);
        }

        public void LoadCountyLine()
        {
            var lineDic = LoadAdrShape(_countyControl, _countyData);
            LoadToMap(lineDic);
        }

        private void LoadToMap(Dictionary<int, List<double[]>> lineDic)
        {
            var lines = ToPointList(lineDic);
            var renderPaths = SymbolFactory.MakeMiltiLine(new MuiltiLineInfo() //Visual
            {
                Lines = lines,
                IsAutoClose = false,
                LineWidth = 1
            }); 
            var layer = new VMap.Layer();
            foreach (var renderPath in renderPaths)
            {
                layer.Add(renderPath);
            }
            App.Current.Map2.Manager.Add(layer);
        }

        private List<Point[]> ToPointList(Dictionary<int, List<double[]>> dic)
        {
            var list = new List<Point[]>();
            foreach (var lines in dic)
            {
                foreach (var line in lines.Value)
                {
                    var length = line.Length / 2;
                    var chouxiNum = length > 10 ? 20 : 1;
                    var points = new Point[length/* / chouxiNum*/];
                    var j = 0;
                    var newIndex = 0;
                    for (var i = 0; i < length; i++)
                    {
                        //if (j < chouxiNum - 1)
                        //{
                        //    j++;
                        //    continue;
                        //}
                        //j = 0;
                        var proj = new MercatorProj();
                        points[newIndex] = proj.Project(line[2 * i], line[2 * i + 1]);
                        points[newIndex].X *= App.Current.Map2.Scale;
                        points[newIndex].Y *= App.Current.Map2.Scale;
                        newIndex++;
                    }
                    list.Add(points);
                }
            }
            return list;
        }

        private bool IsPointsTooClose(double lon1, double lat1, double lon2, double lat2)
        {
            var dis = Math.Sqrt((lon1 - lon2) * (lon1 - lon2) + (lat1 - lat2) * (lat1 - lat2));

            return dis > 0.008;
        }

        /// <summary>
        /// 从二进制文件加载shape数据
        /// </summary>
        /// <param name="ctlPath">控制文件路径</param>
        /// <param name="dataPath">数据文件路径</param>
        /// <returns></returns>
        internal static Dictionary<int, List<double[]>> LoadAdrShape(string ctlPath, string dataPath)
        {
            // var ctlContent = new Dictionary<int, IList<(int start, int length)>>();
            var ctlFile = File.ReadAllText(Path.GetFullPath(ctlPath));
            var ctlContent = JsonConvert.DeserializeObject<Dictionary<int, IList<(int start, int length)>>>(ctlFile);

            var result = new Dictionary<int, List<double[]>>();
            using (var fs = new FileStream(Path.GetFullPath(dataPath), FileMode.Open))
            {
                foreach (var code in ctlContent)
                {
                    var rangePositions = code.Value;
                    var lines = new List<double[]>();
                    foreach (var position in rangePositions)
                    {
                        fs.Seek(position.start, SeekOrigin.Begin);
                        var b = new byte[position.length];
                        fs.Read(b, 0, position.length);

                        var dataArray = new double[position.length / 8];
                        for (int i = 0, j = 0; i < b.Length; i += 8, j++)
                        {
                            dataArray[j] = BitConverter.ToDouble(b, i);
                        }

                        lines.Add(dataArray);
                    }
                    result[code.Key] = lines;
                }
            }

            return result;
        }
    }
}
