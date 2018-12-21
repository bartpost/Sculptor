using System.Windows;

namespace Sculptor.Views
{
    /// <summary>
    /// Interaction logic for TypeSelectionDialog.xaml
    /// </summary>
    public partial class TypeSelectionDialog : Window
    {
        public TypeSelectionDialog()
        {
            InitializeComponent();
        }

        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = TypeViewModelLocator.GetTypeVM();
        }
    }
}
