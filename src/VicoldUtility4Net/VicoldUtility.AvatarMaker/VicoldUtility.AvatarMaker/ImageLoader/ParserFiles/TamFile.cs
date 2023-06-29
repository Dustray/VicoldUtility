using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.AvatarMaker.ImageLoader.ParserFiles
{
    /// <summary>
    /// type and mask
    /// </summary>
    internal class TamFile
    {
        // Constants for the file header
        private const int HeaderSize = 16;
        private const int MagicNumber = 0x54414D20; // "TAM "
        private const int Version = 1; // File format version
        private const int ByteType = 1;
        private const int ShortType = 2;
        private const int IntType = 3;
        private const int LongType = 4;

        // Properties for the file format
        public int ArrayType { get; set; }
        public int ArraySizeX { get; set; }
        public int ArraySizeY { get; set; }

        // Private fields for file handling
        private FileStream fileStream;
        private BinaryReader binaryReader;
        private BinaryWriter binaryWriter;

        public TamFile(string fileName)
        {
            fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
            binaryReader = new BinaryReader(fileStream);
            binaryWriter = new BinaryWriter(fileStream);
        }

        public void ReadHeader()
        {
            // Check if file stream is open
            if (fileStream == null)
            {
                throw new Exception("File is not open");
            }

            // Read header data from file
            int magicNumber = binaryReader.ReadInt32();
            int version = binaryReader.ReadInt32();

            // Check if file format is supported
            if (magicNumber != MagicNumber || version != Version)
            {
                throw new Exception("Unsupported file format");
            }

            // Read array properties
            ArrayType = binaryReader.ReadInt32();
            ArraySizeX = binaryReader.ReadInt32();
            ArraySizeY = binaryReader.ReadInt32();
        }

        public void WriteHeader()
        {
            // Check if file stream is open
            if (fileStream == null)
            {
                throw new Exception("File is not open");
            }

            // Write header data to file
            binaryWriter.Write(MagicNumber);
            binaryWriter.Write(Version);

            // Write array properties
            binaryWriter.Write(ArrayType);
            binaryWriter.Write(ArraySizeX);
            binaryWriter.Write(ArraySizeY);
        }

        public T[,] ReadArray<T>()
        {
            // Check if file stream is open
            if (fileStream == null)
            {
                throw new Exception("File is not open");
            }

            // Check if array type is supported
            if (typeof(T) != typeof(byte) &&
                typeof(T) != typeof(short) &&
                typeof(T) != typeof(int) &&
                typeof(T) != typeof(long))
            {
                throw new Exception("Invalid array type");
            }

            // Calculate size of array element and allocate memory
            int elementSize = 0;
            if (typeof(T) == typeof(byte)) elementSize = 1;
            else if (typeof(T) == typeof(short)) elementSize = 2;
            else if (typeof(T) == typeof(int)) elementSize = 4;
            else if (typeof(T) == typeof(long)) elementSize = 8;
            T[,] array = new T[ArraySizeX, ArraySizeY];

            // Read array data from file
            for (int y = 0; y < ArraySizeY; y++)
            {
                for (int x = 0; x < ArraySizeX; x++)
                {
                    if (typeof(T) == typeof(byte)) array[x, y] = (T)(object)binaryReader.ReadByte();
                    else if (typeof(T) == typeof(short)) array[x, y] = (T)(object)binaryReader.ReadInt16();
                    else if (typeof(T) == typeof(int)) array[x, y] = (T)(object)binaryReader.ReadInt32();
                    else if (typeof(T) == typeof(long)) array[x, y] = (T)(object)binaryReader.ReadInt64();
                }
            }

            return array;
        }

        public void WriteArray<T>(T[,] array)
        {
            // Check if file stream is open
            if (fileStream == null)
            {
                throw new Exception("File is not open");
            }

            // Check if array type is supported
            if (typeof(T) != typeof(byte) &&
                typeof(T) != typeof(short) &&
                typeof(T) != typeof(int) &&
                typeof(T) != typeof(long))
            {
                throw new Exception("Invalid array type");
            }

            // Write array data to file
            for (int y = 0; y < ArraySizeY; y++)
            {
                for (int x = 0; x < ArraySizeX; x++)
                {
                    if (typeof(T) == typeof(byte)) binaryWriter.Write((byte)(object)array[x, y]);
                    else if (typeof(T) == typeof(short)) binaryWriter.Write((short)(object)array[x, y]);
                    else if (typeof(T) == typeof(int)) binaryWriter.Write((int)(object)array[x, y]);
                    else if (typeof(T) == typeof(long)) binaryWriter.Write((long)(object)array[x, y]);
                }
            }
        }

        public void Close()
        {
            // Close file stream
            binaryReader.Close();
            binaryWriter.Close();
            fileStream.Close();
        }
    }
}
