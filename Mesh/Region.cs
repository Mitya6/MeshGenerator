using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mesh.Enum;

namespace Mesh
{
    public class Region
    {
        public List<Contour> Contours { get; set; }
        public Mesh2D Mesh { get; set; }
        public Geometry Geo { get; set; }

        public Region(Geometry geo)
        {
            this.Contours = new List<Contour>();
            this.Geo = geo;
        }

        /// <summary>
        /// Divides the contours in this reigon and creates a front.
        /// </summary>
        public Front DivideContours(out double idealDistance)
        {
            //double d;
            Front front = new Front(this.Contours[0].Divide(out idealDistance));
            //idealDistance = Double.MaxValue;

            for (int i = 1; i < this.Contours.Count; i++)
            {
                double d;
                Front f = new Front(Contours[i].Divide(out d));
                if (d < idealDistance) idealDistance = d;
                front.Join(f);
            }

            // Mark edge points.
            foreach (Point point in front.InitialPoints)
            {
                point.IsEdgePoint = true;
            }

            return front;
        }

        public void BuildMesh(MeshTypes meshType)
        {
            if (meshType == MeshTypes.AdvancingFront)
            {
                this.Mesh = new AdvancingFrontMesh(this);
            }
            else if (meshType == MeshTypes.Delaunay)
            {
                // TODO: delaunay mesh
            }
            this.Mesh.BuildMesh();
        }

        public void SaveVTK()
        {
            this.Mesh.SaveVTK();
        }
    }
}
