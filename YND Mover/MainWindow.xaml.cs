using System;
using System.Collections.Generic;
using System.IO;
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
using CodeWalker.GameFiles;
using FolderBrowserEx;
using Microsoft.Win32;
using SharpDX;
using Xceed.Wpf.Toolkit;
using SharpDX.Mathematics;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;
using Path = System.IO.Path;

namespace YND_Mover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> YNDFiles = new();
        private string outputpath;
        
        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
        }

        private void BtnSelectYND_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Multiselect = true,
                DefaultExt = ".ynd",
                Filter = "YND Files (*.ynd)|*.ynd",
                Title = "Select your YND file(s)"
            };

            if (ofd.ShowDialog() != true) return;
            YNDFiles = ofd.FileNames.ToList();
            MessageBox.Show($"Selected {YNDFiles.Count} YND files", "YND Mover", MessageBoxButton.OK, MessageBoxImage.Information);
            
        }

        private void BtnMovePaths_OnClick(object sender, RoutedEventArgs e)
        {
            if(YNDFiles.Count == 0 && string.IsNullOrEmpty(outputpath)) return;
            var nodes = PathUtils.GetNodesFromYNDs(YNDFiles);
            PathUtils.MoveAllNodes(nodes, new Vector3((float)OffsetX.Value, (float)OffsetY.Value, (float)OffsetZ.Value));
            PathUtils.UpdateAreaIDfromNodes(nodes);
            PathUtils.GenerateYNDperArea(nodes, outputpath);
            MessageBox.Show("Done!", "YND Mover", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnOutput_OnClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new()
            {
                Title = "Select your output folder",
                InitialFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                AllowMultiSelect = false
            };
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            outputpath = dlg.SelectedFolder;
            MessageBox.Show($"Output folder set to {Path.GetDirectoryName(outputpath)}", "YND Mover", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}