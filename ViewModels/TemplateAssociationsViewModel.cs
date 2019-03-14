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
using TD = Telerik.Windows.Data;

namespace Sculptor
{
    public class TemplateAssociationViewModel : ViewModelBase
    {
        private TD.ObservableItemCollection<TemplateAssociationModel> templateAssociations = new TD.ObservableItemCollection<TemplateAssociationModel>();
        private TD.ObservableItemCollection<TemplateAssociationModel> backgroundTemplateAssociations = new TD.ObservableItemCollection<TemplateAssociationModel>();
        private TemplateAssociationModel selectedItem;
        private TD.ObservableItemCollection<TemplateAssociationModel> selectedItems;
        private CollectionViewSource filteredTemplateAssociations;
        private bool isChanged;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand deleteCommand;

        #region Constructor
        public TemplateAssociationViewModel()
        {
            //Load the properties in the background
            //var backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += this.OnLoadInBackground;
            //backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            //backgroundWorker.RunWorkerAsync();
            Load(null);
            FilteredTemplateAssociations = new CollectionViewSource { Source = TemplateAssociations };
            FilteredTemplateAssociations.Filter += TemplateFilter;
        }

        #endregion

        #region Properties

        public TD.ObservableItemCollection<TemplateAssociationModel> TemplateAssociations
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

        public TD.ObservableItemCollection<TemplateAssociationModel> BackgroundTemplateAssociations
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

        public TD.ObservableItemCollection<TemplateAssociationModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new TD.ObservableItemCollection<TemplateAssociationModel>();
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
                    saveCommand = new RelayCommand( p => true, p => this.Save());
                return saveCommand;
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                    refreshCommand = new RelayCommand( p => true, p => this.Refresh());
                return refreshCommand;
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                    deleteCommand = new RelayCommand( p => true, p => this.Delete());
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
            FilteredTemplateAssociations = new CollectionViewSource { Source = TemplateAssociations };
            FilteredTemplateAssociations.Filter += TemplateFilter;
        }
        #endregion

        #region Methods

