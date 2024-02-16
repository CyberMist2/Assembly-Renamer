using Assembly_Renamer.Library;
using Assembly_Renamer.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace Assembly_Renamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DetectButton_Click(object sender, RoutedEventArgs e)
        {
            AssembliesHolder.Children.Clear();
            PaidWorker.DetectAssemblies(AssembliesHolder);
        }
    }
}
