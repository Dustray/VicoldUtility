using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VicoldUtility.AvatarMaker.ImageLoader
{
    internal class Layer
    {
        public string Id { get; set; }
        public int Level { get; set; }
        public byte[] ?Data { get; set; }
        public WriteableBitmap ?Bitmap { get; set; }
    }
}
