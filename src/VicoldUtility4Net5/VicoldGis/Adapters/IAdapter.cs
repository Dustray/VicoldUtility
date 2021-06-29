using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VicoldGis.Adapters
{
    public interface IAdapter
    {
        void Parse();
        List<FrameworkElement> GetViewData();
    }
}
