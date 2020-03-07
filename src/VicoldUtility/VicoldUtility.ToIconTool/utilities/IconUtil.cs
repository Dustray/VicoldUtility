using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.ToIconTool.utilities
{
    class IconUtil
    {
        /// <summary>
        /// Icon图像信息类
        /// </summary>
        public class IconInfo
        {
            public ushort Width = 16;         // 图像宽度
            public ushort Height = 16;        // 图像高度
            public ushort ColorNum = 0;       // 图像中的颜色数
            public ushort Reserved = 0;       // 保留字
            public ushort Planes = 1;       // 为目标设备说明位面数
            public ushort PixelBit = 32;    // 每个像素素所占位数

            public uint ImageSize = 0;      // 图像字节大小
            public uint ImageOffset = 0;    // 图形数据起点偏移位置

            public byte[] ImageData;        // 图形数据

            /// <summary>
            /// 创建默认的Icon图像数据结构
            /// </summary>
            public IconInfo() { }
        }

        /// <summary>
        /// 从pic创建Icon信息, 生成Icon的尺寸为rect
        /// </summary>
        public IconInfo creatIconInfo(Bitmap pic, Rectangle rect)
        {
            //int w = pic.Width > 260 ? 260 : pic.Width;
            //int h = pic.Height > 260 ? 260 : pic.Height;
            //if (rect == Rectangle.Empty || rect.Width > 260 || rect.Height > 260) rect = new Rectangle(0, 0, w, h);

            //rect = new Rectangle(0, 0, pic.Width, pic.Height);

            // 创建最适尺寸的图像
            Bitmap IconBitmap = new Bitmap(rect.Width, rect.Height);
            Graphics g = Graphics.FromImage(IconBitmap);
            g.DrawImage(pic, rect, new Rectangle(0, 0, pic.Width, pic.Height), GraphicsUnit.Pixel);
            g.Dispose();

            // 以位图的形式保存到内存流中
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            IconBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);

            // 从位图创建Icon属性
            IconInfo iconInfo1 = new IconInfo();
            iconInfo1.Width = (ushort)rect.Width;
            iconInfo1.Height = (ushort)rect.Height;

            // 获取图形数据
            memoryStream.Position = 14;
            iconInfo1.ImageData = new byte[memoryStream.Length - memoryStream.Position];
            memoryStream.Read(iconInfo1.ImageData, 0, iconInfo1.ImageData.Length);

            // Icon图像的高是BMP的2倍
            byte[] Height = BitConverter.GetBytes((uint)iconInfo1.Height * 2);
            iconInfo1.ImageData[8] = Height[0];
            iconInfo1.ImageData[9] = Height[1];
            iconInfo1.ImageData[10] = Height[2];
            iconInfo1.ImageData[11] = Height[3];

            iconInfo1.ImageSize = (uint)iconInfo1.ImageData.Length;
            iconInfo1.ImageOffset = 6 + (uint)(1 * 16);

            return iconInfo1;
        }

        /// <summary>
        /// 保存pic为Icon图像,保存文件路径名称FileName
        /// </summary>
        public void SaveToIcon(Bitmap pic, string FileName)
        {
            SaveToIcon(pic, Rectangle.Empty, FileName);
        }

        /// <summary>
        /// 保存pic为Icon图像,尺寸Size，保存文件路径名称PathName
        /// </summary>
        public void SaveToIcon(Bitmap pic, Size size, string PathName)
        {
            SaveToIcon(pic, new Rectangle(0, 0, size.Width, size.Height), PathName);
        }

        /// <summary>
        /// 保存pic为Icon图像,尺寸rect，保存文件路径名称PathName
        /// </summary>
        public void SaveToIcon(Bitmap pic, Rectangle rect, string PathName)
        {
            // Icon图像最大尺寸260
            //if (rect.Width > 260 || rect.Height > 260) return;

            // 获取Icon信息
            IconInfo iconInfo = creatIconInfo(pic, rect);

            // 创建文件输出流，写入文件，生成Icon图像
            System.IO.FileStream stream = new System.IO.FileStream(PathName, System.IO.FileMode.Create);

            // 写入Icon固定部分
            ushort Reserved = 0;
            ushort Type = 1;
            ushort Count = 1;

            byte[] Temp = BitConverter.GetBytes(Reserved);
            stream.Write(Temp, 0, Temp.Length);
            Temp = BitConverter.GetBytes(Type);
            stream.Write(Temp, 0, Temp.Length);
            Temp = BitConverter.GetBytes((ushort)Count);
            stream.Write(Temp, 0, Temp.Length);

            // 写入Icon头信息
            //stream.WriteByte(iconInfo.Width);
            //stream.WriteByte(iconInfo.Height);
            //stream.WriteByte((byte)iconInfo.ColorNum);
            //stream.WriteByte((byte)iconInfo.Reserved);

            //stream.WriteByte((byte)0);
            //stream.WriteByte((byte)0);

            stream.WriteByte((byte)(iconInfo.Width < 256 ? iconInfo.Width : 0));
            stream.WriteByte((byte)(iconInfo.Height < 256 ? iconInfo.Height : 0));
            stream.WriteByte((byte)iconInfo.ColorNum);
            stream.WriteByte((byte)iconInfo.Reserved);



            Temp = BitConverter.GetBytes(iconInfo.Planes);
            stream.Write(Temp, 0, Temp.Length);
            Temp = BitConverter.GetBytes(iconInfo.PixelBit);
            stream.Write(Temp, 0, Temp.Length);
            Temp = BitConverter.GetBytes(iconInfo.ImageSize);
            stream.Write(Temp, 0, Temp.Length);
            Temp = BitConverter.GetBytes(iconInfo.ImageOffset);
            stream.Write(Temp, 0, Temp.Length);

            // 写入图形数据
            stream.Write(iconInfo.ImageData, 0, iconInfo.ImageData.Length);

            stream.Close();
        }

    }
}
