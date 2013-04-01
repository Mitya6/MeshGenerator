using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public static class Transformation
    {
        /// <summary>
        /// Rotate point in X,Y plane around given center.
        /// </summary>
        public static Point Rotate(Point point, Point center, double degrees)
        {
            double radians = degrees * (Math.PI / 180);
            double cosTheta = Math.Cos(radians);
            double sinTheta = Math.Sin(radians);
            double x = cosTheta * (point.X - center.X) - sinTheta * (point.Y - center.Y) + center.X;
            double y = sinTheta * (point.X - center.X) + cosTheta * (point.Y - center.Y) + center.Y;
            return new Point(x, y, 0.0);
        }
    }
}
