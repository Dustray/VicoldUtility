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
    internal class HueOperator : IBufferOperator
    {
        private BaseSlider _slider;
        private ChannelType _channelType;
        private const int _contrastCenter = 240;

        public HueOperator(string display, ChannelType chanelType)
        {
            _channelType = chanelType;
            _slider = new BaseSlider(ID,display, new SliderParams(-1, 1, 0, 0.05, 2));
        }

        public int ID { get; } = Guid.NewGuid().GetHashCode();

        public void Compute(ref BufferIterator bufferIterator)
        {
            // todo 未调用此opt就跳过
            var value = _slider.CurrentValue;
            bufferIterator.ForEach((b, g, r, a) =>
            {
                b = ChannelUtility.IsChannelMatched(_channelType, ChannelType.Blue) ?
                ChannelUtility.DataNumericalLegitimacy(b + ((_contrastCenter - b) * value)) : b;
                g = ChannelUtility.IsChannelMatched(_channelType, ChannelType.Green) ?
                ChannelUtility.DataNumericalLegitimacy(g + ((_contrastCenter - g) * value)) : g;
                r = ChannelUtility.IsChannelMatched(_channelType, ChannelType.Red) ?
                ChannelUtility.DataNumericalLegitimacy(r + ((_contrastCenter - r) * value)) : r;
                return (b, g, r, a);
            });
        }

        public UIElement? GetOperatorControl()
        {
            return _slider;
        }
    }
}
