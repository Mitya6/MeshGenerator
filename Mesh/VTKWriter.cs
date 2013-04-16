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
            FillXmlDocument(points, triangles);
            this.xmlDoc.Save(@"VTKOutput\" + this.fileName + ".vtu");
        }

        private void FillXmlDocument(List<Point> points, List<Triangle> triangles)
        {
            // Add VTK root node and root attributes
            XmlNode rootNode = this.xmlDoc.CreateElement("VTKFile");
            this.xmlDoc.AppendChild(rootNode);

            AddAttribute(rootNode, "type", "UnstructuredGrid");
            AddAttribute(rootNode, "version", "0.1");
            AddAttribute(rootNode, "byte_order", "LittleEndian");

            // Add elements containing the points and the cells
            XmlNode gridNode = AddChildNode(rootNode, "UnstructuredGrid");
            XmlNode pieceNode = AddChildNode(gridNode, "Piece");

            AddAttribute(pieceNode, "NumberOfPoints", points.Count.ToString());
            AddAttribute(pieceNode, "NumberOfCells", triangles.Count.ToString());

            XmlNode pointsNode = AddChildNode(pieceNode, "Points");
            XmlNode cellsNode = AddChildNode(pieceNode, "Cells");

            // Add points
            XmlNode pointsDataArray = AddChildNode(pointsNode, "DataArray");
            AddAttribute(pointsDataArray, "type", "Float32");
            AddAttribute(pointsDataArray, "NumberOfComponents", "3");
            AddAttribute(pointsDataArray, "Format", "ascii");
            pointsDataArray.InnerText = FormatPoints(points);

            // Add cells: connectivity
            XmlNode connectivityDataArray = AddChildNode(cellsNode, "DataArray");
            AddAttribute(connectivityDataArray, "type", "Int32");
            AddAttribute(connectivityDataArray, "Name", "connectivity");
            AddAttribute(connectivityDataArray, "Format", "ascii");
            connectivityDataArray.InnerText = FormatConnectivity(points, triangles);

            // Add cells: offsets
            XmlNode offsetsDataArray = AddChildNode(cellsNode, "DataArray");
            AddAttribute(offsetsDataArray, "type", "Int32");
            AddAttribute(offsetsDataArray, "Name", "offsets");
            AddAttribute(offsetsDataArray, "Format", "ascii");
            offsetsDataArray.InnerText = FormatOffsets(triangles.Count);

            // Add cells: types
            XmlNode typesDataArray = AddChildNode(cellsNode, "DataArray");
            AddAttribute(typesDataArray, "type", "Int32");
            AddAttribute(typesDataArray, "Name", "types");
            AddAttribute(typesDataArray, "Format", "ascii");
            typesDataArray.InnerText = FormatTypes(triangles.Count);
        }

        private string FormatTypes(int count)
        {
            StringBuilder typesData = new StringBuilder();
            for (int i = 1; i < count + 1; i++)
            {
                typesData.Append(" 5");
            }
            return typesData.ToString();
        }

        private string FormatOffsets(int count)
        {
            StringBuilder offsetsData = new StringBuilder();
            for (int i = 1; i < count + 1; i++)
            {
                offsetsData.Append(" ");
                offsetsData.Append((i*3).ToString());
            }
            return offsetsData.ToString();
        }

        private String FormatConnectivity(List<Point> points, List<Triangle> triangles)
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

        private String FormatPoints(List<Point> points)
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
        private XmlNode AddChildNode(XmlNode parentNode, String name)
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
        private void AddAttribute(XmlNode node, String name, String value)
        {
            XmlAttribute att = this.xmlDoc.CreateAttribute(name);
            att.Value = value;
            node.Attributes.Append(att);
        }
    }
}
