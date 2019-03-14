using Sculptor.DataModels;
using Sculptor.EDBEntityDataModel;
using Sculptor.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Navigation;
using Telerik.Windows.Controls.TreeListView;
using Telerik.Windows.Documents.FormatProviders.Xaml;
using Telerik.Windows.Documents.Model;

namespace Sculptor.ViewModels
{
    public class TemplateRequirementViewModel : ViewModelBase, INotifyPropertyChanged
    {

        #region Constructor
        public TemplateRequirementViewModel()
        {
            // Load the objects in the background
            //var backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += this.OnLoadInBackground;
            //backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            //backgroundWorker.RunWorkerAsync();
            Load();
            FilteredTemplateRequirements = new CollectionViewSource { Source = TemplateRequirements };
            FilteredTemplateRequirements.Filter += ObjectFilter;
        }
        #endregion

        #region Properties

        private ObservableCollection<TemplateRequirementModel> templateRequirements = new ObservableCollection<TemplateRequirementModel>();
        public ObservableCollection<TemplateRequirementModel> TemplateRequirements
        {
            get
            {
                return templateRequirements;
            }
            set
            {
                templateRequirements = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TemplateRequirementModel> backgroundTemplateRequirements = new ObservableCollection<TemplateRequirementModel>();
        public ObservableCollection<TemplateRequirementModel> BackgroundTemplateRequirements
        {
            get
            {
                return backgroundTemplateRequirements;
            }
            set
            {
                backgroundTemplateRequirements = value;
                OnPropertyChanged();
            }
        }

        private CollectionViewSource filteredTemplateRequirements;
        public CollectionViewSource FilteredTemplateRequirements
        {
            get
            {
                return filteredTemplateRequirements;
            }
            set
            {
                if (value != filteredTemplateRequirements)
                {
                    filteredTemplateRequirements = value;
                    OnPropertyChanged();
                }
            }
        }

        private TemplateRequirementModel selectedItem;
        public ObservableCollection<TemplateRequirementModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableItemCollection<TemplateRequirementModel>();
                }
                return selectedItems;
            }
        }

