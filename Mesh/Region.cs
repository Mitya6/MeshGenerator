using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class Region
    {
        public List<Contour> Contours { get; set; }
        public Mesh2D Mesh { get; set; }

        public Region()
        {
            this.Contours = new List<Contour>();
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

        public void BuildAdvancingFrontMesh()
        {
            this.Mesh = new AdvancingFrontMesh(this);
            this.Mesh.BuildMesh();
        }

        public void SaveVTK()
        {
            this.Mesh.SaveVTK();
        }
    }
}
