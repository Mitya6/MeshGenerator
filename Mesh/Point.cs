﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Mesh
{
    /// <summary>
    /// 3D point class.
    /// </summary>
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool IsEdgePoint { get; set; }
        public List<Triangle> Triangles { get; set; }
        public List<Segment> Segments { get; set; }

        public Point(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.IsEdgePoint = false;
            this.Triangles = new List<Triangle>();
            this.Segments = new List<Segment>();
        }

        /// <summary>
        /// Converts a Point object to an equivalent Vector3D object.
        /// </summary>
        public Vector3D ToVector3D()
        {
            return new Vector3D(this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Calculates the 3D distance of two points.
        /// </summary>
        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) +
                (p1.Y - p2.Y) * (p1.Y - p2.Y) + (p1.Z - p2.Z) * (p1.Z - p2.Z));
        }

        /// <summary>
        /// Sorts the points in ascending order by their distance from the centre.
        /// </summary>
        public static void SortByDistance(List<Point> points, Point centre)
        {
            Comparison<Point> compfunc = ((p1, p2) =>
            {
                if (Point.Distance(p1, centre) < Point.Distance(p2, centre))
                {
                    return -1;
                }
                return 1;
            });
            points.Sort(compfunc);
        }
    }
}
