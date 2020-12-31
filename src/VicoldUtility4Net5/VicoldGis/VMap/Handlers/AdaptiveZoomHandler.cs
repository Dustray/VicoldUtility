using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VicoldGis.VMap.Handlers
{
    /// <summary>
    /// 控件反缩放
    /// </summary>
    internal class AdaptiveAntiZoomHandler
    {
        private FrameworkElement _element;
        /// <summary>
        /// 缩放回调
        /// </summary>
        public Action<double> OnScale;
        public AdaptiveAntiZoomHandler()
        {
        }

        /// <summary>
        /// 进行反缩放
        /// </summary>
        /// <param name="ratio">正缩放比例</param>
        public void AntiScaling(double ratio)
        {
            OnScale?.Invoke(1 / ratio);
        }

    }
}
