using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Mesh.Curves;

namespace Mesh
{
    public class Geometry
    {
        public List<Region> Regions { get; set; }
        private String Filename { get; set; }

        public Geometry(String filename)
        {
            this.Regions = new List<Region>();
            this.Filename = filename;
        }

        public void Load()
        {
            // Load the whole XML input file.
            XmlDocument doc = new XmlDocument();
            doc.Load(Filename);

            // Parse XML document content.
            parseXMLDocument(doc);
        }

        private void parseXMLDocument(XmlDocument doc)
        {
            // Process data for each region
            XmlNodeList regions = doc.SelectNodes("//Region");
            foreach (var region in regions)
            {
                Region reg = new Region();

                // Select contours in order
                XmlNodeList contours = ((XmlNode)region).SelectNodes("Contour");
                foreach (var contour in contours)
                {
                    Contour cont = null;

                    // Get division method and value for current contour
                    DivisionMethod method = getDivisionMethod((XmlElement)contour);
                    if (method == DivisionMethod.elementSize)
                    {
                        double elementSize = Double.Parse(((XmlElement)contour).
                            Attributes["elementSize"].Value, CultureInfo.InvariantCulture);
                        cont = new Contour(elementSize);
                    }
                    else if (method == DivisionMethod.elementCount)
                    {
                        int elementCount = Int32.Parse(
                            ((XmlElement)contour).Attributes["elementCount"].Value);
                        cont = new Contour(elementCount);
                    }

                    // Read and create points in current contour
                    XmlNodeList points = ((XmlNode)contour).SelectNodes("Points/Point");
                    List<Point> pts = new List<Point>();
                    foreach (var point in points)
                    {
                        // Parse point coordinates
                        String s = ((XmlNode)point).Attributes["x"].Value;
                        Double x = Double.Parse(s, CultureInfo.InvariantCulture);
                        s = ((XmlNode)point).Attributes["y"].Value;
                        Double y = Double.Parse(s, CultureInfo.InvariantCulture);
                        s = ((XmlNode)point).Attributes["z"].Value;
                        Double z = Double.Parse(s, CultureInfo.InvariantCulture);

                        Point p = new Point(x, y, z);
                        pts.Add(p);
                    }

                    // Read and create lines
                    XmlNodeList lines = ((XmlNode)contour).SelectNodes("Curves/Line");
                    foreach (var line in lines)
                    {
                        // Parse line endpoints
                        String s = ((XmlNode)line).Attributes["p1"].Value;
                        int idx1 = int.Parse(s, CultureInfo.InvariantCulture);
                        s = ((XmlNode)line).Attributes["p2"].Value;
                        int idx2 = int.Parse(s, CultureInfo.InvariantCulture);

                        Line l = new Line(pts[idx1], pts[idx2]);
                        if (cont != null)
                        {
                            cont.Curves.Add(l); 
                        }
                    }
                    // TODO: process other types of curves


                    if (cont != null)
                    {
                        reg.Contours.Add(cont); 
                    }
                }

                this.Regions.Add(reg);
            }
        }

        private DivisionMethod getDivisionMethod(XmlElement contour)
        {
            DivisionMethod method = DivisionMethod.indeterminate;
            if (contour.HasAttribute("elementSize"))
            {
                method = DivisionMethod.elementSize;
            }
            else if (contour.HasAttribute("elementCount"))
            {
                method = DivisionMethod.elementCount;
            }
            return method;
        }

        public void SaveVTK()
        {
            foreach (Region region in this.Regions)
            {
                region.SaveVTK();
            }
        }
    }
}
