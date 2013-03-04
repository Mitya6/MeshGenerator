using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class Mesh2D
    {
        // Testing, to be deleted later
        public List<Point> Points { get; set; }
        public List<Triangle> Triangles { get; set; }

        public Mesh2D()
        {
            Points = new List<Point>();
            Triangles = new List<Triangle>();
            createTestMesh();

            // Add a single test triangle to the mesh
            //testTriangles = new Triangle(new Point(-0.34, 10.546, 0), 
            //    new Point(1.13, 0, 0), new Point(-5.1, -1.15, 0));
        }

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

        public void SaveVTK()
        {
            VTKWriter writer = new VTKWriter(Guid.NewGuid().ToString());
            writer.Write(Points, Triangles);
        }
    }
}
