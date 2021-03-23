using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VicoldUtility.PhotoLens.Algorithm.Isoline
{
    internal static class Connector
    {
        private delegate void OnTracing(IntPtr userData, IntPtr linePtr, int index);

        [System.Security.SuppressUnmanagedCodeSecurity()]
        [DllImport(@".\Libs\msq.dll", EntryPoint = "msq_trace_contours", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern int msq_trace_contours(MsqGridSlice gridSlice, MsqTracePramas tracePramas, IntPtr allocator);



        internal unsafe static ValueLine[] CreateValueLines(float[] data, int startX, int startY, int xSize, int ySize, int pitch,
           float[] anaValue, bool isCrossoverAutoOffset, byte smoothCount, float undefValue = 9999.0F,
           float lon = 0, float lat = 0, float lonIn = 1f, float latIn = 1f, Action<ValueLine> callback = null)
        {
            fixed (float* pp = &data[0])
            {
                if (callback == null)
                {
                    return CreateValueLinesDo(pp, startX, startY, xSize, ySize, pitch, anaValue, isCrossoverAutoOffset, smoothCount, undefValue, lon, lat, lonIn, latIn);
                }
                else
                {
                    return CreateValueLinesDoRealTime(pp, startX, startY, xSize, ySize, pitch, anaValue, isCrossoverAutoOffset, smoothCount, undefValue, lon, lat, lonIn, latIn, callback);
                }
            }
        }

        private unsafe static ValueLine[] CreateValueLinesDo(float* data, int startX, int startY, int xSize, int ySize, int pitch,
          float[] anaValue, bool isCrossoverAutoOffset, byte smoothCount, float undefValue, float lon, float lat, float lonIn, float latIn)
        {
            var lines = new List<ValueLine>();

            var grid = new MsqGridSlice()
            {
                pData = new IntPtr(data),
                width = xSize,
                height = ySize,
                rowPitch = pitch,
                startX = startX,
                startY = startY,
                longitude = lon,
                latitude = lat,
                lonInterval = lonIn,
                latInterval = latIn
            };

            fixed (float* pp = &anaValue[0])
            {
                void OnTracings(IntPtr userData, IntPtr linePtr, int index)
                {
                    var lineMsg = (MsqValueLine)Marshal.PtrToStructure(linePtr, typeof(MsqValueLine));
                    var line = new float[lineMsg.pointCount * 2];
                    Marshal.Copy(lineMsg.px, line, 0, lineMsg.pointCount * 2);
                    ValueLine one_line = new ValueLine() { LinePoints = line, Value = lineMsg.value };
                    lines.Add(one_line);
                }

                IntPtr anaValuePtr = new IntPtr(pp);

                var tr = new OnTracing(OnTracings);
                byte isC = (byte)(isCrossoverAutoOffset ? 1 : 0);
                var paras = new MsqTracePramas()
                {
                    lineFunc = Marshal.GetFunctionPointerForDelegate(tr),
                    analysisValueCount = anaValue.Length,
                    analysisValues = anaValuePtr,
                    invalidValue = undefValue,
                    isCrossoverAutoOffset = isC,
                    smoothInterpolationCount = smoothCount,
                };

                int valueCount = anaValue.Length;

                int retArray;
                retArray = msq_trace_contours(grid, paras, IntPtr.Zero);
                GC.KeepAlive(tr);
            }

            return lines.ToArray();
        }

        private unsafe static ValueLine[] CreateValueLinesDoRealTime(float* data, int startX, int startY, int xSize, int ySize, int pitch,
          float[] anaValue, bool isCrossoverAutoOffset, byte smoothCount, float undefValue, float lon, float lat, float lonIn, float latIn,
          Action<ValueLine> callback)
        {

            var grid = new MsqGridSlice()
            {
                pData = new IntPtr(data),
                width = xSize,
                height = ySize,
                rowPitch = pitch,
                startX = startX,
                startY = startY,
                longitude = lon,
                latitude = lat,
                lonInterval = lonIn,
                latInterval = latIn
            };

            fixed (float* pp = &anaValue[0])
            {
                void OnTracings(IntPtr userData, IntPtr linePtr, int index)
                {
                    var lineMsg = (MsqValueLine)Marshal.PtrToStructure(linePtr, typeof(MsqValueLine));
                    var line = new float[lineMsg.pointCount * 2];
                    Marshal.Copy(lineMsg.px, line, 0, lineMsg.pointCount * 2);
                    callback.Invoke(new ValueLine() { Value = lineMsg.value, LinePoints = line });
                }

                IntPtr anaValuePtr = new IntPtr(pp);

                var tr = new OnTracing(OnTracings);
                byte isC = (byte)(isCrossoverAutoOffset ? 1 : 0);
                var paras = new MsqTracePramas()
                {
                    lineFunc = Marshal.GetFunctionPointerForDelegate(tr),
                    analysisValueCount = anaValue.Length,
                    analysisValues = anaValuePtr,
                    invalidValue = undefValue,
                    isCrossoverAutoOffset = isC,
                    smoothInterpolationCount = smoothCount,
                };

                int valueCount = anaValue.Length;

                int retArray;
                retArray = msq_trace_contours(grid, paras, IntPtr.Zero);
                GC.KeepAlive(tr);
            }

            return null;
        }

        private struct MsqValueLine
        {
            public IntPtr px;
            public IntPtr py;
            public IntPtr pz;
            public int nx;
            public int ny;
            public int nz;
            public int pointCount;
            public float value;
        };

        private struct MsqTracePramas
        {
            public IntPtr beginLineGroupFunc;
            public IntPtr lineFunc;
            public IntPtr endLineGroupFunc;
            public IntPtr userData;
            public IntPtr analysisValues;
            public int analysisValueCount;
            public float invalidValue;
            public byte isCrossoverAutoOffset;
            public byte smoothInterpolationCount;
        };

        private struct MsqGridSlice
        {
            public IntPtr pData;
            public int startX;
            public int startY;
            public int width;
            public int height;
            public int rowPitch;
            public float longitude;
            public float latitude;
            public float lonInterval;
            public float latInterval;
        }


    }
}
