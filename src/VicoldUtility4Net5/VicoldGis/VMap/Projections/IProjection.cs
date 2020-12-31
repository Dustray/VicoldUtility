using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VicoldGis.VMap.Projections
{
    internal interface IProjection
    {
        public Point Project(float x, float y);
    }
}
