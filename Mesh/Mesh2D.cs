using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public abstract class Mesh2D
    {
        protected Geometry geometry { get; set; }

        public Mesh2D(Geometry geometry)
        {
            this.geometry = geometry;
        }

        /// <summary>
        /// Creates triangle meshes from the Geometry object.
        /// </summary>
        public abstract void BuildMesh();

        public void SaveVTK()
        {
            this.geometry.SaveVTK();
        }
    }
}
