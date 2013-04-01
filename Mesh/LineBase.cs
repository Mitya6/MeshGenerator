using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    /// <summary>
    /// Base class for representing two connected points
    /// </summary>
    public abstract class LineBase
    {
        protected double length;

        private Point start;
        public Point Start
        {
            get { return start; }
        }

        private Point end;
        public Point End
        {
            get { return end; }
        }

        public LineBase(Point start, Point end)
        {
            this.start = start;
            this.end = end;
            this.length = -1;
        }

        public abstract double GetLength();
        public abstract Point GetPoint(double t);
        public abstract Vector Normal();
    }
}
