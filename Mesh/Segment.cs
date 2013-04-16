using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Mesh
{
    public class Segment : LineBase
    {
        public bool Checked { get; set; }
        
        public Segment(Point start, Point end)
            : base(start, end)
        {
            this.length = GetLength();
            this.Checked = false;
        }

        /// <summary>
        /// Rotates the end point of the segment by 60 degrees, so that the rotated 
        /// and the original endpoints of the segment form a regular triangle.
        /// </summary>
        public Point RotateInward(double degrees)
        {
            return Transformation.Rotate(this.End, this.Start, degrees);
        }

        public Point YIntersection(double y)
        {
            // Both points above or below or on y.
            if ((y > this.Start.Y && y > this.End.Y) || (y < this.Start.Y && y < this.End.Y)
                || (y == this.Start.Y && y == this.End.Y))
            {
                return null;
            }

            // (x2-x1)(y-y1) = (y2-y1)(x-x1)
            double x = ((this.End.X - this.Start.X) * (y - this.Start.Y)) /
                (this.End.Y - this.Start.Y) + this.Start.X;

            return new Point(x, y, 0.0);
        }

        /// <summary>
        /// Returns the point that forms the best quality triangle with the
        /// segment endpoints.
        /// </summary>
        public Point GetIdealPoint(double h)
        {
            Vector3D half = (this.Start.ToVector3D() + this.End.ToVector3D()) / 2;
            Vector3D idealLocation = half + h * Normal();
            return new Point(idealLocation.X, idealLocation.Y, idealLocation.Z);
        }

        /// <summary>
        /// Returns the length of the segment.
        /// </summary>
        public override double GetLength()
        {
            // If length has already been calculated.
            if (this.length >= 0)
                return this.length;

            // If length is being calculated for the first time.
            return (this.End.ToVector3D() - this.Start.ToVector3D()).Length;
        }

        /// <summary>
        /// Get the internal point of segment line at parameter t running from 0 to 1.
        /// </summary>
        public override Point GetPoint(double t)
        {
            // t parameter must be within the [0,1] closed interval
            if (t < 0 || t > 1) return null;

            // Convert points to vectors for easier calculation
            Vector3D vStart = this.Start.ToVector3D();
            Vector3D vEnd = this.End.ToVector3D();

            Vector3D result = vStart + t * (vEnd - vStart);
            return new Point(result.X, result.Y, result.Z);
        }

        public override Vector3D Normal()
        {
            Vector3D vDir = this.End.ToVector3D() - this.Start.ToVector3D();
            Vector3D v = Vector3D.CrossProduct(vDir, new Vector3D(0, 0, -1));
            v.Normalize();
            return v;
        }

        public bool Equals(Segment segment)
        {
            // start == start && end == end
            if (this.Start.Equals(segment.Start))
            {
                if (this.End.Equals(segment.End))
                    return true;
            }
            // start == end && end == start (reverse dir)
            if (this.Start.Equals(segment.End))
            {
                if (this.End.Equals(segment.Start))
                    return true;
            }
            return false;
        }


        public Segment Reverse()
        {
            return new Segment(this.End, this.Start);
        }

        public Point Intersection(Segment other)
        {
            // Algo from stackoverflow
            Vector3D p = this.Start.ToVector3D();
            Vector3D q = other.Start.ToVector3D();
            Vector3D s = other.End.ToVector3D() - q;
            Vector3D r = this.End.ToVector3D() - p;
            Vector3D qmp = q - p;

            Vector3D rxs = Vector3D.CrossProduct(r, s);
            Vector3D qmpxr = Vector3D.CrossProduct(qmp, r);
            Vector3D qmpxs = Vector3D.CrossProduct(qmp, s);

            if (qmpxr.Length == 0.0)
            {
                // Collinear
                //p p+r q q+s
                if (p.X > Math.Min(q.X, (q + s).X) && p.X < Math.Max(q.X, (q + s).X) &&
                    p.Y > Math.Min(q.Y, (q + s).Y) && p.Y < Math.Max(q.Y, (q + s).Y))
                    return new Point(p.X, p.Y, 0.0);
                if ((p + r).X > Math.Min(q.X, (q + s).X) && (p + r).X < Math.Max(q.X, (q + s).X) &&
                    (p + r).Y > Math.Min(q.Y, (q + s).Y) && (p + r).Y < Math.Max(q.Y, (q + s).Y))
                    return new Point((p+r).X, (p+r).Y, 0.0);
                if (q.X > Math.Min(p.X, (p + r).X) && q.X < Math.Max(p.X, (p + r).X) &&
                    q.Y > Math.Min(p.Y, (p + r).Y) && q.Y < Math.Max(p.Y, (p + r).Y))
                    return new Point(q.X, q.Y, 0.0);
                if ((q + s).X > Math.Min(p.X, (p + r).X) && (q + s).X < Math.Max(p.X, (p + r).X) &&
                    (q + s).Y > Math.Min(p.Y, (p + r).Y) && (q + s).Y < Math.Max(p.Y, (p + r).Y))
                    return new Point((q + s).X, (q + s).Y, 0.0);
                return null;
            }

            if (rxs.Z == 0.0)
            {
                // Parallel.
                return null;
            }

            double t = qmpxs.Z / rxs.Z;
            double u = qmpxr.Z / rxs.Z;

            if ((t > 0.0 + Geometry.Epsilon) && (t < 1.0 - Geometry.Epsilon) &&
                (u > 0.0 + Geometry.Epsilon) && (u < 1.0 - Geometry.Epsilon))
            {
                return this.GetPoint(t);
            }
            return null;
        }
    }
}
