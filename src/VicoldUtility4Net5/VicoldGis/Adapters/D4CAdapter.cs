using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VicoldGis.Utilities;

namespace VicoldGis.Adapters
{
    internal class D4CAdapter : IAdapter
    {
        private string _url;

        public D4CAdapter(string url)
        {
            _url = url;
        }

        public List<FrameworkElement> GetViewData()
        {
            throw new NotImplementedException();
        }

        public void Parse()
        {
            //if (!File.Exists(_url))
            //{
            //    return;
            //}

            //char[] seperator = new char[] { ' ', '\r', '\n', '\t' };
            //using (Stream stream = new FileStream(_url, FileMode.Open))
            //{
            //    Encoding encoding = FIleUtilities.GetEncoding(stream);
            //    using (var reader = new FastTextFileReader(stream, encoding))
            //    {
            //        //检查是否有22个头部项可读
            //        bool has_header = reader.EnsureReadableItems(22);
            //        if (!has_header)
            //        {
            //            //空文件
            //            return;
            //        }

            //        reader.ReadAndDiscard(2);


            //        _meta.SetValue("description", reader.Read());
            //        _meta.SetValue("year", reader.Read());
            //        _meta.SetValue("month", reader.Read());
            //        _meta.SetValue("day", reader.Read());
            //        _meta.SetValue("hour", reader.Read());
            //        _meta.SetValue("duration", reader.Read());
            //        _meta.SetValue("level", reader.Read());

            //        //string timeStr = $@"{year}/{month}/{day} {hour}:0:0";
            //        //_meta.SetValue("dataTime", timeStr);
            //        var _xInteravl = reader.ReadFloat(0);
            //        var _yInterval = reader.ReadFloat(0);
            //        float startLon = reader.ReadFloat(0);
            //        float endLon = reader.ReadFloat(0);
            //        float startLat = reader.ReadFloat(0);
            //        float endLat = reader.ReadFloat(0);
            //        int xSize = reader.ReadInt(0);
            //        int ySize = reader.ReadInt(0);

            //        float lineInteravl = reader.ReadFloat(0);
            //        float startValue = reader.ReadFloat(0);
            //        float endValue = reader.ReadFloat(0);

            //        string smooth = reader.Read();
            //        string bold = reader.Read();

            //        _meta.SetValue("xinteravl", _xInteravl.ToString());
            //        _meta.SetValue("yinteravl", _yInterval.ToString());
            //        _meta.SetValue("startlon", startLon.ToString());
            //        _meta.SetValue("endlon", endLon.ToString());
            //        _meta.SetValue("startlat", startLat.ToString());
            //        _meta.SetValue("endlat", endLat.ToString());
            //        _meta.SetValue("xsize", xSize.ToString());
            //        _meta.SetValue("ysize", ySize.ToString());
            //        _meta.SetValue("lineinteravl", lineInteravl.ToString());
            //        _meta.SetValue("startvalue", startValue.ToString());
            //        _meta.SetValue("endvalue", endValue.ToString());
            //        _meta.SetValue("smooth", smooth);
            //        _meta.SetValue("boldvalue", bold);
            //        //read headers

            //        //Coordinate min = new Coordinate(startLon < endLon ? startLon : endLon,
            //        //    startLat < endLat ? startLat : endLat, 0);
            //        //Coordinate max = new Coordinate(endLon > startLon ? endLon : startLon,
            //        //    endLat > startLat ? endLat : startLat, 0);
            //        //_bounds.SetExtents(min, max);

            //        //ReadRecord(xSize, ySize, reader);

            //        if (endValue < startValue && lineInteravl > 0)
            //        {
            //            float tmpv = endValue;
            //            endValue = startValue;
            //            startValue = tmpv;
            //        }
            //        if (lineInteravl != 0)
            //        {
            //            //确定有效位数，使用整型数处理，保证标值不出问题。
            //            string tmpstr = lineInteravl.ToString();
            //            int digNumber = 0;
            //            int scale = 1;
            //            try
            //            {
            //                digNumber = tmpstr.Length - tmpstr.IndexOf(".");
            //                scale = (int)Math.Pow(10, digNumber);
            //            }
            //            catch { }

            //            int anaLineNum = (int)((endValue - startValue) / lineInteravl) + 1;
            //            if (anaLineNum < 0)
            //            {
            //                anaLineNum = -anaLineNum + 2;
            //            }

            //            var anaLineValue = new float[anaLineNum];
            //            if (anaLineNum > 0)
            //            {
            //                anaLineValue[0] = (int)(startValue * scale);
            //            }
            //            for (int i = 1; i < anaLineNum; i++)
            //            {
            //                anaLineValue[i] = (int)(anaLineValue[i - 1] + (int)(lineInteravl * scale));
            //            }
            //            for (int i = 0; i < anaLineNum; i++)
            //                anaLineValue[i] = anaLineValue[i] / scale;

            //            _meta.SetValue("analysisvalues", String.Join(",", anaLineValue));
            //        }
            //    }
            //}
        }
    }
}