using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VicoldUtility.MockLens.WriteableBuffer
{
    internal interface IBufferOperator
    {
        void Compute(ref BufferIterator bufferIterator);
    }
}
