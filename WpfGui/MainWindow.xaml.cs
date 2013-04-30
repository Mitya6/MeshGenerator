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
using System.Windows.Threading;
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
        private Mesh.Geometry geo;
        private int triangleCount = 0;
        private Timer timer;

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

            timer = new Timer { AutoReset = true, Interval = 100 };
            timer.Elapsed += (s, eargs) =>
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background, new Action(() =>
                        this.lblWorking.Content = "Triangles: " + triangleCount));
            };
        }

        private void geo_TriangleAdded(object sender, EventArgs e)
        {
            triangleCount++;
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
            this.timer.Stop();
            this.btnStart.IsEnabled = true;
            this.lblWorking.Visibility = System.Windows.Visibility.Hidden;
        }

        private void OnBuildStart()
        {
            this.timer.Start();
            this.btnStart.IsEnabled = false;
            this.lblWorking.Visibility = System.Windows.Visibility.Visible;
        }

        private void BuildMeshes(String fileName)
        {
            // Read Input XML and build geometry
            this.geo = new Mesh.Geometry(directoryPath + fileName);

            // Subscribe to add and remove triangle events
            this.geo.TriangleAdded += geo_TriangleAdded;

            try
            {
                this.geo.Load();
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
                this.geo.BuildMeshes();
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

            this.geo.SaveVTK();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbInputFiles.SelectedItem == null) return;

            OnBuildStart();
            this.bw.RunWorkerAsync(this.lbInputFiles.SelectedValue);
        }
    }
}
