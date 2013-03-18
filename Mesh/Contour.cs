using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class Contour
    {
        public List<Curve> Curves { get; set; }
        private DivisionMethod divisionMethod;
        private double elementSize = -1.0;
        private int elementCount = -1;

        public Contour(double size)
        {
            this.Curves = new List<Curve>();
            this.divisionMethod = DivisionMethod.elementSize;
            this.elementSize = size;
        }

        public Contour(int count)
        {
            this.Curves = new List<Curve>();
            this.divisionMethod = DivisionMethod.elementCount;
            this.elementCount = count;
        }

        /// <summary>
        /// Divides the contour into discrete points based on the given
        /// element size or element count.
        /// </summary>
        public List<Point> Divide()
        {
            List<Point> contourPoints = new List<Point>();

            // Number of elements for each curve
            List<int> curveElementNumbers = getCurveElementNumbers();

            // Divide each curve in this contour.
            foreach (Curve curve in this.Curves)
            {
                List<Point> curvePoints = null;

                if (divisionMethod == DivisionMethod.elementSize)
                {
                    curvePoints = curve.Divide(elementSize);
                }
                else if (divisionMethod == DivisionMethod.elementCount)
                {
                    curvePoints = curve.Divide(curveElementNumbers[0]);
                    curveElementNumbers.RemoveAt(0);
                }

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

        /// <summary>
        /// Returns a list that contains the number of elements for each curve
        /// allocated from the total elements of the contour.
        /// </summary>
        /// <returns></returns>
        private List<int> getCurveElementNumbers()
        {
            List<int> numbers = new List<int>();
            double contourLength = this.GetLength();
            int remainingCount = this.elementCount;

            // Distribute contour elements among the curves proportionally.
            for (int i = 0; i < this.Curves.Count; i++)
            {
                if (i == this.Curves.Count - 1)
                {
                    numbers.Add(remainingCount);
                    break;
                }
                double proportion = (Curves[i].GetLength() / contourLength);
                int number = (int)(proportion * this.elementCount);
                numbers.Add(number);
                remainingCount -= number;
            }
            return numbers;
        }
    }

    public enum DivisionMethod
    {
        elementSize, elementCount, indeterminate
    }
}
