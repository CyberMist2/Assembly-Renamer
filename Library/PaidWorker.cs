using Assembly_Renamer.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace Assembly_Renamer.Library
{
    internal class PaidWorker
    {
        public partial class AssemblyInfo
        {
            public string Name;
            public string Namespace;
        }

        public static List<AssemblyInfo> AssembliesFound = new List<AssemblyInfo>();

        public static void DetectAssemblies(StackPanel AssembliesHolder)
        {
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

            if (AssembliesFound.Any())
            {
                foreach (var assembly in AssembliesFound)
                {
                    var assemblyCard = new AssemblyInfoCard();

                    assemblyCard.NameHolder.Text = assembly.Name;
                    assemblyCard.NamespaceHolder.Text = assembly.Namespace;

                    AssembliesHolder.Children.Add(assemblyCard);
                }
            }
            else
            {
                AssembliesHolder.Children.Add(new TextBlock
                {
                    Text = "No assemblies found..",
                    Foreground = Brushes.IndianRed,
                    FontWeight = FontWeights.Bold
                });
            }
        }

        public static void RenameAssembly(string old_assembly_name, string new_assembly_name, string old_assembly_namespace, string new_assembly_namespace)
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory;

            List<string> ExcludedFolders = new List<string>
            {
                "obj",
                "bin"
            };

            var filteredFiles = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(file => !ExcludedFolders.Any(excludedDir => file.Contains(excludedDir)))
                .Where(file => Path.GetExtension(file).Equals(".cs", StringComparison.OrdinalIgnoreCase) || 
                            Path.GetExtension(file).Equals(".xaml", StringComparison.OrdinalIgnoreCase) || 
                            Path.GetExtension(file).Equals(".csproj", StringComparison.OrdinalIgnoreCase));


            // step 1: rename resource path
            foreach (string filePath in filteredFiles)
            {
                string content = File.ReadAllText(filePath);

                bool containsWord = content.Contains($"/{old_assembly_name};component/");

                if (containsWord)
                {
                    content = content.Replace($"/{old_assembly_name};component/", $"/{new_assembly_name};component/");

                    File.WriteAllText(filePath, content);
                }
            }

            // step 2: rename namespace
            foreach (string filePath in filteredFiles)
            {
                string content = File.ReadAllText(filePath);

                bool containsWord = content.Contains(old_assembly_namespace);

                if (containsWord)
                {
                    content = content.Replace(old_assembly_namespace, new_assembly_namespace);

                    File.WriteAllText(filePath, content);
                }
            }

            // step 3: rename any executable target references
            foreach (string filePath in filteredFiles)
            {
                string content = File.ReadAllText(filePath);

                bool containsWord = content.Contains($"{old_assembly_name}.exe");

                if (containsWord)
                {
                    content = content.Replace($"{old_assembly_name}.exe", $"{new_assembly_name}.exe");

                    File.WriteAllText(filePath, content);
                }
            }

            // step 4: rename any name
            foreach (string filePath in filteredFiles)
            {
                string content = File.ReadAllText(filePath);

                bool containsWord = content.Contains(old_assembly_name);

                if (containsWord)
                {
                    content = content.Replace(old_assembly_name, new_assembly_name);

                    File.WriteAllText(filePath, content);
                }
            }

            // step 5: delete obj and bin folders
            {
                foreach (string dirPath in Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                    if (dirInfo.Name == "obj" || dirInfo.Name == "bin")
                    {
                        Directory.Delete(dirPath, true);
                    }
                }
            }

            MessageBox.Show("Renaming completed, don't forget:\r\n" +
                "\r\n1. Restart Visual Studio!" +
                "\r\n2. Clean Solution" +
                "\r\n3. Compile");
        }
    }
}
