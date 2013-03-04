using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Mesh
{
    class VTKWriter
    {
        private XmlDocument xmlDoc;
        private string fileName;

        public VTKWriter(string fileName)
        {
            this.fileName = fileName;

            // Create an empty XmlDocument.
            this.xmlDoc = new XmlDocument();
        }

        public void Write(List<Point> points, List<Triangle> triangles)
        {
            // Create directory if needed
            DirectoryInfo di = new DirectoryInfo(@".\");
            if (di.EnumerateDirectories("VTKOutput").Count() == 0)
                di.CreateSubdirectory("VTKOutput");

            // Fill and save the VTK xml document
            fillXmlDocument(points, triangles);
            this.xmlDoc.Save(@"VTKOutput\" + this.fileName + ".vtu");
        }

        private void fillXmlDocument(List<Point> points, List<Triangle> triangles)
        {
            // Add VTK root node and root attributes
            XmlNode rootNode = this.xmlDoc.CreateElement("VTKFile");
            this.xmlDoc.AppendChild(rootNode);

            addAttribute(rootNode, "type", "UnstructuredGrid");
            addAttribute(rootNode, "version", "0.1");
            addAttribute(rootNode, "byte_order", "LittleEndian");

            // Add elements containing the points and the cells
            XmlNode gridNode = addChildNode(rootNode, "UnstructuredGrid");
            XmlNode pieceNode = addChildNode(gridNode, "Piece");

            addAttribute(pieceNode, "NumberOfPoints", points.Count.ToString());
            addAttribute(pieceNode, "NumberOfCells", triangles.Count.ToString());

            XmlNode pointsNode = addChildNode(pieceNode, "Points");
            XmlNode cellsNode = addChildNode(pieceNode, "Cells");

            // Add points
            XmlNode pointsDataArray = addChildNode(pointsNode, "DataArray");
            addAttribute(pointsDataArray, "type", "Float32");
            addAttribute(pointsDataArray, "NumberOfComponents", "3");
            addAttribute(pointsDataArray, "Format", "ascii");
            pointsDataArray.InnerText = formatPoints(points);

            // Add cells: connectivity
            XmlNode connectivityDataArray = addChildNode(cellsNode, "DataArray");
            addAttribute(connectivityDataArray, "type", "Int32");
            addAttribute(connectivityDataArray, "Name", "connectivity");
            addAttribute(connectivityDataArray, "Format", "ascii");
            connectivityDataArray.InnerText = formatConnectivity(points, triangles);

            // Add cells: offsets
            XmlNode offsetsDataArray = addChildNode(cellsNode, "DataArray");
            addAttribute(offsetsDataArray, "type", "Int32");
            addAttribute(offsetsDataArray, "Name", "offsets");
            addAttribute(offsetsDataArray, "Format", "ascii");
            offsetsDataArray.InnerText = formatOffsets(triangles.Count);

            // Add cells: types
            XmlNode typesDataArray = addChildNode(cellsNode, "DataArray");
            addAttribute(typesDataArray, "type", "Int32");
            addAttribute(typesDataArray, "Name", "types");
            addAttribute(typesDataArray, "Format", "ascii");
            typesDataArray.InnerText = formatTypes(triangles.Count);
        }

        private string formatTypes(int count)
        {
            StringBuilder typesData = new StringBuilder();
            for (int i = 1; i < count + 1; i++)
            {
                typesData.Append(" 5");
            }
            return typesData.ToString();
        }

        private string formatOffsets(int count)
        {
            StringBuilder offsetsData = new StringBuilder();
            for (int i = 1; i < count + 1; i++)
            {
                offsetsData.Append(" ");
                offsetsData.Append((i*3).ToString());
            }
            return offsetsData.ToString();
        }

        private String formatConnectivity(List<Point> points, List<Triangle> triangles)
        {
            // TODO: real formatting, now just test data
            StringBuilder connData = new StringBuilder();
            
            foreach (Triangle triangle in triangles)
            {
                foreach (Point point in triangle.Points)
                {
                    connData.Append(points.IndexOf(point).ToString());
                    connData.Append(" ");
                }
            }
            return connData.ToString();
        }

        private String formatPoints(List<Point> points)
        {
            StringBuilder pointData = new StringBuilder();

            foreach (Point point in points)
            {
                pointData.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:0.0000000} {1:0.0000000} {2:0.0000000} ", point.X, point.Y, point.Z));

                // TODO: formatting
                //pointData.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture,
                //    "{0:0.0000000} {1:0.0000000} {2:0.0000000}\n",
                //    point.X, point.Y, point.Z));
            }

            return pointData.ToString();
        }

        /// <summary>
        /// Adds a new XmlNode to the specified parent node.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="name"></param>
        /// <returns>The newly created child node.</returns>
        private XmlNode addChildNode(XmlNode parentNode, String name)
        {
            XmlNode node = this.xmlDoc.CreateElement(name);
            parentNode.AppendChild(node);
            return node;
        }

        /// <summary>
        /// Adds an attribute to the specified xml node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void addAttribute(XmlNode node, String name, String value)
        {
            XmlAttribute att = this.xmlDoc.CreateAttribute(name);
            att.Value = value;
            node.Attributes.Append(att);
        }
    }
}
