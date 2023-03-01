using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.MockLens.WriteableBuffer
{
    internal ref struct BufferIterator
    {
        private ReadOnlySpan<byte> _sourceData;
        private Span<byte> _resultData;

        public BufferIterator(ReadOnlySpan<byte> sourceData,Span<byte> resultData)
        {
            _sourceData = sourceData;
            _resultData = resultData;
        }

        public ReadOnlySpan<byte> SourceData => _sourceData;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forAction"></param>
        public void ForEach(Func<byte, byte, byte, byte, (byte, byte, byte, byte)> forAction)
        {
            for (var i = 0; i < _sourceData.Length; i += 4)
            {
                (byte c1, byte c2, byte c3, byte c4) = forAction(_sourceData[i], _sourceData[i + 1], _sourceData[i + 2], _sourceData[i + 3]);
                _resultData[i] = c1;
                _resultData[i + 1] = c2;
                _resultData[i + 2] = c3;
                _resultData[i + 3] = c4;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forAction"></param>
        public void For(Func<int, (byte, byte, byte, byte)> forAction)
        {
            for (var i = 0; i < _sourceData.Length; i += 4)
            {
                (byte c1, byte c2, byte c3, byte c4) = forAction(i);
                _resultData[i] = c1;
                _resultData[i + 1] = c2;
                _resultData[i + 2] = c3;
                _resultData[i + 3] = c4;
            }
        }
    }
}
