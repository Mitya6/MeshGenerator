using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Mesh
{
    public class Triangle
    {
        public Point[] Points { get; set; }

        public Triangle(Point a, Point b, Point c)
        {
            Points = new Point[3];
            Points[0] = a;
            Points[1] = b;
            Points[2] = c;
        }
    }
}
