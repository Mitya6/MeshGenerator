using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.Curves
{
    public class Line : Curve
    {
        public Line(Point start, Point end)
            : base(start, end)
        {

        }

        /// <summary>
        /// Returns the length of the line.
        /// </summary>
        /// <returns></returns>
        public override double GetLength()
        {
            // If length has already been calculated.
            if (this.length >= 0)
                return this.length;

            // If length is being calculated for the first time.
            this.length = (new Vector(this.End) - new Vector(this.Start)).GetLength();
            return this.length;
        }

        /// <summary>
        /// Get the internal point of the line at parameter t running from 0 to 1.
        /// </summary>
        public override Point GetPoint(double t)
        {
            // t parameter must be within the [0,1] closed interval
            if (t < 0 || t > 1) return null;

            // Convert points to vectors for easier calculation
            Vector vStart = new Vector(this.Start);
            Vector vEnd = new Vector(this.End);
            Vector vDiff = vEnd - vStart;

            return (vStart + t * vDiff).ToPoint();
        }

        public override Vector Normal()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Divides the line into segments with the specified segment size.
        /// </summary>
        public override List<Point> Divide(double elementSize)
        {
            List<Point> points = new List<Point>();
            points.Add(this.Start);

            // Element size compared to line length.
            double relativeSize = elementSize / this.GetLength();
            Vector vStart = new Vector(this.Start);
            Vector vEnd = new Vector(this.End);
            Vector vElement = relativeSize * (vEnd - vStart);

            // Add division points until they are part of the line.
            int i = 1;
            Vector vInternalPoint = vStart + i * vElement;
            while ((vInternalPoint - vStart).GetLength() < this.length)
            {
                points.Add(vInternalPoint.ToPoint());
                i++;
                vInternalPoint = vStart + i * vElement;
            };
            points.Add(this.End);

            // If less than half an element size remains at the end of the line
            // then join the last two segments by deleting the last division point.
            if (Point.Distance(points[points.Count - 1], points[points.Count - 2]) < elementSize/2
                && points.Count > 2)
            {
                points.RemoveAt(points.Count - 2);
            }

            return points;
        }

        /// <summary>
        /// Divides the line into the specified number of elements.
        /// </summary>
        public override List<Point> Divide(int elementCount)
        {
            if (elementCount < 1)
                return null;

            List<Point> points = new List<Point>();
            points.Add(this.Start);

            // Relative size of an element in the [0,1] interval.
            double d = 1.0 / elementCount;

            // Get the internal points.
            for (int i = 1; i < elementCount; i++)
            {
                points.Add(this.GetPoint(i * d));
            }
            points.Add(this.End);

            return points;
        }
    }
}
