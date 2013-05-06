using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class Front
    {
        // Use of sorted list to reach shortest segment quickly.
        // Allows the duplication of keys.
        public SortedList<double, List<Segment>> Segments { get; set; }
        //public List<Point> Points { get; set; }
        public List<Point> InitialPoints { get; set; }
        public Quadtree Points { get; set; }
        public int Count
        {
            get
            {
                int i = 0;
                foreach (var item in Segments)
                    foreach (Segment s in item.Value) i++;
                return i;
            }
        }

        public Front(List<Point> pts)
        {
            this.InitialPoints = pts;
            this.Points = new Quadtree(this.InitialPoints);
            
            this.Segments = new SortedList<double, List<Segment>>();
            if (this.InitialPoints.Count == 0) return;

            // Create segments between points.
            for (int i = 0; i < this.InitialPoints.Count - 1; i++)
            {
                Segment seg = new Segment(this.InitialPoints[i], this.InitialPoints[i + 1]);
                AddSegment(seg);
            }
            Segment s = new Segment(this.InitialPoints[this.InitialPoints.Count - 1], 
                this.InitialPoints[0]);
            AddSegment(s);
        }

        public void InitFront()
        {
            foreach (Point p in this.InitialPoints)
            {
                this.Points.Add(p);
            }
        }

        /// <summary>
        /// Get all segments of this front unordered.
        /// </summary>
        public List<Segment> GetSegmentsUnordered()
        {
            List<Segment> segments = new List<Segment>();
            foreach (var item in this.Segments)
            {
                foreach (Segment s in item.Value)
                {
                    segments.Add(s);
                }
            }
            return segments;
        }

        /// <summary>
        /// Adds a segment to the front.
        /// </summary>
        public void AddSegment(Segment segment)
        {
            double len = segment.GetLength();
            if (!this.Segments.ContainsKey(len))
            {
                this.Segments.Add(len, new List<Segment>());
            }
            this.Segments[len].Add(segment);
            segment.ConnectEndpoints();
        }

        /// <summary>
        /// Returns the shortest segment from the front.
        /// </summary>
        public Segment GetShortestSegment()
        {
            if (this.Segments.Count == 0) return null;

            return this.Segments.ElementAt(0).Value[0];
        }

        /// <summary>
        /// Removes the shortest segment from the front.
        /// </summary>
        public void RemoveShortestSegment()
        {
            if (this.Segments.Count == 0) return;

            Segment s = this.Segments.ElementAt(0).Value[0];
            this.Segments.ElementAt(0).Value.RemoveAt(0);
            s.DisconnectEndpoints();
            RemoveUnconnectedEndpoints(s);

            if (this.Segments.ElementAt(0).Value.Count == 0)
            {
                this.Segments.RemoveAt(0);
            }
        }

        /// <summary>
        /// Removes the points from the front that are not part of any
        /// segment of the front.
        /// </summary>
        private void RemoveUnconnectedEndpoints(Segment s)
        {
            bool endRemains = false;
            bool startRemains = false;

            foreach (Segment segment in this.GetSegmentsUnordered())
            {
                if (s.End.Equals(segment.End) || s.End.Equals(segment.Start))
                {
                    endRemains = true;
                }
            }

            foreach (Segment segment in this.GetSegmentsUnordered())
            {
                if (s.Start.Equals(segment.End) || s.Start.Equals(segment.Start))
                {
                    startRemains = true;
                }
            }
            if (!endRemains) this.Points.Remove(s.End);
            if (!startRemains) this.Points.Remove(s.Start);
        }

        public bool RemoveSegment(Segment s)
        {
            bool removed = false;
            double? toRemove = null;
            foreach (var item in this.Segments)
            {
                Segment sToRemove = null;
                foreach (Segment segment in item.Value)
                {
                    if (s.Equals(segment))
                    {
                        sToRemove = segment;
                        //if (item.Value.Count == 0)
                        //{
                        //    Segments.Remove(item.Key);
                        //}
                    }
                }
                if (sToRemove != null)
                {
                    item.Value.RemoveAll(seg => seg.Equals(sToRemove));
                    removed = true;
                }
                if (item.Value.Count == 0)
                {
                    toRemove = item.Key;
                }
            }
            if (toRemove != null)
            {
                this.Segments.Remove((double)toRemove);
            }
            RemoveUnconnectedEndpoints(s);
            s.DisconnectEndpoints();
            return removed;
        }

        /// <summary>
        /// Returns whether a point is inside the front.
        /// </summary>
        public bool IsPointInside(Point p)
        {
            double y = p.Y;
            int leftIntersection = 0;
            int rightIntersection = 0;

            // Segments crossing the y=p.Y straightline.
            foreach (var sameLengthSegments in this.Segments)
            {
                foreach (Segment s in sameLengthSegments.Value)
                {
                    if ((s.Start.Y < y && s.End.Y >= y) || (s.Start.Y >= y && s.End.Y < y))
                    {
                        Point intersection = s.YIntersection(y);
                        if (intersection.X < p.X) leftIntersection++;
                        else if (intersection.X > p.X) rightIntersection++;
                        // Point is on the segment?
                        else return false;
                    }
                }
            }
            if ((leftIntersection % 2) == 1 && (rightIntersection % 2) == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the front contains a segment.
        /// </summary>
        public bool Contains(Segment s)
        {
            List<Segment> segments = GetSegmentsUnordered();
            foreach (Segment segment in segments)
            {
                if (segment.Equals(s))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the shortest segment of the front that has not been
        /// checked if a triangle can be formed upon it in the current state
        /// of the front.
        /// </summary>
        public Segment GetShortestUncheckedSegment()
        {
            if (this.Segments.Count == 0) return null;

            foreach (var item in this.Segments)
            {
                foreach (Segment s in item.Value)
                {
                    if (s.Checked == false)
                    {
                        return s;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Determines whether the front has at least one commmon 
        /// point with another front.
        /// </summary>
        //public bool HasCommonPointWith(Front other)
        //{
        //    foreach (Point point in this.Points)
        //    {
        //        foreach (Point otherPoint in other.Points)
        //        {
        //            if (point.Equals(otherPoint))
        //                return true;
        //        }
        //    }
        //    return false;
        //}


        public void Join(Front front)
        {
            foreach (Point p in front.GetInitialPoints())
            {
                this.InitialPoints.Add(p);
            }

            List<Segment> segments = front.GetSegmentsUnordered();
            foreach (Segment segment in segments)
            {
                this.AddSegment(segment);
            }
        }

        private List<Point> GetInitialPoints()
        {
            return this.InitialPoints;
        }

        /// <summary>
        /// Replaces two front points with one common point.
        /// </summary>
        public void ReplacePoints(Point p1, Point p2, Point midPoint)
        {
            // Add midPoint
            this.Points.Add(midPoint);

            // Update segment endpoints
            List<Segment> segments = GetSegmentsUnordered();
            foreach (Segment segment in segments)
            {
                if (segment.Start.Equals(p1))
                    segment.Start = midPoint;
                if (segment.End.Equals(p1))
                    segment.End = midPoint;
                if (segment.Start.Equals(p2))
                    segment.Start = midPoint;
                if (segment.End.Equals(p2))
                    segment.End = midPoint;
            }

            // Remove p1 and p2
            this.Points.Remove(p1);
            this.Points.Remove(p2);
        }

        public List<Point> GetAllPoints()
        {
            return this.Points.GetAllPoints();
        }

        public List<Point> NeighbourAreaPoints(Point p, int cellDistance)
        {
            return this.Points.NeighbourAreaPoints(p, cellDistance);
        }
    }
}
