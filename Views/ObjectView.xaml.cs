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
            TemplateTypePopup.DataContext = TemplateTreeListView.DataContext;
            PropertyTreeListView.DataContext = PropertyViewModelLocator.GetPropertyVM();
            PropertyTypePopup.DataContext = PropertyTreeListView.DataContext;
            RequirementTreeListView.DataContext = RequirementViewModelLocator.GetRequirementVM();
            ObjectAssociationTreeListView.DataContext = ObjectAssociationViewModelLocator.GetObjectAssociationVM();
            ObjectRequirementTreeListView.DataContext = ObjectRequirementViewModelLocator.GetObjectRequirementVM();


            // Filter deleted lines of Object Tree
            IColumnFilterDescriptor DeletedObjectFilter = this.ObjectTreeListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            DeletedObjectFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            DeletedObjectFilter.FieldFilter.Filter1.Value = "False";

            // Filter deleted lines of Class Tree
            IColumnFilterDescriptor DeletedClassFilter = this.TemplateTreeListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            DeletedClassFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            DeletedClassFilter.FieldFilter.Filter1.Value = "False";

            // Filter deleted lines of Property Tree
            IColumnFilterDescriptor DeletedPropertyFilter = this.PropertyTreeListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            DeletedPropertyFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            DeletedPropertyFilter.FieldFilter.Filter1.Value = "False";

            // Filter deleted lines of Requirement Tree
            IColumnFilterDescriptor DeletedRequirementFilter = this.RequirementTreeListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            DeletedRequirementFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            DeletedRequirementFilter.FieldFilter.Filter1.Value = "False";
        }
    }

}
