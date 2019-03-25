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
using Telerik.Windows.Data;
using TD = Telerik.Windows.Data;

namespace Sculptor
{
    public class ObjectAssociationViewModel : ViewModelBase
    {
        #region Constructor
        public ObjectAssociationViewModel()
        {
            Load(null);
            FilteredObjectAssociations = new CollectionViewSource { Source = ObjectAssociations };
            FilteredObjectAssociations.Filter += ObjectFilter;
        }

        #endregion

        #region Properties

        private TD.ObservableItemCollection<ObjectAssociationModel> objectAssociations = new TD.ObservableItemCollection<ObjectAssociationModel>();
        public TD.ObservableItemCollection<ObjectAssociationModel> ObjectAssociations
        {
            get { return objectAssociations; }
            set
            {
                objectAssociations = value;
                OnPropertyChanged();
            }
        }

        private CollectionViewSource filteredObjectAssociations;
        public CollectionViewSource FilteredObjectAssociations
        {
            get { return filteredObjectAssociations; }
            set
            {
                if (value != filteredObjectAssociations)
                {
                    filteredObjectAssociations = value;
                    OnPropertyChanged();
                }
            }
        }

        private TD.ObservableItemCollection<ObjectAssociationModel> selectedItems;
        public TD.ObservableItemCollection<ObjectAssociationModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new TD.ObservableItemCollection<ObjectAssociationModel>();
                }
                return selectedItems;
            }
        }

        private ObjectAssociationModel selectedItem;
        public ObjectAssociationModel SelectedItem
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
            Load(null);
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            // Create a collection that holds the Associations of the selected object and add the filter event handler
            // Note: the FilteredObjectAssociations collection is updated every time a new object is selected in the object tree 
            // (triggered in the setter of the SelectedItem property)
            FilteredObjectAssociations = new CollectionViewSource { Source = ObjectAssociations };
            FilteredObjectAssociations.Filter += ObjectFilter;
        }
        #endregion

        #region Methods

        private void Load(Guid? associationParent_ID)
        {
            // Check if the templates and properties have ben loaded. Necessary because of the various background workers
            //while (!TemplateViewModelLocator.IsLoaded() || !PropertyViewModelLocator.IsLoaded());
            try
            {
                using (EDBEntities eDB = new EDBEntities())
                {
                    foreach (tblObjectAssociation Rec in (from o in eDB.tblObjectAssociations where (o.Project_ID == Globals.Project_ID) orderby o.AssociationType select o))
                    {
                        ObjectAssociationModel objectAssociationItem = new ObjectAssociationModel
                        {
                            ID = Rec.ID,
                            Project_ID = Rec.Project_ID,
                            Object_ID = Rec.Object_ID,
                            Association_ID = Rec.Association_ID,
                            AssociationType = Rec.AssociationType,
                            IsChanged = false,
                            IsNew = false,
                            IsDeleted = false,
                            ChildAssociations = new TD.ObservableItemCollection<ObjectAssociationModel>()
                        };
                        switch (objectAssociationItem.AssociationType)
                        {
                            case "Template":
                                // Get detail info of the template
                                var templateItem = TemplateViewModelLocator.GetTemplateVM().GetTemplate(objectAssociationItem.Association_ID, null);
                                if (templateItem != null)
                                {
                                    objectAssociationItem.Name = templateItem.TemplateName;
                                    objectAssociationItem.Description = templateItem.Description;
                                    objectAssociationItem.AssociationType_ID = templateItem.TemplateType_ID;
                                    // and get any child items
                                    foreach (var childItem in templateItem.ChildTemplates)
                                    {
                                        ObjectAssociationModel item = new ObjectAssociationModel
                                        {
                                            ID = Rec.ID,
                                            Project_ID = childItem.Project_ID,
                                            Object_ID = objectAssociationItem.Object_ID,
                                            Association_ID = childItem.ID,
                                            Name = childItem.TemplateName,
                                            Description = childItem.Description,
                                            AssociationType = "Template",
                                            AssociationType_ID = childItem.TemplateType_ID, 
                                            ChildAssociations = new TD.ObservableItemCollection<ObjectAssociationModel>()
                                        };
                                        objectAssociationItem.ChildAssociations.Add(item);
                                        LoadTemplateProperties(item);
                                    }
                                    LoadTemplateProperties(objectAssociationItem);
                                }
                                else
                                {
                                    throw new System.InvalidOperationException(String.Format("Association without source\nTemplate ID: {0}\nFix in database", objectAssociationItem.Association_ID));
                                }

                                break;
                            case "Property":
                                var propertyItem = PropertyViewModelLocator.GetPropertyVM().GetProperty(objectAssociationItem.Association_ID, null);
                                if (propertyItem != null)
                                {
                                    objectAssociationItem.Name = propertyItem.PropertyName;
                                    objectAssociationItem.Description = propertyItem.Description;
                                    objectAssociationItem.AssociationType_ID = propertyItem.PropertyType_ID;

                                    // If the Object Association has a value in the table, use this as the associated value
                                    // otherwise use the value defined in the property
                                    if (!String.IsNullOrEmpty(Rec.Value))
                                        objectAssociationItem.Value = Rec.Value;
                                    else
                                        objectAssociationItem.Value = propertyItem.Value;

                                    foreach (var childItem in propertyItem.ChildProperties)
                                    {
                                        ObjectAssociationModel item = new ObjectAssociationModel
                                        {
                                            ID = Rec.ID,
                                            Project_ID = childItem.Project_ID,
                                            Object_ID = objectAssociationItem.Object_ID,
                                            Association_ID = childItem.ID,
                                            Name = childItem.PropertyName,
                                            Description = childItem.Description,
                                            AssociationType = "Property",
                                            AssociationType_ID = childItem.PropertyType_ID,
                                            ChildAssociations = new TD.ObservableItemCollection<ObjectAssociationModel>()
                                        };
                                        objectAssociationItem.ChildAssociations.Add(item);
                                    }
                                }
                                else
                                {
                                    throw new System.InvalidOperationException(String.Format("Association without source\nProperty ID: {0}\nFix in database", objectAssociationItem.Association_ID));
                                }
                                break;
                            case "TemplateProperty":
                                break;
                        }

                        ObjectAssociations.Add(objectAssociationItem);
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
            var tblObjectAssociations = eDB.tblObjectAssociations.Where(p => p.Project_ID == Globals.Project_ID);

            // Check if each association of the table exists in the associations collection
            // if not, delete the association in the table
            foreach (var objectAssociationRec in tblObjectAssociations)
            {
                var objectAssociationItem = GetObjectAssociation(objectAssociationRec.Object_ID, objectAssociationRec.Association_ID);
                if (objectAssociationItem == null) // association not found in collection
                    eDB.tblObjectAssociations.Remove(objectAssociationRec);
            }

            // Add and update associations recursively
            SaveLevel(ObjectAssociations, eDB);
            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Fault while saving object associations:\n" + ex.Message
                });
            }
            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(ObservableCollection<ObjectAssociationModel> treeLevel, EDBEntities eDB)
        {
            try
            {
                if (treeLevel != null)
                {
                    foreach (var objectAssociationItem in treeLevel)
                    {

                        if (objectAssociationItem.IsNew)
                        {
                            tblObjectAssociation NewRec = new tblObjectAssociation();
                            var Rec = eDB.tblObjectAssociations.Add(NewRec);
                            Rec.ID = objectAssociationItem.ID;
                            Rec.Object_ID = objectAssociationItem.Object_ID;
                            Rec.Association_ID = objectAssociationItem.Association_ID;
                            Rec.Project_ID = Globals.Project_ID;
                            Rec.AssociationType = objectAssociationItem.AssociationType;
                            Rec.Value = objectAssociationItem.Value;
                            objectAssociationItem.IsNew = false;
                        }
                        // Only save changes if the value has changed and the value = not empty or null 
                        if (objectAssociationItem.IsChanged && !string.IsNullOrEmpty(objectAssociationItem.Value))
                        {
                            tblObjectAssociation Rec = eDB.tblObjectAssociations.Where(o => o.Object_ID == objectAssociationItem.Object_ID && o.Association_ID == objectAssociationItem.Association_ID).FirstOrDefault();
                            // If the association is in the table, save the value
                            if (Rec != null)
                                Rec.Value = objectAssociationItem.Value;
                            // Otherwise add a record to the association table and save the value.
                            // Note: child associations normally are not saved to the association table but inherited from the templates or properties
                            else
                            {
                                tblObjectAssociation NewRec = new tblObjectAssociation();
                                Rec = eDB.tblObjectAssociations.Add(NewRec);
                                Rec.ID = objectAssociationItem.ID;
                                Rec.Object_ID = objectAssociationItem.Object_ID;
                                Rec.Association_ID = objectAssociationItem.Association_ID;
                                Rec.Project_ID = Globals.Project_ID;
                                Rec.AssociationType = objectAssociationItem.AssociationType;
                                Rec.Value = objectAssociationItem.Value;
                            }
                            objectAssociationItem.IsChanged = false;
                        }
                        // Recursive call
                        if (objectAssociationItem.ChildAssociations != null) SaveLevel(objectAssociationItem.ChildAssociations, eDB);
                    }
                }
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Fault while adding/updating to database:\n" + ex.Message
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            ObjectAssociations.Clear();
            Load(null);
            FilteredObjectAssociations.View.Refresh();
        }

        public void AssociateWithObject(TreeListViewRow destination)
        {
            try
            {
                foreach (var objectItem in ObjectViewModelLocator.GetObjectVM().SelectedItems)
                {
                    switch (Globals.DraggedItem.Type)
                    {
                        case "Template":
                            foreach (var templateItem in TemplateViewModelLocator.GetTemplateVM().SelectedItems)
                            {
                                ObjectAssociationModel objectAssociationItem = new ObjectAssociationModel
                                {
                                    IsNew = true,
                                    IsChanged = false,
                                    IsDeleted = false,
                                    ID = Guid.NewGuid(),
                                    Project_ID = Globals.Project_ID,
                                    Object_ID = objectItem.ID,
                                    AssociationType = Globals.DraggedItem.Type,
                                    Value = "",
                                    ChildAssociations = new TD.ObservableItemCollection<ObjectAssociationModel>(),
                                };
                                objectAssociationItem.Association_ID = templateItem.ID;
                                objectAssociationItem.Name = templateItem.TemplateName;
                                objectAssociationItem.Description = templateItem.Description;
                                objectAssociationItem.AssociationType_ID = templateItem.TemplateType_ID;

                                foreach (var childItem in templateItem.ChildTemplates)
                                {
                                    ObjectAssociationModel objectAssociationChildItem = new ObjectAssociationModel
                                    {
                                        IsNew = true,
                                        IsChanged = false,
                                        IsDeleted = false,
                                        ID = Guid.NewGuid(),
                                        AssociationType = "Template",
                                        Project_ID = Globals.Project_ID,
                                        Object_ID = objectItem.ID,
                                        Association_ID = childItem.ID,
                                        Name = childItem.TemplateName,
                                        Description = childItem.Description,
                                        AssociationType_ID = childItem.TemplateType_ID,
                                        Value = "",
                                        ChildAssociations = new TD.ObservableItemCollection<ObjectAssociationModel>()
                                    };
                                    objectAssociationItem.ChildAssociations.Add(objectAssociationChildItem);
                                    LoadTemplateProperties(objectAssociationChildItem);
                                };
                                ObjectAssociations.Add(objectAssociationItem);
                                LoadTemplateProperties(objectAssociationItem);
                            }
                            break;
                        case "Property":
                            foreach (var propertyItem in PropertyViewModelLocator.GetPropertyVM().SelectedItems)
                            {
                                ObjectAssociationModel objectAssociationItem = new ObjectAssociationModel
                                {
                                    IsNew = true,
                                    IsChanged = false,
                                    IsDeleted = false,
                                    ID = Guid.NewGuid(),
                                    Project_ID = Globals.Project_ID,
                                    Object_ID = objectItem.ID,
                                    AssociationType = Globals.DraggedItem.Type,
                                    Value = "",
                                    ChildAssociations = new TD.ObservableItemCollection<ObjectAssociationModel>(),
                                };

                                objectAssociationItem.Association_ID = propertyItem.ID;
                                objectAssociationItem.Name = propertyItem.PropertyName;
                                objectAssociationItem.Description = propertyItem.Description;
                                objectAssociationItem.AssociationType_ID = propertyItem.PropertyType_ID;

                                foreach (var childItem in propertyItem.ChildProperties)
                                {
                                    ObjectAssociationModel objectAssociationChildItem = new ObjectAssociationModel
                                    {
                                        IsNew = true,
                                        IsChanged = false,
                                        IsDeleted = false,
                                        AssociationType = "Property",
                                        Project_ID = Globals.Project_ID,
                                        Object_ID = objectItem.ID,
                                        Association_ID = childItem.ID,
                                        Name = childItem.PropertyName,
                                        Description = childItem.Description,
                                        AssociationType_ID = childItem.PropertyType_ID,
                                        Value = "",
                                        ChildAssociations = new TD.ObservableItemCollection<ObjectAssociationModel>()
                                    };
                                    objectAssociationItem.ChildAssociations.Add(objectAssociationChildItem);
                                }
                                ObjectAssociations.Add(objectAssociationItem);
                            }
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                RadWindow.Alert(ex.Message);
            }
        }

        public void ObjectFilter(object sender, FilterEventArgs e)
        {
            ObjectViewModel ovm = ObjectViewModelLocator.GetObjectVM();
            ObjectModel om = ovm.SelectedItem;
            if (e.Item != null && om != null)
                e.Accepted = (e.Item as ObjectAssociationModel).Object_ID == ObjectViewModelLocator.GetObjectVM().SelectedItem.ID;
        }

        private ObjectAssociationModel GetObjectAssociation(Guid? object_ID, Guid? association_ID)
        {
            foreach (var objectAssociationItem in ObjectAssociations)
            {
                if (objectAssociationItem.Object_ID == object_ID && objectAssociationItem.Association_ID == association_ID) return objectAssociationItem;
            }
            return null;
        }

        private void LoadTemplateProperties(ObjectAssociationModel templateItem)
        {

            //var propertyItem = PropertyViewModelLocator.GetPropertyVM().GetProperty(objectAssociationItem.Association_ID, null);
            //if (templateItem != null)
            //{
                //objectAssociationItem.Name = templateItem.TemplateName;
                //objectAssociationItem.Description = templateItem.Description;
                //objectAssociationItem.AssociationType_ID = templateItem.TemplateType_ID;
                // and get any child items
                foreach (var itemAssociation in TemplateAssociationViewModelLocator.GetTemplateAssociationVM().TemplateAssociations)
                {
                    if (itemAssociation.Template_ID == templateItem.Association_ID)
                    {
                        var propertyItem = PropertyViewModelLocator.GetPropertyVM().GetProperty(itemAssociation.Association_ID);
                        if (propertyItem != null)
                        {
                            ObjectAssociationModel item = new ObjectAssociationModel
                            {
                                ID = itemAssociation.ID,
                                Project_ID = templateItem.Project_ID,
                                Object_ID = templateItem.Object_ID,
                                Association_ID = itemAssociation.Association_ID,
                                Name = propertyItem.PropertyName,
                                Description = propertyItem.Description,
                                AssociationType = "TemplateProperty",
                                AssociationType_ID = itemAssociation.AssociationType_ID,
                                IsChanged = false,
                                IsNew = false,
                                ChildAssociations = new TD.ObservableItemCollection<ObjectAssociationModel>()
                            };
                            templateItem.ChildAssociations.Add(item);
                        }
                    }
                }
            //}
            //else
            //{
            //    throw new System.InvalidOperationException(String.Format("Association without source\nTemplate ID: {0}\nFix in database", objectAssociationItem.Association_ID));
            //}
        }
    }
    #endregion

}
