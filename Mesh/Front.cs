using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class Front
    {
        public List<Segment> Segments { get; set; }
        public List<Point> Points { get; set; }

        public Front(List<Point> pts)
        {
            this.Points = pts;
            this.Segments = new List<Segment>();
            for (int i = 0; i < pts.Count - 1; i++)
            {
                this.Segments.Add(new Segment(pts[i], pts[i + 1]));
            }
            this.Segments.Add(new Segment(pts[pts.Count - 1], pts[0]));
        }
    }
}
