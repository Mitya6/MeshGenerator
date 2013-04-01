using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class Region
    {
        public List<Point> Points { get; set; }
        public List<Triangle> Triangles { get; set; }
        public List<Contour> Contours { get; set; }
        public List<Front> Fronts { get; set; }

        public Region()
        {
            this.Points = new List<Point>();
            this.Triangles = new List<Triangle>();
            this.Contours = new List<Contour>();
            this.Fronts = new List<Front>();
        }

        public void DivideContours()
        {
            foreach (Contour contour in this.Contours)
            {
                Front f = new Front(contour.Divide());
                this.Fronts.Add(f);
                this.Points.AddRange(f.Points);
            }
        }

        // TO BE continued
        public void FormTriangles()
        {
            foreach (Front front in this.Fronts)
            {
                foreach (Segment segment in front.Segments)
                {
                    Point rotatedPoint = segment.RotateInward60();
                    this.Points.Add(rotatedPoint);
                    this.Triangles.Add(new Triangle(segment.Start, segment.End, rotatedPoint));
                }
            }
        }

        public void SaveVTK()
        {
            VTKWriter writer = new VTKWriter(Guid.NewGuid().ToString());
            writer.Write(Points, Triangles);
        }
    }
}
