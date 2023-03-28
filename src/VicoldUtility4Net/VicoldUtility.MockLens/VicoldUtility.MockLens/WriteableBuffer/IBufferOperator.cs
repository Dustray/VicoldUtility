using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VicoldUtility.MockLens.WriteableBuffer
{
    internal interface IBufferOperator
    {
        int ID { get; }
        void Compute(ref BufferIterator bufferIterator);
        UIElement? GetOperatorControl();
    }
}
