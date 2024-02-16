using Assembly_Renamer.Library;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Assembly_Renamer.Modules
{
    /// <summary>
    /// Interaction logic for AssemblyInfoCard.xaml
    /// </summary>
    public partial class AssemblyInfoCard : UserControl
    {
        public AssemblyInfoCard()
        {
            InitializeComponent();
        }

        private void NewNameHolder_KeyUp(object sender, KeyEventArgs e)
        {
            NewNamespaceHolder.Text = NewNameHolder.Text.Replace(" ", "_");
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Rename Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                PaidWorker.RenameAssembly(NameHolder.Text, NewNameHolder.Text, NamespaceHolder.Text, NewNamespaceHolder.Text);
            }
        }
    }
}
