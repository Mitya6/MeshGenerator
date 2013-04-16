using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public abstract class Mesh2D
    {
        public List<Point> Points { get; set; }
        public List<Triangle> Triangles { get; set; }
        public List<Front> Fronts { get; set; }
        public Region OwnerRegion { get; set; }

        public Mesh2D(Region region)
        {
            this.Points = new List<Point>();
            this.Triangles = new List<Triangle>();
            this.Fronts = new List<Front>();
            this.OwnerRegion = region;
        }

        /// <summary>
        /// Creates triangle meshes from the Geometry object.
        /// </summary>
        public abstract void BuildMesh();

        public void SaveVTK()
        {
            VTKWriter writer = new VTKWriter(Guid.NewGuid().ToString());
            writer.Write(Points, Triangles);
        }
    }
}
