using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mesh;

namespace WpfGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Read Input XML and build geometry
            Mesh.Geometry geo1 = new Mesh.Geometry(@"..\..\..\Input\rectangleInput.xml");
            try
            {
                geo1.Load();
            }
            catch (ApplicationException appEx)
            {
                MessageBox.Show(appEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);   
            }
            catch (Exception)
            {
                MessageBox.Show("Error while reading input file", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Build meshes based on geometry
            try
            {
                geo1.BuildMeshes();
            }
            catch (ApplicationException appEx)
            {
                MessageBox.Show(appEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Error while building mesh", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            geo1.SaveVTK();
        }
    }
}