        private void Load(Guid? associationParent_ID)
        {
            // Check if the classes and properties have ben loaded. Necessary because of the various background workers

            try
            {
                using (EDBEntities eDB = new EDBEntities())
                {
                    foreach (tblTemplateAssociation Rec in (from o in eDB.tblTemplateAssociations where (o.Project_ID == Globals.Project_ID) select o))
                    {
                        TemplateAssociationModel templateAssociationItem = new TemplateAssociationModel
                        {
                            ID = Rec.ID,
                            Project_ID = Rec.Project_ID,
                            Template_ID = Rec.Template_ID,
                            Association_ID = Rec.Association_ID,
                            Value = Rec.Value,
                            AssociationType = Rec.AssociationType,
                            IsChanged = false,
                            IsNew = false,
                            IsDeleted = false,
                            ChildAssociations = new TD.ObservableItemCollection<TemplateAssociationModel>()
                        };
                        switch (templateAssociationItem.AssociationType)
                        {
                            case "Property":
                                var propertyItem = PropertyViewModelLocator.GetPropertyVM().GetProperty(templateAssociationItem.Association_ID, null);
                                if (propertyItem != null)
                                {
                                    templateAssociationItem.Name = propertyItem.PropertyName;
                                    templateAssociationItem.Description = propertyItem.Description;
                                    templateAssociationItem.AssociationType_ID = propertyItem.PropertyType_ID;
                                    foreach (var childItem in propertyItem.ChildProperties)
                                    {
                                        TemplateAssociationModel item = new TemplateAssociationModel
                                        {
                                            ID = Rec.ID,
                                            Project_ID = childItem.Project_ID,
                                            Template_ID = templateAssociationItem.Template_ID,
                                            Association_ID = childItem.ID,
                                            Name = childItem.PropertyName,
                                            Description = childItem.Description,
                                            AssociationType = "Property",
                                            AssociationType_ID = childItem.PropertyType_ID,
                                            ChildAssociations = new TD.ObservableItemCollection<TemplateAssociationModel>()
                                        };
                                        templateAssociationItem.ChildAssociations.Add(item);
                                    }
                                }
                                else
                                {
                                    throw new System.InvalidOperationException(String.Format("Association without source\nProperty ID: {0}\nFix in database", templateAssociationItem.Association_ID));
                                }
                                break;
                            case "TemplateProperty":
                                break;
                        }

                        TemplateAssociations.Add(templateAssociationItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate { RadWindow.Alert(ex.Message); });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Delete()
        {
            // ToDo: Deleting items in the collection only works using the Del key for now. 
            // Implement delete method to also provide option using context menu
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            EDBEntities eDB = new EDBEntities();

            // To determine which items have been deleted in the collection, get all associations of the project stored in the database table first
            var tblTemplateAssociations = eDB.tblTemplateAssociations.Where(p => p.Project_ID == Globals.Project_ID);

            // Check if each association of the table exists in the associations collection
            // if not, delete the association in the table
            foreach (var templateAssociationRec in tblTemplateAssociations)
            {
                var templateAssociationItem = GetTemplateAssociation(templateAssociationRec.Template_ID, templateAssociationRec.Association_ID);
                if (templateAssociationItem == null) // association not found in collection
                    eDB.tblTemplateAssociations.Remove(templateAssociationRec);
            }

            // Add and update associations recursively
            SaveLevel(TemplateAssociations, eDB);
            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Fault while saving template associations:\n" + ex.Message
                });
            }
            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(ObservableCollection<TemplateAssociationModel> treeLevel, EDBEntities eDB)
        {
            try
            {
                if (treeLevel != null)
                {
                    foreach (var templateAssociationItem in treeLevel)
                    {

                        if (templateAssociationItem.IsNew)
                        {
                            tblTemplateAssociation NewRec = new tblTemplateAssociation();
                            var Rec = eDB.tblTemplateAssociations.Add(NewRec);
                            Rec.ID = templateAssociationItem.ID;
                            Rec.Template_ID = templateAssociationItem.Template_ID;
                            Rec.Association_ID = templateAssociationItem.Association_ID;
                            Rec.Project_ID = Globals.Project_ID;
                            Rec.AssociationType = templateAssociationItem.AssociationType;
                            Rec.Value = templateAssociationItem.Value;
                            templateAssociationItem.IsNew = false;
                        }
                        if (templateAssociationItem.IsChanged)
                        {
                            tblTemplateAssociation Rec = eDB.tblTemplateAssociations.Where(o => o.ID == templateAssociationItem.ID).FirstOrDefault();
                            Rec.Value = templateAssociationItem.Value; 
                        }
                        // Recursive call
                        //if (propertyItem.ChildProperties != null) SaveLevel(propertyItem.ChildProperties, eDB);
                    }
                }
            }
            catch (Exception ex)
            {
                RadWindow.Alert("Fault while saving to database: " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            TemplateAssociations.Clear();
            Load(null);
            FilteredTemplateAssociations.View.Refresh();
        }

        public void AssociateWithTemplate(TreeListViewRow destination)
        {
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
                                    ID = Guid.NewGuid(),
                                    IsNew = true,
                                    IsChanged = false,
                                    IsDeleted = false,
                                    Project_ID = Globals.Project_ID,
                                    Template_ID = templateItem.ID,
                                    AssociationType = Globals.DraggedItem.Type,
                                    ChildAssociations = new TD.ObservableItemCollection<TemplateAssociationModel>(),
                                };
                                templateAssociationItem.Association_ID = propertyItem.ID;
                                templateAssociationItem.Name = propertyItem.PropertyName;
                                templateAssociationItem.Description = propertyItem.Description;
                                templateAssociationItem.AssociationType_ID = propertyItem.PropertyType_ID;
                                templateAssociationItem.Value = propertyItem.Value;

                                foreach (var childItem in propertyItem.ChildProperties)
                                {
                                    TemplateAssociationModel templateAssociationChildItem = new TemplateAssociationModel
                                    {
                                        IsNew = true,
                                        IsChanged = false,
                                        IsDeleted = false,
                                        AssociationType = "Property",
                                        ID = Guid.NewGuid(),
                                        Project_ID = Globals.Project_ID,
                                        Template_ID = templateItem.ID,
                                        Association_ID = childItem.ID,
                                        Name = childItem.PropertyName,
                                        Description = childItem.Description,
                                        AssociationType_ID = childItem.PropertyType_ID,
                                        Value = childItem.Value,
                                        ChildAssociations = new TD.ObservableItemCollection<TemplateAssociationModel>()
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

        private TemplateAssociationModel GetTemplateAssociation(Guid? template_ID, Guid? association_ID)
        {
            foreach (var templateAssociationItem in TemplateAssociations)
            {
                if (templateAssociationItem.Template_ID == template_ID && templateAssociationItem.Association_ID == association_ID) return templateAssociationItem;
            }
            return null;
        }

    }
    #endregion

}
