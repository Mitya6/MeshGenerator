using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Mesh
{
    /// <summary>
    /// Base class for representing two connected points
    /// </summary>
    public abstract class LineBase
    {
        protected double length;

        public Point Start { get; set; }
        public Point End { get; set; }

        public LineBase(Point start, Point end)
        {
            this.Start = start;
            this.End = end;
            this.length = -1;
        }

        public abstract double GetLength();
        public abstract Point GetPoint(double t);
        public abstract Vector3D Normal();
    }
}
