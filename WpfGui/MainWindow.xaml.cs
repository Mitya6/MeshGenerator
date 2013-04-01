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
            Mesh.Geometry geo1 = new Mesh.Geometry(@"..\..\..\Input\meshInput.xml");
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

            // Create and build mesh based on geometry
            Mesh2D mesh = new AdvancingFrontMesh(geo1);
            try
            {
                mesh.BuildMesh();
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

            mesh.SaveVTK();
        }
    }
}
