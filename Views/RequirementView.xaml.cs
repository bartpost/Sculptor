using Sculptor;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.RichTextBoxUI;
using Telerik.Windows.Controls.RichTextBoxUI.Dialogs;
using Telerik.Windows.Data;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using Telerik.Windows.Documents.Flow.FormatProviders.Rtf;
using Telerik.Windows.Documents.Flow.FormatProviders.Txt;
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.FormatProviders;
using Telerik.Windows.Documents.FormatProviders.Xaml;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.Proofing;
using Telerik.Windows.Documents.UI.Extensibility;

namespace Sculptor
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RequirementView : UserControl
    {

        public RequirementView()
        {
            //RadCompositionInitializer.Catalog = new TypeCatalog(
            //                                    // format providers 
            //                                    typeof(XamlFormatProvider),
            //                                    typeof(RtfFormatProvider),
            //                                    typeof(DocxFormatProvider),

            //                                    // mini toolbars 
            //                                    typeof(SelectionMiniToolBar),
            //                                    typeof(ImageMiniToolBar),

            //                                    // context menu 
            //                                    typeof(Telerik.Windows.Controls.RichTextBoxUI.ContextMenu),

            //                                    // the default English spellchecking dictionary 
            //                                    typeof(RadEn_USDictionary),

            //                                    // dialogs 
            //                                    typeof(AddNewBibliographicSourceDialog),
            //                                    typeof(ChangeEditingPermissionsDialog),
            //                                    typeof(EditCustomDictionaryDialog),
            //                                    typeof(FindReplaceDialog),
            //                                    typeof(FloatingBlockPropertiesDialog),
            //                                    typeof(FontPropertiesDialog),
            //                                    typeof(ImageEditorDialog),
            //                                    typeof(InsertCaptionDialog),
            //                                    typeof(InsertCrossReferenceWindow),
            //                                    typeof(InsertDateTimeDialog),
            //                                    typeof(InsertTableDialog),
            //                                    typeof(InsertTableOfContentsDialog),
            //                                    typeof(ManageBibliographicSourcesDialog),
            //                                    typeof(ManageBookmarksDialog),
            //                                    typeof(ManageStylesDialog),
            //                                    typeof(NotesDialog),
            //                                    typeof(ProtectDocumentDialog),
            //                                    typeof(RadInsertHyperlinkDialog),
            //                                    typeof(RadInsertSymbolDialog),
            //                                    typeof(RadParagraphPropertiesDialog),
            //                                    typeof(SetNumberingValueDialog),
            //                                    typeof(SpellCheckingDialog),
            //                                    typeof(StyleFormattingPropertiesDialog),
            //                                    typeof(TableBordersDialog),
            //                                    typeof(TablePropertiesDialog),
            //                                    typeof(TabStopsPropertiesDialog));
            InitializeComponent();
            this.radRichTextBox.FontFamily = new FontFamily("Arial");
            this.radRichTextBox.FontSize = Unit.PointToDip(10);
            this.radRichTextBox.DocumentInheritsDefaultStyleSettings = true;
        }

        public object DeletedObjectRequirementFilter { get; private set; }

        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as RadContextMenu).DataContext = RequirementViewModelLocator.GetRequirementVM();

            //(sender as RadContextMenu).ItemsSource = ObjectViewModelLocator.GetObjects().ObjectTypes;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = RequirementViewModelLocator.GetRequirementVM();
            // Link the RichTextBoxRibbon to the commands defined in the RichTextBox control, part of the Telerik library
            radRichTextBoxRibbonUI.DataContext = radRichTextBox.Commands;

            IColumnFilterDescriptor DeletedRequirementFilter = this.RequirementTreeListView.Columns["IsDeleted"].ColumnFilterDescriptor;
            DeletedRequirementFilter.FieldFilter.Filter1.Operator = FilterOperator.IsEqualTo;
            DeletedRequirementFilter.FieldFilter.Filter1.Value = "False";

        }

    }
}
