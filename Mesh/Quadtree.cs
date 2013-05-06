using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class Quadtree
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        private double verticalHalf;
        private double horizontalHalf;
        private List<Point> pts;

        public Quadtree Root { get; set; }
        public Quadtree[] Children { get; set; }
        public List<Point> Points { get; set; }
        public Quadtree[] Neighbours { get; set; }

        public double Size { get { return this.Top - this.Bottom; } }

        public Quadtree(double left, double top, double right, double bottom, Quadtree root)
        {
            InitQuadtree(left, top, right, bottom, root);
        }

        public Quadtree(List<Point> pts)
        {
            double minx = Double.MaxValue, maxx = Double.MinValue;
            double miny = Double.MaxValue, maxy = Double.MinValue;

            foreach (Point p in pts)
            {
                minx = p.X < minx ? p.X : minx;
                maxx = p.X > maxx ? p.X : maxx;
                miny = p.Y < miny ? p.Y : miny;
                maxy = p.Y > maxy ? p.Y : maxy;
            }

            // Uniform size => square shape.
            double size = (Math.Max(maxx - minx, maxy - miny)) * 1.3 / 2;
            double centerx = (maxx + minx) / 2;
            double centery = (maxy + miny) / 2;

            InitQuadtree(centerx - size, centery + size, centerx + size, centery - size, this);
        }

        private void InitQuadtree(double left, double top, double right, double bottom, Quadtree root)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
            this.Children = null;
            this.Neighbours = new Quadtree[4];
            this.Root = root;

            this.verticalHalf = (this.Left + this.Right) / 2;
            this.horizontalHalf = (this.Top + this.Bottom) / 2;
        }

        /// <summary>
        /// Divides a node into four quadrants.
        /// </summary>
        public void Subdivide()
        {
            this.Children = new Quadtree[4];

            // First quadrant
            this.Children[0] = new Quadtree(verticalHalf, this.Top, this.Right, horizontalHalf, this.Root);
            // Second quadrant
            this.Children[1] = new Quadtree(this.Left, this.Top, verticalHalf, horizontalHalf, this.Root);
            // Third quadrant
            this.Children[2] = new Quadtree(this.Left, horizontalHalf, verticalHalf, this.Bottom, this.Root);
            // Fourth quadrant
            this.Children[3] = new Quadtree(verticalHalf, horizontalHalf, this.Right, this.Bottom, this.Root);

        }

        /// <summary>
        /// Builds quadtree sturcture until leaf size is smaller than limit.
        /// </summary>
        public void BuildRecursive(double limit)
        {
            // End of recursion.
            if (this.Size < limit)
            {
                this.Points = new List<Point>();
                return;
            }

            this.Points = null;
            Subdivide();
            foreach (Quadtree qt in this.Children)
            {
                qt.BuildRecursive(limit);
            }
        }

        /// <summary>
        /// Connects neighbouring cells.
        /// </summary>
        public void ConnectNeighbours()
        {
            if (IsLeaf())
            {
                ////// debug
                if (this.Bottom > -1.91 && this.Bottom < -1.89 &&
                    this.Left < -0.43 && this.Left > -0.44)
                {
                    int dds = 2;
                }
                ////


                // Add neighbour references.
                Point center = new Point(this.verticalHalf, this.horizontalHalf, 0);
                this.Neighbours[0] = Root.Find(new Point(center.X - Size, center.Y, 0));
                this.Neighbours[1] = Root.Find(new Point(center.X, center.Y + Size, 0));
                this.Neighbours[2] = Root.Find(new Point(center.X + Size, center.Y, 0));
                this.Neighbours[3] = Root.Find(new Point(center.X, center.Y - Size, 0));
                return;
            }

            foreach (Quadtree qt in this.Children)
            {
                qt.ConnectNeighbours();
            }
        }

        /// <summary>
        /// Returns all points stored in the quadtree structure.
        /// </summary>
        public List<Point> GetAllPoints()
        {
            if (IsLeaf())
            {
                return this.Points;
            }

            List<Point> pts = new List<Point>();
            foreach (Quadtree qt in this.Children)
            {
                pts.AddRange(qt.GetAllPoints());
            }
            return pts;
        }

        int ctr = 0;
        public bool IsLeaf()
        {
            ctr++;
            if (ctr == 1000)
            {
                int j = 1;
            }
            return this.Children == null;
        }

        public void Add(Point p)
        {
            Find(p).Points.Add(p);
        }

        public void Remove(Point p)
        {
            Find(p).Points.Remove(p);
        }

        /// <summary>
        /// Gets the points within the specified cell distance.
        /// </summary>
        public List<Point> NeighbourAreaPoints(Point p, int distance)
        {
            if (IsLeaf())
            {
                List<Point> pts = new List<Point>();

                for (int x = -distance; x <= distance; x++)
                    for (int y = -distance; y <= distance; y++)
                    {
                        /////// debug
                        if (x < 0 && y==1)
                        {
                            int h = 6;
                        }
                        /////////

                        int xpos = x, ypos = y;
                        Quadtree current = this;

                        while (Math.Abs(xpos) > 0)
                        {
                            current = xpos > 0 ? current.Neighbours[/*1*/2] : current.Neighbours[/*3*/0];
                            xpos = xpos > 0 ? xpos - 1 : xpos + 1;
                            if (current == null) break;
                        }

                        if (current == null) continue;

                        while (Math.Abs(ypos) > 0)
                        {
                            current = ypos > 0 ? current.Neighbours[/*2*/1] : current.Neighbours[/*0*/3];
                            ypos = ypos > 0 ? ypos - 1 : ypos + 1;
                            if (current == null) break;
                        }

                        if (current == null) continue;

                        pts.AddRange(current.Points);
                    }

                return pts;
            }

            Quadtree targetCell = Find(p);

            ///////////////
            if (targetCell == null)
            {
                int dds = 1;
            }
            //////////////////

            return targetCell.NeighbourAreaPoints(p, distance);
        }

        /// <summary>
        /// Returns the leaf quadtree that contains the point.
        /// </summary>
        private Quadtree Find(Point p)
        {
            // Out of covered area
            if (p.X < this.Left || p.X >= this.Right ||
                p.Y < this.Bottom || p.Y >= this.Top)
            {
                return null;
            }

            if (IsLeaf()) return this;

            // I. and II. quadrant
            if (p.Y >= this.horizontalHalf)
            {
                // I. quadrant
                if (p.X >= this.verticalHalf)
                    return this.Children[0].Find(p);
                // II. quadrant
                else
                    return this.Children[1].Find(p);
            }
            // III. and IV. quadrant
            else
            {
                // IV. quadrant
                if (p.X >= this.verticalHalf)
                    return this.Children[3].Find(p);
                // III. quadrant
                else
                    return this.Children[2].Find(p);
            }
        }
    }
}
