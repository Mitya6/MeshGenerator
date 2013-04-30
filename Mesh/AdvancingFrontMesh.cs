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

        public AdvancingFrontMesh(Region region)
            : base(region)
        {
            this.radiusMultiplier = 0.7;
            this.idealDistance = Double.MaxValue;
        }

        /// <summary>
        /// Creates triangle meshes from the Geometry object using the
        /// Advancing Front Triangulation method.
        /// </summary>
        public override void BuildMesh()
        {
            // Check whether the contours of the geometry are closed.

            // Check whether the points of the contours are in the same plane.
            CheckPlane();

            // Check inner/outer contour numbers.

            // Divide contours into segments.
            this.Fronts = this.OwnerRegion.DivideContours();

            // Calculate min ideal point distance from base segment.
            CalculateIdealDistance();

            // Form triangles
            ProcessFronts();
        }

        // temporary solution, needs to be rewrited
        private void CalculateIdealDistance()
        {
            foreach (Front f in this.Fronts)
            {
                List<Segment> segments = f.GetSegmentsUnordered();
                foreach (Segment segment in segments)
                {
                    if (segment.GetLength() < this.idealDistance)
                    {
                        this.idealDistance = segment.GetLength();
                    }
                }
            }
        }

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

        private void ProcessFronts()
        {
            foreach (Front front in this.Fronts)
            {
                // Add front points to output points.
                this.Points.AddRange(front.Points);
            }


            while (GetMeshSegmentCount() > 0)
            {
                Segment shortestSegment = GetShortestUncheckedSegment();

                //debug
                // ha semelyik segmensre sem lehet 3szöget illeszteni, akkor:
                // 1) 3 hosszú kör keresés és 3szög formálás
                // 2) 4 hosszú kör keresés
                //      2a) nagy szögek -> közelebbi pontpár összeköt
                //      2b) kis szögek -> pontösszevonás
                if (shortestSegment == null)
                {
                    int dd = 1;
                    return;
                }
                //debug

                shortestSegment.Checked = true;

                // Find ideal point.
                Point idealPoint = shortestSegment.GetIdealPoint(idealDistance);

                // Search for nearby points.
                double radius = idealDistance * radiusMultiplier;
                List<Point> nearbyPoints = GetNearbyPoints(idealPoint, radius);
                Point.SortByDistance(nearbyPoints, idealPoint);
                nearbyPoints.Add(idealPoint);

                while (nearbyPoints.Count > 0)
                {
                    bool formed =
                        TryFormTriangle(shortestSegment, nearbyPoints[0], nearbyPoints.Count != 1);

                    // Reset checked flag
                    if (formed)
                    {
                        foreach (Front front in this.Fronts)
                            foreach (Segment seg in front.GetSegmentsUnordered())
                                seg.Checked = false;
                        break;
                    }

                    nearbyPoints.RemoveAt(0);
                }
            }

        }

        private bool TryFormTriangle(Segment shortestSegment, Point p, bool existingPoint)
        {
            Segment newSegment1 = new Segment(shortestSegment.Start, p);
            Segment newSegment2 = new Segment(p, shortestSegment.End);

            List<Segment> SegmentsToAdd = new List<Segment>();
            List<Segment> SegmentsToRemove = new List<Segment>();

            // Check if triangle can be formed
            if (this.Contains(newSegment1))
            {
                SegmentsToRemove.Add(newSegment1);
            }
            else
            {
                // Test if triangle candidate intersects with existing elements
                if (IsIntersecting(newSegment1) == false) return false;
                SegmentsToAdd.Add(newSegment1);
            }

            if (this.Contains(newSegment2))
            {
                SegmentsToRemove.Add(newSegment2);
            }
            else
            {
                // Test if triangle candidate intersects with existing elements
                if (IsIntersecting(newSegment2) == false) return false;
                SegmentsToAdd.Add(newSegment2);
            }

            // update front and form triangle

            // Remove current segment from the front.
            Front activeFront = null;
            foreach (Front f in this.Fronts)
            {
                if (f.RemoveSegment(shortestSegment))
                    activeFront = f;
            }

            // Update front with new segments.
            if (!existingPoint)
            {
                this.Points.Add(p);
                activeFront.Points.Add(p);
            }
            foreach (Segment s in SegmentsToAdd)
            {
                activeFront.AddSegment(s);
            }
            foreach (Segment s in SegmentsToRemove)
            {
                activeFront.RemoveSegment(s);
            }

            this.Triangles.Add(new Triangle(shortestSegment.Start, shortestSegment.End, p));
            this.OwnerRegion.Geo.RaiseTriangleAdded();


            // közös pont => frontok összevonása
            bool possibleCommonPoint = true;
            do
            {
                possibleCommonPoint = TryUniteFronts();
            } while (possibleCommonPoint);

            return true;
        }

        private bool TryUniteFronts()
        {
            for (int i = 0; i < this.Fronts.Count; i++)
            {
                for (int j = i + 1; j < this.Fronts.Count; j++)
                {
                    bool needUnion = this.Fronts[i].HasCommonPointWith(this.Fronts[j]);
                    this.Fronts[i].Join(this.Fronts[j]);
                    this.Fronts.Remove(this.Fronts[j]);
                    return true;
                }
            }
            return false;
        }

        private int GetMeshSegmentCount()
        {
            int count = 0;
            foreach (Front front in this.Fronts)
            {
                count += front.Segments.Count;
            }
            return count;
        }

        private Segment GetShortestUncheckedSegment()
        {
            double shortest = Double.MaxValue;
            Segment shortestSegment = null;
            foreach (Front front in this.Fronts)
            {
                Segment current = front.GetShortestUncheckedSegment();

                if (current == null)
                {
                    return null;
                }

                if (current.GetLength() < shortest)
                {
                    shortest = current.GetLength();
                    shortestSegment = current;
                }
            }
            return shortestSegment;
        }

        private bool Contains(Segment segment)
        {
            foreach (Front front in this.Fronts)
            {
                if (front.Contains(segment))
                    return true;
            }
            return false;
        }

        private bool IsIntersecting(Segment s)
        {
            // Check intersection for triangles
            foreach (Triangle triangle in this.Triangles)
            {
                Point p1 = s.Intersection(new Segment(triangle.Points[0], triangle.Points[1]));
                Point p2 = s.Intersection(new Segment(triangle.Points[1], triangle.Points[2]));
                Point p3 = s.Intersection(new Segment(triangle.Points[2], triangle.Points[0]));
                if (p1 != null || p2 != null || p3 != null)
                    return false;
            }

            // Check intersection for segments of all fronts
            List<Segment> segments = new List<Segment>();
            foreach (Front front in this.Fronts)
            {
                segments.AddRange(front.GetSegmentsUnordered());
            }
            foreach (Segment other in segments)
            {
                Point p = s.Intersection(other);
                if (p != null)
                    return false;
            }

            return true;
        }

        // to be improved (store points in tree structure)
        private List<Point> GetNearbyPoints(Point idealPoint, double r)
        {
            List<Point> pts = new List<Point>();
            foreach (Front f in this.Fronts)
            {
                foreach (Point p in f.Points)
                {
                    if (Point.Distance(idealPoint, p) <= r)
                    {
                        pts.Add(p);
                    }
                }
            }
            return pts;
        }
    }
}
