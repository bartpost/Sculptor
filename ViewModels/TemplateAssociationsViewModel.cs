using Sculptor.DataModels;
using Sculptor.EDBEntityDataModel;
using Sculptor.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;

namespace Sculptor
{
    public class TemplateAssociationViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ObservableCollectionWithItemChanged<TemplateAssociationModel> templateAssociations = new ObservableCollectionWithItemChanged<TemplateAssociationModel>();
        private ObservableCollectionWithItemChanged<TemplateAssociationModel> backgroundTemplateAssociations = new ObservableCollectionWithItemChanged<TemplateAssociationModel>();
        private TemplateAssociationModel selectedItem;
        private ObservableCollectionWithItemChanged<TemplateAssociationModel> selectedItems;
        private CollectionViewSource filteredTemplateAssociations;
        private bool isChanged;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand deleteCommand;

        #region Constructor
        public TemplateAssociationViewModel()
        {
            //Load the properties in the background
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        #endregion

        #region Properties

        public ObservableCollectionWithItemChanged<TemplateAssociationModel> TemplateAssociations
        {
            get
            {
                return templateAssociations;
            }
            set
            {
                templateAssociations = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionWithItemChanged<TemplateAssociationModel> BackgroundTemplateAssociations
        {
            get
            {
                return backgroundTemplateAssociations;
            }
            set
            {
                backgroundTemplateAssociations = value;
                OnPropertyChanged();
            }
        }

        public CollectionViewSource FilteredTemplateAssociations
        {
            get
            {
                return filteredTemplateAssociations;
            }
            set
            {
                if (value != filteredTemplateAssociations)
                {
                    filteredTemplateAssociations = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollectionWithItemChanged<TemplateAssociationModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<TemplateAssociationModel>();
                }
                return selectedItems;
            }
        }

        public TemplateAssociationModel SelectedItem
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

        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(
                        p => this.CanSave(),
                        p => this.Save());
                }
                return saveCommand;
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new RelayCommand(
                        p => this.CanRefresh(),
                        p => this.Refresh());
                }
                return refreshCommand;
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    deleteCommand = new RelayCommand(
                        p => this.CanDelete(),
                        p => this.Delete());
                }
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
            Load(null);
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            TemplateAssociations = BackgroundTemplateAssociations;

