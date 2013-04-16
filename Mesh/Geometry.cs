using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Mesh.Curves;
using Mesh.Enum;

namespace Mesh
{
    /// <summary>
    /// Main object and top level container for all geometric data.
    /// </summary>
    public class Geometry
    {
        public const double Epsilon = 0.00001;
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
            ParseXMLDocument(doc);
        }

        public void BuildMeshes()
        {
            foreach (Region region in this.Regions)
            {
                region.BuildAdvancingFrontMesh();
            }
        }

        /// <summary>
        /// Builds geometry from an XmlDocument object.
        /// </summary>
        private void ParseXMLDocument(XmlDocument doc)
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

                    // Get contour type
                    ContourTypes type = GetContourType((XmlElement)contour);

                    // Get division method and value for current contour
                    DivisionMethod method = GetDivisionMethod((XmlElement)contour);
                    if (method == DivisionMethod.ElementSize)
                    {
                        double elementSize = Double.Parse(((XmlElement)contour).
                            Attributes["elementSize"].Value, CultureInfo.InvariantCulture);
                        cont = new Contour(elementSize, type);
                    }
                    else
                    {
                        cont = new Contour(type);
                    }

                    // Read and create points in current contour
                    XmlNodeList points = ((XmlNode)contour).SelectNodes("Point");
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
                    XmlNodeList lines = ((XmlNode)contour).SelectNodes("Line");
                    foreach (var line in lines)
                    {
                        StraightLine l = null;

                        // Division method for current curve, overrides contour level
                        // default division method
                        DivisionMethod divisionMethod = GetDivisionMethod((XmlElement)line);

                        // Parse line endpoints
                        String s = ((XmlNode)line).Attributes["p1"].Value;
                        int idx1 = int.Parse(s, CultureInfo.InvariantCulture);
                        s = ((XmlNode)line).Attributes["p2"].Value;
                        int idx2 = int.Parse(s, CultureInfo.InvariantCulture);

                        if (divisionMethod == DivisionMethod.ElementCount)
                        {
                            s = ((XmlNode)line).Attributes["elementCount"].Value;
                            l = new StraightLine(pts[idx1], pts[idx2], int.Parse(s, CultureInfo.InvariantCulture));
                        }
                        else if (divisionMethod == DivisionMethod.ElementSize)
                        {
                            s = ((XmlNode)line).Attributes["elementSize"].Value;
                            l = new StraightLine(pts[idx1], pts[idx2], Double.Parse(s, CultureInfo.InvariantCulture));
                        }
                        // Get default element size for the whole contour
                        else
                        {
                            if (cont.DivisionMethod != DivisionMethod.ElementSize)
                            {
                                throw new ApplicationException("Invalid input XML");
                            }
                            l = new StraightLine(pts[idx1], pts[idx2], cont.ElementSize);
                        }

                        if (cont != null && l != null)
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

        /// <summary>
        /// Returns the type of the contour (inner or outer).
        /// </summary>
        private ContourTypes GetContourType(XmlElement contour)
        {
            ContourTypes type = ContourTypes.Invalid;
            String s = contour.Attributes["type"].Value;
            if (s == "Outer" || s == "outer")
            {
                type = ContourTypes.Outer;
            }
            if (s == "Inner" || s == "inner")
            {
                type = ContourTypes.Inner;
            }
            return type;
        }

        /// <summary>
        /// Returns the division method for a contour or curve xml element.
        /// </summary>
        private DivisionMethod GetDivisionMethod(XmlElement xmlElement)
        {
            DivisionMethod method = DivisionMethod.Indeterminate;
            if (xmlElement.HasAttribute("elementSize"))
            {
                method = DivisionMethod.ElementSize;
            }
            else if (xmlElement.HasAttribute("elementCount"))
            {
                method = DivisionMethod.ElementCount;
            }
            return method;
        }

        /// <summary>
        /// Saves data for each region in VTK files.
        /// </summary>
        public void SaveVTK()
        {
            foreach (Region region in this.Regions)
            {
                region.SaveVTK();
            }
        }
    }
}
