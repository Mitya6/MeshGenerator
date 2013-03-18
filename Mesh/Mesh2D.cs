using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class Mesh2D
    {
        private Geometry geometry { get; set; }

        public Mesh2D(Geometry geometry)
        {
            this.geometry = geometry;
        }

        /// <summary>
        /// Creates the mesh from the Geometry object.
        /// </summary>
        public void BuildMesh()
        {
            // Check whether the contours of the geometry are closed.

            // Check whether the points of the contours are in the same plane.

            // Divide contours into segments.
            DivideContours();

            // TEST: form triangles
            FormTriangles();
        }

        private void FormTriangles()
        {
            foreach (Region region in this.geometry.Regions)
            {
                region.FormTriangles();
            }
        }

        public void SaveVTK()
        {
            this.geometry.SaveVTK();
        }

        public void DivideContours()
        {
            foreach (Region region in this.geometry.Regions)
            {
                region.DivideContours();
            }
        }
    }
}
