using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mesh.Curves;

namespace Mesh
{
    public abstract class Curve
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

        public Curve(Point s, Point e)
        {
            this.start = s;
            this.end = e;
            length = -1;
        }

        public abstract double GetLength();
        public abstract Point GetPoint(double t);
        public abstract Vector Normal();
        public abstract List<Point> Divide(double elementSize);
        public abstract List<Point> Divide(int elementCount);
    }
}