        private ObservableCollection<TemplateRequirementModel> selectedItems;
        public TemplateRequirementModel SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                if (value != this.selectedItem)
                {
                    this.selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isChanged;
        public bool IsChanged
        {
            get
            {
                return this.isChanged;
            }
            set
            {
                if (value != this.isChanged)
                {
                    this.isChanged = value;
                }
            }
        }
        #endregion

        #region Commands

        private ICommand showCommand;
        public ICommand ShowCommand
        {
            get
            {
                if (showCommand == null)
                    showCommand = new RelayCommand(p => this.CanShow(), p => this.Show());
                return showCommand;
            }
        }

        private ICommand refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                    refreshCommand = new RelayCommand(p => true, p => this.Refresh());
                return refreshCommand;
            }
        }

        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new RelayCommand(p => true, p => this.Save());
                return saveCommand;
            }
        }

        private ICommand deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                    deleteCommand = new RelayCommand(p => true, p => this.Delete());
                return deleteCommand;
            }
        }
        #endregion

        #region Events
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnLoadInBackground(object sender, DoWorkEventArgs e)
        {
            Load();
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            TemplateRequirements = BackgroundTemplateRequirements;

            // Create a collection that holds the functionalities of the selected object and add the filter event handler
            // Note: the FilteredObjectFunctionalities collection is updated every time a new object is selected in the object tree 
            // (triggered in the setter of the SelectedItem property)
            FilteredTemplateRequirements = new CollectionViewSource { Source = TemplateRequirements };
            FilteredTemplateRequirements.Filter += ObjectFilter;
        }
        #endregion

        #region Methods

        private void Load()
        {
            // Check if the classes and properties have ben loaded

            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblTemplateRequirement Rec in (from o in eDB.tblTemplateRequirements where (o.Project_ID == Globals.Project_ID) select o))
                {

                    TemplateRequirementModel templateRequirementItem = new TemplateRequirementModel
                    {
                        Project_ID = Rec.Project_ID,
                        Template_ID = Rec.Template_ID,
                        Requirement_ID = Rec.Requirement_ID,
                        RequirementType = Rec.RequirementType,
                        PreFATOk = Rec.PreFATOk,
                        FATOk = Rec.FATOk,
                        FATBy = Rec.FATBy,
                        FATDate = Rec.FATDate,
                        SATOk = Rec.SATOk,
                        SATBy = Rec.SATBy,
                        SATDate = Rec.SATDate,
                        IsChanged = false,
                        IsNew = false,
                        IsDeleted = false,
                        ChildRequirements = new ObservableCollection<TemplateRequirementModel>()
                    };

                    var requirementItem = RequirementViewModelLocator.GetRequirementVM().GetRequirement(templateRequirementItem.Requirement_ID, null);

                    if (requirementItem != null)
                    {
                        templateRequirementItem.ArticleNo = requirementItem.ArticleNo;
                        templateRequirementItem.ArticleHeader = requirementItem.ArticleHeader;
                        templateRequirementItem.RequirementType_ID = requirementItem.RequirementType_ID;

                        foreach (var childItem in requirementItem.ChildRequirements)
                        {
                            TemplateRequirementModel item = new TemplateRequirementModel
                            {
                                Project_ID = childItem.Project_ID,
                                Template_ID = templateRequirementItem.Template_ID,
                                Requirement_ID = childItem.ID,
                                ArticleNo = childItem.ArticleNo,
                                ArticleHeader = childItem.ArticleHeader,
                                RequirementType = "Requirement",
                                RequirementType_ID = childItem.RequirementType_ID,
                                ChildRequirements = new ObservableCollection<TemplateRequirementModel>()
                            };
                            templateRequirementItem.ChildRequirements.Add(item);
                        }

                        TemplateRequirements.Add(templateRequirementItem);
                    }
                }
            }
        }

        private bool CanShow()
        {
            return true;
        }

        private void Show()
        {
            StackPanel sp = new StackPanel();
            RadRichTextBox rtb = new RadRichTextBox();

            var reqItem = RequirementViewModelLocator.GetRequirementVM().GetRequirement(SelectedItem.Requirement_ID, null);
            XamlFormatProvider provider = new XamlFormatProvider();
            RadDocument document;
            if (reqItem.Content != null)
                document = provider.Import(reqItem.Content);
            else
                document = new RadDocument();
            rtb.Document = document;
            rtb.Document.LayoutMode = DocumentLayoutMode.Flow;
            sp.Children.Add(rtb);

            var window = new RadWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Header = SelectedItem.ArticleHeader,
                Content = sp,
                Width = 700,
                Height = 500,
            };
            window.Show();
        }

        private void Delete()
        {
            // ToDo: Deleting items in the collection only works using the Del key for now. 
            // Implement delete method to also provide option using context menu
        }

        public void Save()
        {
            EDBEntities eDB = new EDBEntities();

            // To determine which items have been deleted in the collection, get all requirements of the project stored in the database table first
            var tblTemplateRequirements = eDB.tblTemplateRequirements.Where(p => p.Project_ID == Globals.Project_ID);

            // Check if each requirement of the table exists in the requirements collection
            // if not, delete the requirement in the table
            foreach (var templateRequirementRec in tblTemplateRequirements)
            {
                var templateRequirementItem = GetTemplateRequirement(templateRequirementRec.Template_ID, templateRequirementRec.Requirement_ID);
                if (templateRequirementItem == null) // requirement not found in collection
                    eDB.tblTemplateRequirements.Remove(templateRequirementRec);
            }

            // Add and update requirements recursively
            SaveLevel(TemplateRequirements, eDB);
            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Fault while saving template requirements: \n" + ex.Message
                });
            }
            IsChanged = false;
        }

        private void SaveLevel(ObservableCollection<TemplateRequirementModel> treeLevel, EDBEntities eDB)
        {
            try
            {
                if (treeLevel != null)
                {
                    foreach (var templateRequirementItem in treeLevel)
                    {
                        if (templateRequirementItem.IsNew)
                        {
                            tblTemplateRequirement NewRec = new tblTemplateRequirement();
                            var Rec = eDB.tblTemplateRequirements.Add(NewRec);
                            Rec.Template_ID = templateRequirementItem.Template_ID;
                            Rec.Requirement_ID = templateRequirementItem.Requirement_ID;
                            Rec.PreFATOk = templateRequirementItem.PreFATOk;
                            Rec.FATOk = templateRequirementItem.FATOk;
                            Rec.FATBy = templateRequirementItem.FATBy;
                            Rec.FATDate = templateRequirementItem.FATDate;
                            Rec.SATOk = templateRequirementItem.SATOk;
                            Rec.SATBy = templateRequirementItem.SATBy;
                            Rec.SATDate = templateRequirementItem.SATDate;
                            Rec.Project_ID = Globals.Project_ID;
                            //Rec.RequirementType_ID = objectRequirementItem.RequirementType_ID;
                            templateRequirementItem.IsNew = false;
                            RequirementViewModel requirementVM = RequirementViewModelLocator.GetRequirementVM();
                            RequirementModel requirementItem = requirementVM.GetRequirement(templateRequirementItem.Requirement_ID);
                        }
                        if (templateRequirementItem.IsChanged)
                        {
                            tblTemplateRequirement Rec = eDB.tblTemplateRequirements.Where(o => o.Template_ID == templateRequirementItem.Template_ID && o.Requirement_ID == templateRequirementItem.Requirement_ID).FirstOrDefault();
                            Rec.PreFATOk = templateRequirementItem.PreFATOk;
                            Rec.FATOk = templateRequirementItem.FATOk;
                            Rec.FATBy = templateRequirementItem.FATBy;
                            Rec.FATDate = templateRequirementItem.FATDate;
                            Rec.SATOk = templateRequirementItem.SATOk;
                            Rec.SATBy = templateRequirementItem.SATBy;
                            Rec.SATDate = templateRequirementItem.SATDate;
                            templateRequirementItem.IsChanged = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RadWindow.Alert("Fault while saving to database: " + ex.Message);
            }
        }

        public void Refresh()
        {
            TemplateRequirements.Clear();
            FilteredTemplateRequirements.View.Refresh();
            Load();
        }

        public void AssociateWithTemplate(TreeListViewRow destination)
        {
            try
            {
                // Associate all selected templates
                foreach (var templateItem in TemplateViewModelLocator.GetTemplateVM().SelectedItems)
                    // With all selected requirements (m:n)
                    foreach (var requirementItem in RequirementViewModelLocator.GetRequirementVM().SelectedItems)
                    {
                        TemplateRequirementModel templateRequirementItem = new TemplateRequirementModel
                        {
                            IsNew = true,
                            IsChanged = false,
                            IsDeleted = false,
                            Project_ID = Globals.Project_ID,
                            Template_ID = templateItem.ID,
                            Requirement_ID = requirementItem.ID,
                            RequirementType_ID = requirementItem.RequirementType_ID,
                            ArticleNo = requirementItem.ArticleNo,
                            ArticleHeader = requirementItem.ArticleHeader,
                            //RequirementType = Globals.DraggedItem.Type,
                            ChildRequirements = new ObservableCollection<TemplateRequirementModel>(),
                        };

                        foreach (var childItem in requirementItem.ChildRequirements)
                        {
                            TemplateRequirementModel templateRequirementChildItem = new TemplateRequirementModel
                            {
                                IsNew = true,
                                IsChanged = false,
                                IsDeleted = false,
                                Project_ID = Globals.Project_ID,
                                Template_ID = templateRequirementItem.Template_ID,
                                Requirement_ID = childItem.ID,
                                RequirementType_ID = childItem.RequirementType_ID,
                                //objectFunctionalityChildItem.FunctionParent_ID = childItem.Parent_ID;
                                ArticleNo = childItem.ArticleNo,
                                ArticleHeader = childItem.ArticleHeader,
                                ChildRequirements = new ObservableCollection<TemplateRequirementModel>()
                            };
                            templateRequirementItem.ChildRequirements.Add(templateRequirementChildItem);
                        };

                        TemplateRequirements.Add(templateRequirementItem);
                    }
            }
            catch (Exception ex)
            {
                RadWindow.Alert(ex.Message);
            }
        }

        public void ObjectFilter(object sender, FilterEventArgs e)
        {
            TemplateViewModel ovm = TemplateViewModelLocator.GetTemplateVM();
            TemplateModel om = ovm.SelectedItem;
            if (e.Item != null && om != null)
                e.Accepted = (e.Item as TemplateRequirementModel).Template_ID == TemplateViewModelLocator.GetTemplateVM().SelectedItem.ID &&
                             (e.Item as TemplateRequirementModel).IsDeleted == false;

        }

        private TemplateRequirementModel GetTemplateRequirement(Guid? template_ID, Guid? requirement_ID)
        {
            foreach (var templateRequirementItem in TemplateRequirements)
            {
                if (templateRequirementItem.Template_ID == template_ID && templateRequirementItem.Requirement_ID == requirement_ID) return templateRequirementItem;
            }
            return null;
        }
        #endregion
    }
}
