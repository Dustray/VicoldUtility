using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VicoldUtility.MockLens.ChannelColor;
using VicoldUtility.MockLens.Utilities;
using VicoldUtility.MockLens.WriteableBuffer.OperatorControls;

namespace VicoldUtility.MockLens.WriteableBuffer.BufferOperators
{
    internal class ContrastOperator : IBufferOperator
    {
        private BaseSlider _slider;
        private ChannelType _channelType;
        private const int _contrastCenter = 128;

        public int ID { get; } = Guid.NewGuid().GetHashCode();

        public ContrastOperator(string display, ChannelType chanelType)
        {
            _channelType = chanelType;
            _slider = new BaseSlider(ID,display, new SliderParams(-0.5, 2, 0, 0.05, 2));
        }

        public void Compute(ref BufferIterator bufferIterator)
        {
            // todo 未调用此opt就跳过
            var value = _slider.CurrentValue;
            bufferIterator.ForEach((b, g, r, a) =>
            {
                b = ChannelUtility.IsChannelMatched(_channelType, ChannelType.Blue) ?
                ChannelUtility.DataNumericalLegitimacy(b + ((b - _contrastCenter) * value)) : b;
                g = ChannelUtility.IsChannelMatched(_channelType, ChannelType.Green) ?
                ChannelUtility.DataNumericalLegitimacy(g + ((g - _contrastCenter) * value)) : g;
                r = ChannelUtility.IsChannelMatched(_channelType, ChannelType.Red) ?
                ChannelUtility.DataNumericalLegitimacy(r + ((r - _contrastCenter) * value)) : r;
                return (b, g, r, a);
            });
        }

        public UIElement? GetOperatorControl()
        {
            return _slider;
        }
    }
}