            // Create a collection that holds the Associations of the selected template and add the filter event handler
            // Note: the FilteredTemplateAssociations collection is updated every time a new template is selected in the template tree 
            // (triggered in the setter of the SelectedItem property)
            FilteredTemplateAssociations = new CollectionViewSource
            {
                Source = TemplateAssociations
            };
            FilteredTemplateAssociations.Filter += TemplateFilter;
        }
        #endregion

        #region Methods

        private void Load(Guid? associationParent_ID)
        {
            // Check if the classes and properties have ben loaded. Necessary because of the various background workers
            while (!PropertyViewModelLocator.IsLoaded());

            try
            {
                using (EDBEntities eDB = new EDBEntities())
                {
                    foreach (tblTemplateAssociation Rec in (from o in eDB.tblTemplateAssociations where (o.Project_ID == Globals.Project_ID) select o))
                    {
                        TemplateAssociationModel templateAssociationItem = new TemplateAssociationModel
                        {
                            Project_ID = Rec.Project_ID,
                            Template_ID = Rec.Template_ID,
                            Association_ID = Rec.Association_ID,
                            Value = Rec.Value,
                            AssociationType = Rec.AssociationType,
                            IsChanged = false,
                            IsNew = false,
                            IsDeleted = false,
                            ChildAssociations = new ObservableCollectionWithItemChanged<TemplateAssociationModel>()
                        };
                        switch (templateAssociationItem.AssociationType)
                        {
                            case "Property":
                                var propertyItem = PropertyViewModelLocator.GetPropertyVM().GetProperty(templateAssociationItem.Association_ID, null);
                                if (propertyItem != null)
                                {
                                    templateAssociationItem.Name = propertyItem.PropertyName;
                                    templateAssociationItem.Description = propertyItem.Description;
                                    foreach (var childItem in propertyItem.ChildProperties)
                                    {
                                        TemplateAssociationModel item = new TemplateAssociationModel
                                        {
                                            Project_ID = childItem.Project_ID,
                                            Template_ID = templateAssociationItem.Template_ID,
                                            Association_ID = childItem.ID,
                                            Name = childItem.PropertyName,
                                            Description = childItem.Description,
                                            AssociationType = "Property",
                                            ChildAssociations = new ObservableCollectionWithItemChanged<TemplateAssociationModel>()
                                        };
                                        templateAssociationItem.ChildAssociations.Add(item);
                                    }
                                }
                                else
                                {
                                    throw new System.InvalidOperationException(String.Format("Association without source\nProperty ID: {0}\nFix in database", templateAssociationItem.Association_ID));
                                }
                                break;
                            case "ClassProperty":
                                break;
                        }

                        BackgroundTemplateAssociations.Add(templateAssociationItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate { RadWindow.Alert(ex.Message); });
            }
        }

        private bool CanDelete()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Delete()
        {
            // TODO: make this a recursive function so we can step through multiple levels
            if (SelectedItem != null)
            {
                foreach (var childItem in SelectedItem.ChildAssociations)
                {
                    childItem.IsChanged = false;
                    childItem.IsNew = false;
                    childItem.IsDeleted = true;
                }

                SelectedItem.IsChanged = false;
                SelectedItem.IsNew = false;
                SelectedItem.IsDeleted = true;

                // Because the FilteredTemplateAssociations collection doesn't refresh on PropertyChanged, we have to refresh the collection
                FilteredTemplateAssociations.View.Refresh();
            }
        }

        private bool CanSave()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            try
            {
                EDBEntities eDB = new EDBEntities();
                SaveLevel(TemplateAssociations, eDB);
                eDB.SaveChanges();
            }
            catch (Exception ex)
            { 
                RadWindow.Alert("Fault while saving template associations: " + ex.Message);
            }
            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(ObservableCollection<TemplateAssociationModel> treeLevel, EDBEntities eDB)
        {
            if (treeLevel != null)
            {
                foreach (var templateAssociationItem in treeLevel)
                {

                    if (templateAssociationItem.IsNew)
                    {
                        tblTemplateAssociation NewRec = new tblTemplateAssociation();
                        var Rec = eDB.tblTemplateAssociations.Add(NewRec);
                        Rec.Template_ID = templateAssociationItem.Template_ID;
                        Rec.Association_ID = templateAssociationItem.Association_ID;
                        Rec.Project_ID = Globals.Project_ID;
                        Rec.AssociationType = templateAssociationItem.AssociationType;
                        templateAssociationItem.IsNew = false;
                        //switch (templateAssociationItem.AssociationType)
                        //{
                        //    case "Template":
                        //        TemplateViewModel classVM = TemplateViewModelLocator.GetTemplateVM();
                        //        TemplateModel classItem = classVM.GetTemplate(templateAssociationItem.Association_ID);

                        //        break;
                        //}
                    }
                    if (templateAssociationItem.IsChanged)
                    {
                        //tblObjectAssociation Rec = eDB.tblObjectAssociations.Where(o => o.ID == objectAssociationItem.Object_ID).FirstOrDefault();
                        //Rec.Parent_ID = propertyItem.Parent_ID;
                        //Rec.PropertyName = propertyItem.PropertyName;
                        //Rec.Description = propertyItem.Description;
                        //Rec.Project_ID = propertyItem.Project_ID;
                        //Rec.PropertyType_ID = propertyItem.PropertyType_ID;
                        //Rec.Aspect = propertyItem.Aspect;
                        //Rec.Attribute1 = propertyItem.Attribute1;
                        //Rec.Attribute2 = propertyItem.Attribute2;
                        //Rec.Attribute3 = propertyItem.Attribute3;
                        //propertyItem.IsChanged = false;
                    }
                    if (templateAssociationItem.IsDeleted)
                    {
                        tblTemplateAssociation Rec = eDB.tblTemplateAssociations.Where(o => (o.Template_ID == templateAssociationItem.Template_ID && o.Association_ID == templateAssociationItem.Association_ID)).FirstOrDefault();
                        if (Rec != null)
                            eDB.tblTemplateAssociations.Remove(Rec);
                    }
                    // Recursive call
                    //if (propertyItem.ChildProperties != null) SaveLevel(propertyItem.ChildProperties, eDB);
                }
            }
        }

        private bool CanRefresh()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            TemplateAssociations.Clear();
            FilteredTemplateAssociations.View.Refresh();
            Load(null);
        }

        public void AssociateWithTemplate(TreeListViewRow destination)
        {
            //if (destination != null)
            //{
            try
            {
                foreach (var templateItem in TemplateViewModelLocator.GetTemplateVM().SelectedItems)
                {
                    switch (Globals.DraggedItem.Type)
                    {

                        case "Property":
                            foreach (var propertyItem in PropertyViewModelLocator.GetPropertyVM().SelectedItems)
                            {
                                TemplateAssociationModel templateAssociationItem = new TemplateAssociationModel
                                {
                                    IsNew = true,
                                    IsChanged = false,
                                    IsDeleted = false,
                                    Project_ID = Globals.Project_ID,
                                    Template_ID = templateItem.ID,
                                    AssociationType = Globals.DraggedItem.Type,
                                    Value = "",
                                    ChildAssociations = new ObservableCollectionWithItemChanged<TemplateAssociationModel>(),
                                };

                                templateAssociationItem.Association_ID = propertyItem.ID;
                                templateAssociationItem.Name = propertyItem.PropertyName;
                                templateAssociationItem.Description = propertyItem.Description;

                                foreach (var childItem in propertyItem.ChildProperties)
                                {
                                    TemplateAssociationModel templateAssociationChildItem = new TemplateAssociationModel
                                    {
                                        IsNew = true,
                                        IsChanged = false,
                                        IsDeleted = false,
                                        AssociationType = "Property",
                                        Project_ID = Globals.Project_ID,
                                        Template_ID = templateItem.ID,
                                        Association_ID = childItem.ID,
                                        Name = childItem.PropertyName,
                                        Description = childItem.Description,
                                        Value = "",
                                        ChildAssociations = new ObservableCollectionWithItemChanged<TemplateAssociationModel>()
                                    };
                                    templateAssociationItem.ChildAssociations.Add(templateAssociationChildItem);
                                }
                                TemplateAssociations.Add(templateAssociationItem);
                            }
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                RadWindow.Alert(ex.Message);
            }
            //}
        }

        public void TemplateFilter(object sender, FilterEventArgs e)
        {
            TemplateViewModel ovm = TemplateViewModelLocator.GetTemplateVM();
            TemplateModel om = ovm.SelectedItem;
            if (e.Item != null && om != null)
                e.Accepted = (e.Item as TemplateAssociationModel).Template_ID == TemplateViewModelLocator.GetTemplateVM().SelectedItem.ID &&
                             (e.Item as TemplateAssociationModel).IsDeleted == false;

        }
    }
    #endregion

}
