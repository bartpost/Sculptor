using System.Windows;
using System.Windows.Controls;

namespace Sculptor
{
    /// <summary>
    /// Interaction logic for ControlObjectView.xaml
    /// </summary>
    public partial class ControlObjectView : UserControl
    {
        public ControlObjectView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Link View Models to the Trees and popups
            DataContext = ControlObjectViewModelLocator.GetControlObjectVM();
            HardIOTreeListView.DataContext = HardIOViewModelLocator.GetHardIOVM();
            ControlPropertyTreeListView.DataContext = PropertyViewModelLocator.GetPropertyVM();
            //RequirementTreeListView.DataContext = RequirementViewModelLocator.GetRequirementVM();
            ControlObjectAssociationTreeListView.DataContext = ControlObjectAssociationViewModelLocator.GetControlObjectAssociationVM();
            //ObjectRequirementTreeListView.DataContext = ObjectRequirementViewModelLocator.GetObjectRequirementVM();
        }
    }
}
 