
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;

namespace Sculptor
{
    /// <summary>
    /// Interaction logic for PropertyTree.xaml
    /// </summary>
    public partial class PropertyView : UserControl
    {
        public PropertyView()
        {
            InitializeComponent();
            DataContext = PropertyViewModelLocator.GetPropertyVM();
            AspectListView.DataContext = AspectViewModelLocator.GetAspectVM();
            AttributeListView.DataContext = AttributeViewModelLocator.GetAttributeVM();
        }
    }
}
