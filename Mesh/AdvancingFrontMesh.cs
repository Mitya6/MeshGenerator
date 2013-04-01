using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class AdvancingFrontMesh : Mesh2D
    {
        public AdvancingFrontMesh(Geometry geometry)
            : base(geometry) { }

        /// <summary>
        /// Creates triangle meshes from the Geometry object using the
        /// Advancing Front Triangulation method.
        /// </summary>
        public override void BuildMesh()
        {
            // Check whether the contours of the geometry are closed.

            // Check whether the points of the contours are in the same plane.
            checkPlane();

            // Check inner/outer contour numbers.

            // Divide contours into segments.
            foreach (Region region in this.geometry.Regions)
            {
                region.DivideContours();
            }

            // TEST: form triangles
            formTriangles();
        }

        /// <summary>
        /// Checks that all points in each region are in the same plane.
        /// </summary>
        private void checkPlane()
        {
            foreach (Region region in this.geometry.Regions)
            {
                List<Point> regionPoints = new List<Point>();
                foreach (Contour contour in region.Contours)
                {
                    regionPoints.AddRange(contour.GetContourPoints());
                }
                Plane plane = new Plane(regionPoints[0], regionPoints[1], regionPoints[2]);
                foreach (Point point in regionPoints)
                {
                    if (!plane.Contains(point))
                    {
                        throw new ApplicationException(
                            "Not all points are in the same plane in some regions");
                    }
                }
            }
        }

        private void formTriangles()
        {
            foreach (Region region in this.geometry.Regions)
            {
                region.FormTriangles();
            }
        }
    }
}
