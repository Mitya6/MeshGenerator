using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mesh.Curves;
using Mesh.Enum;

namespace Mesh
{
    /// <summary>
    /// Represents an inner or outer border of a plane figure.
    /// </summary>
    public class Contour
    {
        public List<Curve> Curves { get; set; }
        public DivisionMethod DivisionMethod { get; set; }
        public double ElementSize { get; set; }
        public ContourTypes ContourType { get; set; }

        public Contour(ContourTypes type)
        {
            this.Curves = new List<Curve>();
            this.DivisionMethod = DivisionMethod.Indeterminate;
            this.ContourType = type;
        }

        public Contour(double size, ContourTypes type)
        {
            this.Curves = new List<Curve>();
            this.DivisionMethod = DivisionMethod.ElementSize;
            this.ElementSize = size;
            this.ContourType = type;
        }

        /// <summary>
        /// Divides the contour into discrete points.
        /// </summary>
        public List<Point> Divide()
        {
            List<Point> contourPoints = new List<Point>();

            // Divide each curve in this contour.
            foreach (Curve curve in this.Curves)
            {
                List<Point> curvePoints = curve.Divide();

                // Remove last point of the curve, it is the same as the 
                // first point of the next curve
                curvePoints.RemoveAt(curvePoints.Count - 1);

                // Append the divided curve points to the divided contour points.
                contourPoints.AddRange(curvePoints);
            }

            return contourPoints;
        }

        /// <summary>
        /// Returns the total length of this contour.
        /// </summary>
        public double GetLength()
        {
            double length = 0.0;
            foreach (Curve curve in this.Curves)
            {
                length += curve.GetLength();
            }
            return length;
        }

        public List<Point> GetContourPoints()
        {
            List<Point> pts = new List<Point>();
            foreach (Curve curve in this.Curves)
            {
                pts.Add(curve.Start);
            }
            return pts;
        }
    }       
}
