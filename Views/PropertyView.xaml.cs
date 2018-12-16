
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
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = PropertyViewModelLocator.GetPropertyVM();
            AspectListView.DataContext = AspectViewModelLocator.GetAspectVM();
            AttributeListView.DataContext = AttributeViewModelLocator.GetAttributeVM();

            IColumnFilterDescriptor DeletedPropertyFilter = this.PropertyTreeListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            DeletedPropertyFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            DeletedPropertyFilter.FieldFilter.Filter1.Value = "False";

            IColumnFilterDescriptor DeletedAspectFilter = this.AspectListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            DeletedAspectFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            DeletedAspectFilter.FieldFilter.Filter1.Value = "False";

            IColumnFilterDescriptor DeletedAttributeFilter = this.AttributeListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            DeletedAttributeFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            DeletedAttributeFilter.FieldFilter.Filter1.Value = "False";
        }
    }
}
