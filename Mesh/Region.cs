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

        // Testing only
        public void FormTriangles()
        {
            foreach (Front front in this.Fronts)
            {
                Point fixPoint = front.Points[0];
                for (int i = 1; i < front.Points.Count - 1; i++)
                {
                    this.Triangles.Add(
                        new Triangle(fixPoint, front.Points[i], front.Points[i + 1]));
                }
            }
        }

        public void SaveVTK()
        {
            VTKWriter writer = new VTKWriter(Guid.NewGuid().ToString());
            writer.Write(Points, Triangles);
        }

        // Testing only
        private void createTestMesh()
        {
            Points.Add(new Point(1, -2, 0));
            Points.Add(new Point(4, -3, 0));
            Points.Add(new Point(6, -1, 0));
            Points.Add(new Point(4, 0, 0));
            Points.Add(new Point(1, 1, 0));
            Points.Add(new Point(5, 3, 0));
            Points.Add(new Point(2, 4, 0));
            Points.Add(new Point(-1, 4, 0));
            Points.Add(new Point(1, 7, 0));

            Triangles.Add(new Triangle(Points[0], Points[1], Points[3]));
            Triangles.Add(new Triangle(Points[1], Points[2], Points[3]));
            Triangles.Add(new Triangle(Points[0], Points[3], Points[4]));
            Triangles.Add(new Triangle(Points[3], Points[2], Points[5]));
            Triangles.Add(new Triangle(Points[4], Points[3], Points[5]));
            Triangles.Add(new Triangle(Points[4], Points[5], Points[6]));
            Triangles.Add(new Triangle(Points[4], Points[6], Points[7]));
            Triangles.Add(new Triangle(Points[7], Points[6], Points[8]));
        }
    }
}
