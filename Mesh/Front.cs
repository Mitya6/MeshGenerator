using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class Front
    {
        //public List<Segment> Segments { get; set; }
        public List<Point> Points { get; set; }

        public Front(List<Point> pts)
        {
            this.Points = pts;
        }
    }
}
