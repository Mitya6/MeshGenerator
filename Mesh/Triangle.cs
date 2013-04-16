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

        //public bool AreCollinear(Point p1, Point p2, Point p3)
        //{
        //    Vector3D v1 = p2.ToVector3D() - p1.ToVector3D();
        //    Vector3D v2 = p3.ToVector3D() - p1.ToVector3D();
        //    if (Math.Abs(Vector3D.CrossProduct(v1, v2).Length) < Geometry.Epsilon) return true;
        //    return false;
        //}
    }
}
