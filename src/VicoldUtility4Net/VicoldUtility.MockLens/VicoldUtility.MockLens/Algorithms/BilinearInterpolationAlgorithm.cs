using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.MockLens.Algorithms
{
    /// <summary>
    /// 双线性插值算法
    /// </summary>
    public static class BilinearInterpolationAlgorithm
    {
        private struct CellInfo
        {
            internal int minX;
            internal int maxX;
            internal int minY;
            internal int maxY;
            internal float indexX;
            internal float indexY;
            internal CellInfo(float indexX, float indexY)
            {
                this.indexX = indexX;
                this.indexY = indexY;
                minX = (int)Math.Floor(indexX);//x上取整
                maxX = (int)Math.Ceiling(indexX);//x下取整
                minY = (int)Math.Floor(indexY);//y上取整
                maxY = (int)Math.Ceiling(indexY);//y下取整
            }
        }

        /// <summary>
        /// 双线性插值算法
        /// </summary>
        /// <param name="matrix">一维矩阵数组</param>
        /// <param name="width">矩阵宽</param>
        /// <param name="height">矩阵高</param>
        /// <param name="indexX">所求索引点横坐标</param>
        /// <param name="indexY">所求索引点纵坐标</param>
        /// <param name="invalidValue">无效值</param>
        /// <returns></returns>
        public static byte Execute(byte[] matrix, int width, int height, float indexX, float indexY)
        {
            var cell = new CellInfo(indexX, indexY);
            return Execute(cell, matrix, width, height);
        }

        /// <summary>
        /// 双线性插值算法
        /// </summary>
        /// <param name="matrix">二维矩阵数组</param>
        /// <param name="indexX">所求索引点横坐标</param>
        /// <param name="indexY">所求索引点纵坐标</param>
        /// <param name="invalidValue">无效值</param>
        /// <returns></returns>
        public static byte Execute(byte[,] matrix, float indexX, float indexY)
        {
            var cell = new CellInfo(indexX, indexY);
            return Execute(cell, matrix);
        }

        /// <summary>
        /// 双线性插值算法
        /// </summary>
        /// <param name="leftTop">左上值</param>
        /// <param name="rightTop">右上值</param>
        /// <param name="leftBottom">左下值</param>
        /// <param name="rightBottom">右下值</param>
        /// <param name="indexX">所求索引点横坐标</param>
        /// <param name="indexY">所求索引点纵坐标</param>
        /// <param name="invalidValue">无效值</param>
        /// <returns></returns>
        public static byte Execute(byte leftTop, byte rightTop, byte leftBottom, byte rightBottom, float indexX, float indexY)
        {
            var cell = new CellInfo(indexX, indexY);
            return Execute(cell, leftTop, rightTop, leftBottom, rightBottom);
        }

        #region BilinearInterpolation 内部计算方法

        /// <summary>
        /// 执行双线性插值算法
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static byte Execute(CellInfo cell, byte[] matrix, int width, int height)
        {
            int exchangeIndex(int x, int y) => y * width + x;
            var leftTop = matrix[exchangeIndex(cell.minX, cell.minY)];//左上 整数点值
            var rightTop = matrix[exchangeIndex(cell.maxX, cell.minY)];//右上 整数点值
            var leftBottom = matrix[exchangeIndex(cell.minX, cell.maxY)];//左下 整数点值
            var rightBottom = matrix[exchangeIndex(cell.maxX, cell.maxY)];//右下 整数点值
            return Compute(cell, leftTop, rightTop, leftBottom, rightBottom);
        }

        /// <summary>
        /// 执行双线性插值算法
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static byte Execute(CellInfo cell, byte[,] matrix)
        {
            var leftTop = matrix[cell.minX, cell.minY];
            var rightTop = matrix[cell.maxX, cell.minY];
            var leftBottom = matrix[cell.minX, cell.maxY];
            var rightBottom = matrix[cell.maxX, cell.maxY];
            return Compute(cell, leftTop, rightTop, leftBottom, rightBottom);
        }

        /// <summary>
        /// 执行双线性插值算法
        /// </summary>
        /// <param name="leftTop"></param>
        /// <param name="rightTop"></param>
        /// <param name="leftBottom"></param>
        /// <param name="rightBottom"></param>
        /// <returns></returns>
        private static byte Execute(CellInfo cell, byte leftTop, byte rightTop, byte leftBottom, byte rightBottom)
        {
            return Compute(cell, leftTop, rightTop, leftBottom, rightBottom);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftTop"></param>
        /// <param name="rightTop"></param>
        /// <param name="leftBottom"></param>
        /// <param name="rightBottom"></param>
        /// <returns></returns>
        private static byte Compute(CellInfo cell, byte leftTop, byte rightTop, byte leftBottom, byte rightBottom)
        {
            float M, N, P;

            //M插值
            M = (cell.maxX - cell.minX) == 0 ? leftTop : (cell.indexX - cell.minX) / (cell.maxX - cell.minX) * (rightTop - leftTop) + leftTop;

            //N插值
            N = (cell.maxX - cell.minX) == 0 ? leftBottom : (cell.indexX - cell.minX) / (cell.maxX - cell.minX) * (rightBottom - leftBottom) + leftBottom;

            //P插值
            P = (cell.maxY - cell.minY) == 0 ? M : (cell.indexY - cell.minY) / (cell.maxY - cell.minY) * (N - M) + M;

            if (P < 0)
            {
                P = 0;
            }
            else if (P > 255)
            {
                P = 255;
            }

            return (byte)Math.Round(P, 0, MidpointRounding.AwayFromZero);
        }

        #endregion

    }
}
