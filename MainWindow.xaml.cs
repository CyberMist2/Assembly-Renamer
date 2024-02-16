using Assembly_Renamer.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Assembly_Renamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public partial class AssemblyInfo
        {
            public string Name;
            public string Namespace;
        }

        private List<AssemblyInfo> AssembliesFound = new List<AssemblyInfo>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DetectButton_Click(object sender, RoutedEventArgs e)
        {
            DetectAssemblies();
        }

        private void DetectAssemblies()
        {
            AssembliesHolder.Children.Clear();
            AssembliesFound.Clear();

            string folderPath = AppDomain.CurrentDomain.BaseDirectory;

            List<string> ExcludedFolders = new List<string>
            {
                "obj",
                "bin"
            };

            var filteredFiles = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(file => !ExcludedFolders.Any(excludedDir => file.Contains(excludedDir)))
                .Where(file => Path.GetExtension(file).Equals(".csproj", StringComparison.OrdinalIgnoreCase));

            foreach (string filePath in filteredFiles)
            {
                XDocument doc = XDocument.Load(filePath);

                XNamespace ns = doc.Root.GetDefaultNamespace();

                string assemblyName = doc.Descendants(ns + "AssemblyName").FirstOrDefault()?.Value;
                string rootNamespace = doc.Descendants(ns + "RootNamespace").FirstOrDefault()?.Value;

                AssembliesFound.Add(new AssemblyInfo { Name = assemblyName, Namespace = rootNamespace });
            }

            foreach (var assembly in AssembliesFound)
            {
                var assemblyCard = new AssemblyInfoCard();

                assemblyCard.NameHolder.Text = assembly.Name;
                assemblyCard.NamespaceHolder.Text = assembly.Namespace;

                AssembliesHolder.Children.Add(assemblyCard);
            }
        }
    }
}
