using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

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
            //PropertyTreeListView.DataContext = PropertyViewModelLocator.GetPropertyVM();
            //RequirementTreeListView.DataContext = RequirementViewModelLocator.GetRequirementVM();
            //ObjectAssociationTreeListView.DataContext = ObjectAssociationViewModelLocator.GetObjectAssociationVM();
            //ObjectRequirementTreeListView.DataContext = ObjectRequirementViewModelLocator.GetObjectRequirementVM();
        }
    }
}
