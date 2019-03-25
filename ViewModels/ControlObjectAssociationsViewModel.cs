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
    public class ControlObjectAssociationViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Constructor
        public ControlObjectAssociationViewModel()
        {
            Load(null);
            FilteredControlObjectAssociations = new CollectionViewSource { Source = ControlObjectAssociations };
            FilteredControlObjectAssociations.Filter += ControlObjectFilter;
        }

        #endregion

        #region Properties

        private TD.ObservableItemCollection<ControlObjectAssociationModel> controlObjectAssociations = new TD.ObservableItemCollection<ControlObjectAssociationModel>();
        public TD.ObservableItemCollection<ControlObjectAssociationModel> ControlObjectAssociations
        {
            get { return controlObjectAssociations; }
            set
            {
                controlObjectAssociations = value;
                OnPropertyChanged();
            }
        }

        private CollectionViewSource filteredControlObjectAssociations;
        public CollectionViewSource FilteredControlObjectAssociations
        {
            get { return filteredControlObjectAssociations; }
            set
            {
                if (value != filteredControlObjectAssociations)
                {
                    filteredControlObjectAssociations = value;
                    OnPropertyChanged();
                }
            }
        }

        private TD.ObservableItemCollection<ControlObjectAssociationModel> selectedItems;
        public TD.ObservableItemCollection<ControlObjectAssociationModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new TD.ObservableItemCollection<ControlObjectAssociationModel>();
                }
                return selectedItems;
            }
        }

        private ControlObjectAssociationModel selectedItem;
        public ControlObjectAssociationModel SelectedItem
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
                    foreach (tblControlObjectAssociation Rec in (from o in eDB.tblControlObjectAssociations where (o.Project_ID == Globals.Project_ID) select o))
                    {
                        ControlObjectAssociationModel controlObjectAssociationItem = new ControlObjectAssociationModel
                        {
                            ID = Rec.ID,
                            Project_ID = Rec.Project_ID,
                            ControlObject_ID = Rec.ControlObject_ID,
                            Association_ID = Rec.Association_ID,
                            AssociationType = Rec.AssociationType,
                            IsChanged = false,
                            IsNew = false,
                            IsDeleted = false,

                        };
                        switch (controlObjectAssociationItem.AssociationType)
                        {
                            case "Template":

                                break;
                            case "Property":
                                var propertyItem = PropertyViewModelLocator.GetPropertyVM().GetProperty(controlObjectAssociationItem.Association_ID, null);
                                if (propertyItem != null)
                                {
                                    //controlObjectAssociationItem.Name = propertyItem.PropertyName;
                                    //controlObjectAssociationItem.Description = propertyItem.Description;
                                    //controlObjectAssociationItem.AssociationType = propertyItem.PropertyType_ID;

                                    // If the Object Association has a value in the table, use this as the associated value
                                    // otherwise use the value defined in the property
                                    //if (!String.IsNullOrEmpty(Rec.Value))
                                    //    controlObjectAssociationItem.Value = Rec.Value;
                                    //else
                                    //    controlObjectAssociationItem.Value = propertyItem.Value;

                                    //foreach (var childItem in propertyItem.ChildProperties)
                                    //{
                                    //    ControlObjectAssociationModel item = new ControlObjectAssociationModel
                                    //    {
                                    //        ID = Rec.ID,
                                    //        Project_ID = childItem.Project_ID,
                                    //        ControlObject_ID = controlObjectAssociationItem.ControlObject_ID,
                                    //        Association_ID = childItem.ID,
                                    //        Name = childItem.PropertyName,
                                    //        Description = childItem.Description,
                                    //        AssociationType = "Property",
                                    //        AssociationType_ID = childItem.PropertyType_ID,
                                    //    };
                                    //}
                                }
                                else
                                {
                                    throw new System.InvalidOperationException(String.Format("Association without source\nProperty ID: {0}\nFix in database", controlObjectAssociationItem.Association_ID));
                                }
                                break;
                            case "TemplateProperty":
                                break;
                        }

                        ControlObjectAssociations.Add(controlObjectAssociationItem);
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
            var tblControlObjectAssociations = eDB.tblControlObjectAssociations.Where(p => p.Project_ID == Globals.Project_ID);

            // Check if each association of the table exists in the associations collection
            // if not, delete the association in the table
            foreach (var controlObjectAssociationRec in tblControlObjectAssociations)
            {
                var controlObjectAssociationItem = GetObjectAssociation(controlObjectAssociationRec.ControlObject_ID, controlObjectAssociationRec.Association_ID);
                if (controlObjectAssociationItem == null) // association not found in collection
                    eDB.tblControlObjectAssociations.Remove(controlObjectAssociationRec);
            }

            // Add and update associations recursively
            SaveLevel(ControlObjectAssociations, eDB);
            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Fault while saving control object associations:\n" + ex.Message
                });
            }
            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(ObservableCollection<ControlObjectAssociationModel> treeLevel, EDBEntities eDB)
        {
            try
            {
                if (treeLevel != null)
                {
                    foreach (var controlObjectAssociationItem in treeLevel)
                    {

                        if (controlObjectAssociationItem.IsNew)
                        {
                            tblControlObjectAssociation NewRec = new tblControlObjectAssociation();
                            var Rec = eDB.tblControlObjectAssociations.Add(NewRec);
                            Rec.ID = controlObjectAssociationItem.ID;
                            Rec.ControlObject_ID = controlObjectAssociationItem.ControlObject_ID;
                            Rec.Association_ID = controlObjectAssociationItem.Association_ID;
                            Rec.Project_ID = Globals.Project_ID;
                            Rec.AssociationType = controlObjectAssociationItem.AssociationType;
                            //Rec.Value = controlObjectAssociationItem.Value;
                            controlObjectAssociationItem.IsNew = false;
                        }
                        // Only save changes if the value has changed and the value = not empty or null 
                        if (controlObjectAssociationItem.IsChanged && !string.IsNullOrEmpty(controlObjectAssociationItem.Value))
                        {
                            tblControlObjectAssociation Rec = eDB.tblControlObjectAssociations.Where(o => o.ControlObject_ID == controlObjectAssociationItem.ControlObject_ID && o.Association_ID == controlObjectAssociationItem.Association_ID).FirstOrDefault();
                            // If the association is in the table, save the value
                            if (Rec != null) ;
                            //Rec.Value = controlObjectAssociationItem.Value;
                            // Otherwise add a record to the association table and save the value.
                            // Note: child associations normally are not saved to the association table but inherited from the templates or properties
                            else
                            {
                                tblControlObjectAssociation NewRec = new tblControlObjectAssociation();
                                Rec = eDB.tblControlObjectAssociations.Add(NewRec);
                                Rec.ID = controlObjectAssociationItem.ID;
                                Rec.ControlObject_ID = controlObjectAssociationItem.ControlObject_ID;
                                Rec.Association_ID = controlObjectAssociationItem.Association_ID;
                                Rec.Project_ID = Globals.Project_ID;
                                Rec.AssociationType = controlObjectAssociationItem.AssociationType;
                                //Rec.Value = controlObjectAssociationItem.Value;
                            }
                            controlObjectAssociationItem.IsChanged = false;
                        }
                        // Recursive call
                        //if (controlObjectAssociationItem.ChildAssociations != null) SaveLevel(objectAssociationItem.ChildAssociations, eDB);
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
            ControlObjectAssociations.Clear();
            Load(null);
            FilteredControlObjectAssociations.View.Refresh();
        }

        public void AssociateWithControlObject(TreeListViewRow destination)
        {
            try
            {
                foreach (var controlObjectItem in ControlObjectViewModelLocator.GetControlObjectVM().SelectedItems)
                {
                    switch (Globals.DraggedItem.Type)
                    {
                        case "ControlProperty":
                            break;
                        case "HardIO":
                            foreach (var hardIOItem in HardIOViewModelLocator.GetHardIOVM().SelectedItems)
                            {
                                ControlObjectAssociationModel controlObjectAssociationItem = new ControlObjectAssociationModel
                                {
                                    IsNew = true,
                                    IsChanged = false,
                                    IsDeleted = false,
                                    ID = Guid.NewGuid(),
                                    Project_ID = Globals.Project_ID,
                                    ControlObject_ID = controlObjectItem.ID,
                                    //ControlProperty_ID = Globals.DraggedItem.HardIOModelSource.ID,
                                    AssociationType = Globals.DraggedItem.Type,
                                    Value = "",
                                };

                                controlObjectAssociationItem.Name = hardIOItem.PropertyName;
                                controlObjectAssociationItem.Description = hardIOItem.Description;

                                //foreach (var childItem in hardIOItem.ChildProperties)
                                //{
                                //    ControlObjectAssociationModel controlObjectAssociationChildItem = new ControlObjectAssociationModel
                                //    {
                                //        IsNew = true,
                                //        IsChanged = false,
                                //        IsDeleted = false,
                                //        AssociationType = "Property",
                                //        Project_ID = Globals.Project_ID,
                                //        ControlObject_ID = controlObjectItem.ID,
                                //        Association_ID = childItem.ID,
                                //        Name = childItem.PropertyName,
                                //        Description = childItem.Description,
                                //        Value = "",
                                //    };
                                //}
                                ControlObjectAssociations.Add(controlObjectAssociationItem);
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

        public void ControlObjectFilter(object sender, FilterEventArgs e)
        {
            ControlObjectViewModel ovm = ControlObjectViewModelLocator.GetControlObjectVM();
            ControlObjectModel om = ovm.SelectedItem;
            if (e.Item != null && om != null)
                e.Accepted = (e.Item as ControlObjectAssociationModel).ControlObject_ID == ControlObjectViewModelLocator.GetControlObjectVM().SelectedItem.ID;
        }

        private ControlObjectAssociationModel GetObjectAssociation(Guid? object_ID, Guid? association_ID)
        {
            foreach (var controlObjectAssociationItem in ControlObjectAssociations)
            {
                if (controlObjectAssociationItem.ControlObject_ID == object_ID && controlObjectAssociationItem.Association_ID == association_ID) return controlObjectAssociationItem;
            }
            return null;
        }

        private void LoadTemplateProperties(ControlObjectAssociationModel templateItem)
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
                                Object_ID = templateItem.ControlObject_ID,
                                Association_ID = itemAssociation.Association_ID,
                                Name = propertyItem.PropertyName,
                                Description = propertyItem.Description,
                                AssociationType = "TemplateProperty",
                                AssociationType_ID = itemAssociation.AssociationType_ID,
                                IsChanged = false,
                                IsNew = false,
                                ChildAssociations = new TD.ObservableItemCollection<ObjectAssociationModel>()
                            };
                            //templateItem.ChildAssociations.Add(item);
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
