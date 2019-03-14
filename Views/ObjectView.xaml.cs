using Sculptor;
using Sculptor.Interfaces;
using Sculptor.ViewModels;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;
using Telerik.Windows.Persistence.Storage;

namespace Sculptor
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class ObjectView : UserControl
    {
        public ObjectView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Link View Models to the Trees and popups
            DataContext = ObjectViewModelLocator.GetObjectVM();
            TemplateTreeListView.DataContext = TemplateViewModelLocator.GetTemplateVM();
            PropertyTreeListView.DataContext = PropertyViewModelLocator.GetPropertyVM();
            RequirementTreeListView.DataContext = RequirementViewModelLocator.GetRequirementVM();
            ObjectAssociationTreeListView.DataContext = ObjectAssociationViewModelLocator.GetObjectAssociationVM();
            ObjectRequirementTreeListView.DataContext = ObjectRequirementViewModelLocator.GetObjectRequirementVM();
        }
    }

}
