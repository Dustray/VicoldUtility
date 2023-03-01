using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VicoldUtility.MockLens.Utilities;

namespace VicoldUtility.MockLens.WriteableBuffer.BufferOperators
{
    internal class ContrastOperator : IBufferOperator, IBufferUpdate<float>
    {
        private float _value;

        public void Compute(ref BufferIterator bufferIterator)
        {
            bufferIterator.ForEach((b, g, r, a) =>
            {
                return (ChannelUtility.DataNumericalLegitimacy(b + (((255 / 2) - b) * _value)),
                        ChannelUtility.DataNumericalLegitimacy(g + (((255 / 2) - g) * _value)),
                        ChannelUtility.DataNumericalLegitimacy(r + (((255 / 2) - r) * _value)),
                        a);
            });
        }

        public void Update(float value)
        {
            _value = value;
        }
    }
}
