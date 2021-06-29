using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using VicoldGis.Algorithms.Entities;

namespace VicoldGis.Algorithms
{
    public struct MsqContourLine
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

    public struct MsqTracePramas
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

    public struct MsqGridSlice
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

    public static class ContourVicoldAlgo
    {
        [System.Security.SuppressUnmanagedCodeSecurity()]
        [DllImport(@".\Libs\msq.dll", EntryPoint = "msq_trace_contours", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern int msq_trace_contours(MsqGridSlice gridSlice, MsqTracePramas tracePramas, IntPtr allocator);


        public unsafe static ContourLine[] CreateContourLines(float[] data, int startX, int startY, int xSize, int ySize, int pitch,
           float[] anaValue, bool isCrossoverAutoOffset, byte smoothCount, float undefValue = 9999.0F, bool isDebug = false)
        {
            fixed (float* pp = &data[0])
            {
                return CreateContourLinesDo(pp, startX, startY, xSize, ySize, pitch, anaValue, isCrossoverAutoOffset, smoothCount, undefValue, 0, 0, 1f, 1f, isDebug);
            }
        }
        public unsafe static ContourLine[] CreateContourLines(float[] data, int startX, int startY, int xSize, int ySize, int pitch,
           float[] anaValue, bool isCrossoverAutoOffset, byte smoothCount, float undefValue = 9999.0F,
           float lon = 0, float lat = 0, float lonIn = 1f, float latIn = 1f, bool isDebug = false)
        {
            fixed (float* pp = &data[0])
            {
                return CreateContourLinesDo(pp, startX, startY, xSize, ySize, pitch, anaValue, isCrossoverAutoOffset, smoothCount, undefValue, lon, lat, lonIn, latIn, isDebug);
            }
        }

        delegate void OnTracing(IntPtr userData, IntPtr linePtr, int index);
        private unsafe static ContourLine[] CreateContourLinesDo(float* data, int startX, int startY, int xSize, int ySize, int pitch,
          float[] anaValue, bool isCrossoverAutoOffset, byte smoothCount, float undefValue = 9999.0F,
          float lon = 0, float lat = 0, float lonIn = 1f, float latIn = 1f, bool isDebug = false)
        {
            var lines = new List<ContourLine>();

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
                    var lineMsg = (MsqContourLine)Marshal.PtrToStructure(linePtr, typeof(MsqContourLine));
                    var line = new float[lineMsg.pointCount * 2];
                    Marshal.Copy(lineMsg.px, line, 0, lineMsg.pointCount * 2);
                    ContourLine one_line = new ContourLine() { LinePoints = line, Value = lineMsg.value };
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
    }
}