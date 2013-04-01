using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class Segment : LineBase
    {
        public Segment(Point start, Point end)
            : base(start, end) 
        {
            this.length = GetLength();
        }

        /// <summary>
        /// Rotates the end point of the segment by 60 degrees, so that the rotated 
        /// and the original endpoints of the segment form a regular triangle.
        /// </summary>
        public Point RotateInward60()
        {
            return Transformation.Rotate(this.End, this.Start, 60);
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
            return (new Vector(this.End) - new Vector(this.Start)).GetLength();
        }

        /// <summary>
        /// Get the internal point of segment line at parameter t running from 0 to 1.
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
    }
}
