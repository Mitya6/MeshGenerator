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
        /// Divides the contours in this reigon into fronts.
        /// </summary>
        public List<Front> DivideContours()
        {
            List<Front> fronts = new List<Front>();
            foreach (Contour contour in this.Contours)
            {
                Front f = new Front(contour.Divide());
                fronts.Add(f);
            }
            return fronts;
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
