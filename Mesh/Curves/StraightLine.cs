using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Mesh.Enum;

namespace Mesh.Curves
{
    /// <summary>
    /// Straight line connecting two 3D points.
    /// </summary>
    public class StraightLine : Curve
    {
        public StraightLine(Point start, Point end, int elementCount)
            : base(start, end, elementCount) 
        {
            this.length = GetLength();
        }

        public StraightLine(Point start, Point end, double elementSize)
            : base(start, end, elementSize) 
        {
            this.length = GetLength();
        }

        /// <summary>
        /// Returns the length of the line.
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
        /// Get the internal point of the line at parameter t running from 0 to 1.
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Divides the line either by element size or by element count.
        /// </summary>
        public override List<Point> Divide(out double distance)
        {
            if (this.divisionMethod == DivisionMethod.ElementCount)
            {
                return DivideCount(out distance);
            }
            return DivideSize(out distance);
        }

        /// <summary>
        /// Divides the line into segments with approximately the specified segment size.
        /// </summary>
        private List<Point> DivideSize(out double distance)
        {
            // Calculate how many elements fit on this line
            this.elementCount = (int)Math.Floor(this.GetLength() / this.elementSize) + 1;
            return DivideCount(out distance);
        }

        /// <summary>
        /// Divides the line into the specified number of elements.
        /// </summary>
        private List<Point> DivideCount(out double distance)
        {
            if (this.elementCount < 1)
            {
                distance = -1;
                return null;
            }

            List<Point> points = new List<Point>();
            points.Add(this.Start);

            // Relative size of an element in the [0,1] interval.
            double d = 1.0 / this.elementCount;

            // Get the internal points.
            for (int i = 1; i < this.elementCount; i++)
            {
                points.Add(this.GetPoint(i * d));
            }
            points.Add(this.End);

            distance = this.GetLength() / this.elementCount;
            return points;
        }
    }
}
