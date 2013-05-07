using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class AdvancingFrontMesh : Mesh2D
    {
        private double radiusMultiplier;
        private double idealDistance;
        private double sharpAngleThreshold;
        private int cellDistance;

        public AdvancingFrontMesh(Region region)
            : base(region)
        {
            this.radiusMultiplier = 0.7;
            this.idealDistance = Double.MaxValue;
            this.sharpAngleThreshold = 25;
            this.cellDistance = 2;
        }

        /// <summary>
        /// Creates triangle meshes from the Geometry object using the
        /// Advancing Front Triangulation method.
        /// </summary>
        public override void BuildMesh()
        {
            // Check whether the points of the contours are in the same plane.
            CheckPlane();

            // Divide contours into segments and create initial front.
            double distance;
            this.Front = this.OwnerRegion.DivideContours(out distance);
            this.idealDistance = distance;
            this.Front.Points.BuildRecursive(this.idealDistance);
            this.Front.Points.ConnectNeighbours();
            this.Front.InitFront();

            // Initialize Quadtree to store mesh points.
            this.Points = new Quadtree(this.Front.GetAllPoints());
            this.Points.BuildRecursive(this.idealDistance);
            this.Points.ConnectNeighbours();

            // Form triangles
            ProcessFront();
        }

        // temporary solution, needs to be rewrited
        //private void CalculateIdealDistance()
        //{
        //    List<Segment> segments = this.Front.GetSegmentsUnordered();
        //    foreach (Segment segment in segments)
        //    {
        //        if (segment.GetLength() < this.idealDistance)
        //        {
        //            this.idealDistance = segment.GetLength();
        //        }
        //    }
        //}

        // Refinement needed (3 points on the same straight).
        /// <summary>
        /// Checks that all input points in the owner region are in the same plane.
        /// </summary>
        private void CheckPlane()
        {
            List<Point> regionPoints = new List<Point>();
            foreach (Contour contour in this.OwnerRegion.Contours)
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

        private void ProcessFront()
        {
            // Add front points to output points.
            foreach (Point p in this.Front.GetAllPoints())
            {
                this.Points.Add(p);
            }

            while (this.Front.Segments.Count > 0)
            {
                Segment shortestSegment = this.Front.GetShortestUncheckedSegment();

                if (shortestSegment == null)
                {
                    DecreaseFront();
                    ResetCheckedFlag();
                    continue;
                }

                shortestSegment.Checked = true;

                // Find ideal point.
                Point idealPoint = shortestSegment.GetIdealPoint(idealDistance);

                // Search for nearby points.
                double radius = idealDistance * radiusMultiplier;
                List<Point> nearbyPoints = GetNearbyFrontPoints(idealPoint, radius);
                Point.SortByDistance(nearbyPoints, idealPoint);
                nearbyPoints.Add(idealPoint);

                while (nearbyPoints.Count > 0)
                {
                    bool formed = TryFormTriangle(
                        shortestSegment, nearbyPoints[0], nearbyPoints.Count != 1);

                    // Reset checked flag
                    if (formed)
                    {
                        ResetCheckedFlag();
                        break;
                    }
                    nearbyPoints.RemoveAt(0);
                }
            }
        }

        private void DecreaseFront()
        {
            List<Segment> segments = this.Front.GetSegmentsUnordered();

            if (segments.Count < 3)
            {
                throw new ApplicationException("Front processing error");
            }
            
            // Find 3 long cycle and form triangle if possible
            for (int i = 0; i < segments.Count; i++)
                for (int j = 0; j < segments.Count; j++)
                    for (int k = 0; k < segments.Count; k++)
                    {
                        if (i == j || j == k || k == i) continue;

                        if (segments[i].End.Equals(segments[j].Start) &&
                            segments[j].End.Equals(segments[k].Start) &&
                            segments[k].End.Equals(segments[i].Start))
                        {
                            AddTriangle(segments[i].Start, segments[j].Start,
                                segments[k].Start);
                            this.Front.RemoveSegment(segments[i]);
                            this.Front.RemoveSegment(segments[j]);
                            this.Front.RemoveSegment(segments[k]);
                            return;
                        }
                    }

            // Find 4 or longer cycle and reduce it
            for (int i = 0; i < segments.Count; i++)
                for (int j = i + 1; j < segments.Count; j++)
                {
                    if (segments[i].Connected(segments[j]))
                    {
                        if (Segment.Angle(segments[i], segments[j]) < sharpAngleThreshold)
                        {
                            TryContractVertices(segments[i], segments[j]);
                            this.Front.RemoveSegment(segments[i]);
                            this.Front.RemoveSegment(segments[j]);
                            // Remove possible 2 long circles
                            Clean2Loops();
                            return;
                        }
                    }
                }

            throw new ApplicationException("Early finish");

        }

        private void Clean2Loops()
        {
            List<Segment> segments = this.Front.GetSegmentsUnordered();
            for (int i = 0; i < segments.Count; i++)
            {
                for (int j = i + 1; j < segments.Count; j++)
                {
                    if (segments[i].Equals(segments[j]))
                    {
                        this.Front.RemoveSegment(segments[i]);
                        this.Front.RemoveSegment(segments[j]);
                    }
                }
            }
        }

        private void TryContractVertices(Segment s1, Segment s2)
        {
            Point p1, p2, common;
            if (s1.End.Equals(s2.Start))
            {
                common = s1.End;
                p1 = s1.Start;
                p2 = s2.End;
            }
            else if (s1.Start.Equals(s2.End))
            {
                common = s1.Start;
                p2 = s1.End;
                p1 = s2.Start;
            }
            else
            {
                throw new ApplicationException("Invalid segment order");
            }

            if (p1.IsEdgePoint && p2.IsEdgePoint)
            {
                AddTriangle(p1, p2, common);
                this.Front.AddSegment(new Segment(p1, p2));
            }
            else
            {
                Point midPoint;
                if (p1.IsEdgePoint)
                    midPoint = p1;
                else if (p2.IsEdgePoint)
                    midPoint = p2;
                else
                {
                    midPoint = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2, 0);
                }
                ReplacePoints(p1, p2, midPoint);
            }
        }

        private void ReplacePoints(Point p1, Point p2, Point midPoint)
        {
            // Replace points in front
            this.Front.ReplacePoints(p1, p2, midPoint);

            // Replace points in mesh

            // Add midpoint to output points
            this.Points.Add(midPoint);

            // Replace points in triangles
            foreach (Triangle triangle in this.Triangles)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (p1.Equals(triangle.Points[i]))
                        triangle.Points[i] = midPoint;
                    if (p2.Equals(triangle.Points[i]))
                        triangle.Points[i] = midPoint;
                }
            }

            // Remove p1 and p2
            this.Points.Remove(p1);
            this.Points.Remove(p2);
        }

        private void ResetCheckedFlag()
        {
            foreach (Segment seg in this.Front.GetSegmentsUnordered())
                seg.Checked = false;
        }

        private bool TryFormTriangle(Segment shortestSegment, Point p, bool existingPoint)
        {
            Segment newSegment1 = new Segment(shortestSegment.Start, p);
            Segment newSegment2 = new Segment(p, shortestSegment.End);

            List<Segment> SegmentsToAdd = new List<Segment>();
            List<Segment> SegmentsToRemove = new List<Segment>();

            // Check if triangle can be formed
            if (this.Front.Contains(newSegment1, cellDistance))
            {
                SegmentsToRemove.Add(newSegment1);
            }
            else
            {
                // Test if triangle candidate intersects with existing elements
                if (IsIntersecting(newSegment1)) return false;

                // Test if segment is outside of the front
                if (!this.Front.IsPointInside(newSegment1.GetPoint(0.5))) return false;

                SegmentsToAdd.Add(newSegment1);
            }

            if (this.Front.Contains(newSegment2, cellDistance))
            {
                SegmentsToRemove.Add(newSegment2);
            }
            else
            {
                // Test if triangle candidate intersects with existing elements
                if (IsIntersecting(newSegment2)) return false;

                // Test if segment is outside of the front
                if (!this.Front.IsPointInside(newSegment2.GetPoint(0.5))) return false;

                SegmentsToAdd.Add(newSegment2);
            }

            // update front and form triangle

            // Remove current segment from the front.
            this.Front.RemoveSegment(shortestSegment);

            // Update front with new segments.
            if (!existingPoint)
            {
                this.Points.Add(p);
                this.Front.Points.Add(p);
            }
            foreach (Segment s in SegmentsToAdd)
            {
                this.Front.AddSegment(s);
            }
            foreach (Segment s in SegmentsToRemove)
            {
                this.Front.RemoveSegment(s);
            }

            AddTriangle(shortestSegment.Start, shortestSegment.End, p);

            return true;
        }

        /// <summary>
        /// Adds a triangle to the mesh specified by the three points and
        /// fires the triangle added event.
        /// </summary>
        private void AddTriangle(Point p1, Point p2, Point p3)
        {
            Triangle t = new Triangle(p1, p2, p3);
            this.Triangles.Add(t);

            foreach (Point p in t.Points)
            {
                p.Triangles.Add(t);
            }
            this.OwnerRegion.Geo.RaiseTriangleAdded();
        }

        /// <summary>
        /// Checks if the segment is intersecting with nearby front and mesh elements.
        /// </summary>
        private bool IsIntersecting(Segment s)
        {
            bool intersecting = false;

            // Get nearby triangles
            HashSet<Triangle> triangles = s.GetNearbyTriangles(this, cellDistance);

            // Check intersection with nearby triangles
            foreach (Triangle triangle in triangles)
            {
                Point p1 = s.Intersection(new Segment(triangle.Points[0], triangle.Points[1]));
                Point p2 = s.Intersection(new Segment(triangle.Points[1], triangle.Points[2]));
                Point p3 = s.Intersection(new Segment(triangle.Points[2], triangle.Points[0]));
                if (p1 != null || p2 != null || p3 != null)
                    return true;
            }
            //Parallel.ForEach(triangles, (triangle, state) =>
            //{
            //    Point p1 = s.Intersection(new Segment(triangle.Points[0], triangle.Points[1]));
            //    Point p2 = s.Intersection(new Segment(triangle.Points[1], triangle.Points[2]));
            //    Point p3 = s.Intersection(new Segment(triangle.Points[2], triangle.Points[0]));
            //    if (p1 != null || p2 != null || p3 != null)
            //    {
            //        intersecting = true;
            //        state.Stop();
            //    }
            //});

            // Get nearby segments of the front
            HashSet<Segment> segments = s.GetNearbySegments(this.Front, cellDistance);
            
            // Check intersecion with nearby segments
            foreach (Segment other in segments)
            {
                Point p = s.Intersection(other);
                if (p != null)
                    return true;
            }
            //Parallel.ForEach(segments, (other, state) =>
            //{
            //    Point p = s.Intersection(other);
            //    if (p != null)
            //    {
            //        intersecting = true;
            //        state.Stop();
            //    }
            //});

            return false;
            //return intersecting;
        }

        /// <summary>
        /// Returns those points of the front that fall within the given radius
        /// of a central point.
        /// </summary>
        private List<Point> GetNearbyFrontPoints(Point centre, double r)
        {
            List<Point> pts = this.Front.NeighbourAreaPoints(centre, cellDistance);
            List<Point> nearbyPoints = new List<Point>();
            foreach (Point p in pts)
            {
                if (Point.Distance(centre, p) <= r)
                {
                    nearbyPoints.Add(p);
                }
            }
            return nearbyPoints;
        }
    }
}
