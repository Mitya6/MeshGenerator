using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        private String directoryPath = @"..\..\..\Input\";
        private BackgroundWorker bw;

        public MainWindow()
        {
            InitializeComponent();

            this.bw = new BackgroundWorker();
            this.bw.DoWork += (s, eargs) => BuildMeshes(eargs.Argument.ToString());
            this.bw.RunWorkerCompleted += (s, eargs) => OnBuildFinish();

            List<string> fileNames = ReadInputFileNames();
            foreach (String fileName in fileNames)
            {
                this.lbInputFiles.Items.Add(fileName);
            }
        }

        private List<string> ReadInputFileNames()
        {
            DirectoryInfo di = new DirectoryInfo(directoryPath);
            var fileInfoArray = di.EnumerateFiles("*.xml");
            List<string> fileNames = new List<string>();
            foreach (var fileInfo in fileInfoArray)
            {
                fileNames.Add(fileInfo.Name);
            }
            return fileNames;
        }

        private void OnBuildFinish()
        {
            this.btnStart.IsEnabled = true;
            this.lblWorking.Visibility = System.Windows.Visibility.Hidden;
        }

        private void OnBuildStart()
        {
            this.btnStart.IsEnabled = false;
            this.lblWorking.Visibility = System.Windows.Visibility.Visible;
        }

        private void BuildMeshes(String fileName)
        {
            // Read Input XML and build geometry
            Mesh.Geometry geo1 = new Mesh.Geometry(directoryPath + fileName);
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

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbInputFiles.SelectedItem == null) return;

            OnBuildStart();
            this.bw.RunWorkerAsync(this.lbInputFiles.SelectedValue);
        }
    }
}
