using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public abstract class Mesh2D
    {
        public Quadtree Points { get; set; }
        //public List<Point> Points { get; set; }
        public List<Triangle> Triangles { get; set; }
        public Front Front { get; set; }
        public Region OwnerRegion { get; set; }

        public Mesh2D(Region region)
        {
            this.Points = null;
            this.Triangles = new List<Triangle>();
            //this.Fronts = new List<Front>();
            this.OwnerRegion = region;
        }

        /// <summary>
        /// Creates triangle meshes from the Geometry object.
        /// </summary>
        public abstract void BuildMesh();

        public void SaveVTK()
        {
            VTKWriter writer = new VTKWriter(Guid.NewGuid().ToString());
            writer.Write(GetAllPoints(), Triangles);
        }

        public List<Point> GetAllPoints()
        {
            return this.Points.GetAllPoints();
        }
    }
}
