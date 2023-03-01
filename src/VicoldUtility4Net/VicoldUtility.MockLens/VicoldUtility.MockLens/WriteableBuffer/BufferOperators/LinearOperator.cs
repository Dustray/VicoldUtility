using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VicoldUtility.MockLens.Utilities;

namespace VicoldUtility.MockLens.WriteableBuffer.BufferOperators
{
    internal class LinearOperator : IBufferOperator, IBufferUpdate<int>
    {
        private int _value;

        public void Compute(ref BufferIterator bufferIterator)
        {
            bufferIterator.ForEach((b, g, r, a) =>
            {
                return (ChannelUtility.DataNumericalLegitimacy(b + _value),
                        ChannelUtility.DataNumericalLegitimacy(g + _value),
                        ChannelUtility.DataNumericalLegitimacy(r + _value), 
                        a);
            });
        }

        public void Update(int value)
        {
            _value = value;
        }
    }
}
