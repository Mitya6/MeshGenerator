using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mesh.Curves;
using Mesh.Enum;

namespace Mesh.Curves
{
    /// <summary>
    /// Abstract class for line, circle-arc and other contour types.
    /// </summary>
    public abstract class Curve : LineBase
    {
        protected DivisionMethod divisionMethod;
        protected int elementCount;
        protected double elementSize;

        public Curve(Point s, Point e, int elementCount)
            : base(s, e)
        {
            this.divisionMethod = DivisionMethod.ElementCount;
            this.elementCount = elementCount;
            this.elementSize = -1.0;
        }

        public Curve(Point s, Point e, double elementSize)
            : base(s, e)
        {
            this.divisionMethod = DivisionMethod.ElementSize;
            this.elementCount = -1;
            this.elementSize = elementSize;
        }

        public abstract List<Point> Divide();
    }
}
