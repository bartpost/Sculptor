using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;

namespace Sculptor
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class TemplateView : UserControl
    {
        public TemplateView()
        {
            InitializeComponent();
            DataContext = TemplateViewModelLocator.GetTemplateVM();
            PropertyTreeListView.DataContext = PropertyViewModelLocator.GetPropertyVM();
            RequirementTreeListView.DataContext = RequirementViewModelLocator.GetRequirementVM();
            TemplateAssociationTreeListView.DataContext = TemplateAssociationViewModelLocator.GetTemplateAssociationVM();
            TemplateRequirementTreeListView.DataContext = TemplateRequirementViewModelLocator.GetTemplateRequirementVM();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //IColumnFilterDescriptor DeletedTemplateFilter = this.TemplateTreeListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            //DeletedTemplateFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            //DeletedTemplateFilter.FieldFilter.Filter1.Value = "False";

            //// Filter deleted lines of Property Tree
            //IColumnFilterDescriptor DeletedPropertyFilter = this.PropertyTreeListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            //DeletedPropertyFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            //DeletedPropertyFilter.FieldFilter.Filter1.Value = "False";

            //// Filter deleted lines of Requirement Tree
            //IColumnFilterDescriptor DeletedRequirementFilter = this.RequirementTreeListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            //DeletedRequirementFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            //DeletedRequirementFilter.FieldFilter.Filter1.Value = "False";
        }
    }
}
