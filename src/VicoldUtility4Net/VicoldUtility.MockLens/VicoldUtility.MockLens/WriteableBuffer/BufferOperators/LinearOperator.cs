using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VicoldUtility.MockLens.Utilities;
using VicoldUtility.MockLens.WriteableBuffer.OperatorControls;

namespace VicoldUtility.MockLens.WriteableBuffer.BufferOperators
{
    internal class LinearOperator : IBufferOperator
    {
        private BaseSlider _slider;

        public LinearOperator()
        {
            _slider = new BaseSlider(ID, "曝光", new SliderParams(-100, 100, 0, 1, 2));
        }

        public int ID { get; } = Guid.NewGuid().GetHashCode();

        public void Compute(ref BufferIterator bufferIterator)
        {
            // todo 未调用此opt就跳过
            var value = _slider.CurrentValue;
            bufferIterator.ForEach((b, g, r, a) =>
            {
                return (ChannelUtility.DataNumericalLegitimacy(b + value),
                        ChannelUtility.DataNumericalLegitimacy(g + value),
                        ChannelUtility.DataNumericalLegitimacy(r + value),
                        a);
            });
        }

        //public void Update(int value)
        //{
        //    _value = value;
        //}

        public UIElement? GetOperatorControl()
        {
            return null;
        }
    }
}
