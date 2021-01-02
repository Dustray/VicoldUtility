using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VicoldGis.VMap.Projections
{
    public class MercatorProj : IProjection
    {
        const float kLon0 = 110;
        public Point Project(double x, double y)
        {
            var isLeft = x < 0;
            if (x < 0) x += 360;
            x -= kLon0;
            if (x > 180)
            {
                x -= 360;
            }
            else if (x == 180)
            {
                x *= isLeft ? -1 : 1;
            }
            return new Point(x, -y);
        }
    }
}
