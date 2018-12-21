using System.Windows;


namespace Sculptor.Views
{
    /// <summary>
    /// Interaction logic for AddObjectTypeDialog.xaml
    /// </summary>
    public partial class TypeEditDialog : Window
    {
        public TypeEditDialog()
        {
            InitializeComponent();
        }

        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = TypeViewModelLocator.GetTypeVM();
        }
    }
}
