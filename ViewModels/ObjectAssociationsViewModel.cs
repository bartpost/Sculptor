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
    public class ObjectAssociationViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ObservableCollectionWithItemChanged<ObjectAssociationModel> objectAssociations = new ObservableCollectionWithItemChanged<ObjectAssociationModel>();
        private ObservableCollectionWithItemChanged<ObjectAssociationModel> backgroundObjectAssociations = new ObservableCollectionWithItemChanged<ObjectAssociationModel>();
        private ObjectAssociationModel selectedItem;
        private ObservableCollectionWithItemChanged<ObjectAssociationModel> selectedItems;
        private CollectionViewSource filteredObjectAssociations;
        private bool isChanged;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand deleteCommand;

        #region Constructor
        public ObjectAssociationViewModel()
        {
            //Load the properties in the background
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        #endregion

        #region Properties

        public ObservableCollectionWithItemChanged<ObjectAssociationModel> ObjectAssociations
        {
            get
            {
                return objectAssociations;
            }
            set
            {
                objectAssociations = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionWithItemChanged<ObjectAssociationModel> BackgroundObjectAssociations
        {
            get
            {
                return backgroundObjectAssociations;
            }
            set
            {
                backgroundObjectAssociations = value;
                OnPropertyChanged();
            }
        }

        public CollectionViewSource FilteredObjectAssociations
        {
            get
            {
                return filteredObjectAssociations;
            }
            set
            {
                if (value != filteredObjectAssociations)
                {
                    filteredObjectAssociations = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollectionWithItemChanged<ObjectAssociationModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<ObjectAssociationModel>();
                }
                return selectedItems;
            }
        }

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

            ObjectAssociations = BackgroundObjectAssociations;

            // Create a collection that holds the Associations of the selected object and add the filter event handler
            // Note: the FilteredObjectAssociations collection is updated every time a new object is selected in the object tree 
            // (triggered in the setter of the SelectedItem property)
            FilteredObjectAssociations = new CollectionViewSource
            {
                Source = ObjectAssociations
            };
            FilteredObjectAssociations.Filter += ObjectFilter;
        }
        #endregion

        #region Methods

        private void Load(Guid? associationParent_ID)
        {
            // Check if the templates and properties have ben loaded. Necessary because of the various background workers
            while (!TemplateViewModelLocator.IsLoaded() || !PropertyViewModelLocator.IsLoaded());
            try
            {
                using (EDBEntities eDB = new EDBEntities())
                {
                    foreach (tblObjectAssociation Rec in (from o in eDB.tblObjectAssociations where (o.Project_ID == Globals.Project_ID) orderby o.AssociationType select o))
                    {
                        ObjectAssociationModel objectAssociationItem = new ObjectAssociationModel
                        {
                            Project_ID = Rec.Project_ID,
                            Object_ID = Rec.Object_ID,
                            Association_ID = Rec.Association_ID,
                            Value = Rec.Value,
                            AssociationType = Rec.AssociationType,
                            IsChanged = false,
                            IsNew = false,
                            IsDeleted = false,
                            ChildAssociations = new ObservableCollectionWithItemChanged<ObjectAssociationModel>()
                        };
                        switch (objectAssociationItem.AssociationType)
                        {
                            case "Template":
                                var templateItem = TemplateViewModelLocator.GetTemplateVM().GetTemplate(objectAssociationItem.Association_ID, null);
                                if (templateItem != null)
                                {
                                    objectAssociationItem.Name = templateItem.TemplateName;
                                    objectAssociationItem.Description = templateItem.Description;
                                    foreach (var childItem in templateItem.ChildTemplates)
                                    {
                                        ObjectAssociationModel item = new ObjectAssociationModel
                                        {
                                            Project_ID = childItem.Project_ID,
                                            Object_ID = objectAssociationItem.Object_ID,
                                            Association_ID = childItem.ID,
                                            Name = childItem.TemplateName,
                                            Description = childItem.Description,
                                            AssociationType = "Template",
                                            ChildAssociations = new ObservableCollectionWithItemChanged<ObjectAssociationModel>()
                                        };
                                        objectAssociationItem.ChildAssociations.Add(item);
                                    }
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
                                    foreach (var childItem in propertyItem.ChildProperties)
                                    {
                                        ObjectAssociationModel item = new ObjectAssociationModel
                                        {
                                            Project_ID = childItem.Project_ID,
                                            Object_ID = objectAssociationItem.Object_ID,
                                            Association_ID = childItem.ID,
                                            Name = childItem.PropertyName,
                                            Description = childItem.Description,
                                            AssociationType = "Property",
                                            ChildAssociations = new ObservableCollectionWithItemChanged<ObjectAssociationModel>()
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

                        BackgroundObjectAssociations.Add(objectAssociationItem);
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

                // Because the FilteredObjectAssociations collection doesn't refresh on PropertyChanged, we have to refresh the collection
                FilteredObjectAssociations.View.Refresh();
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
                SaveLevel(ObjectAssociations, eDB);
                eDB.SaveChanges();
            }
            catch (Exception ex)
            { 
                RadWindow.Alert("Fault while saving object associations: " + ex.Message);
            }
            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(ObservableCollection<ObjectAssociationModel> treeLevel, EDBEntities eDB)
        {
            if (treeLevel != null)
            {
                foreach (var objectAssociationItem in treeLevel)
                {

                    if (objectAssociationItem.IsNew)
                    {
                        tblObjectAssociation NewRec = new tblObjectAssociation();
                        var Rec = eDB.tblObjectAssociations.Add(NewRec);
                        Rec.Object_ID = objectAssociationItem.Object_ID;
                        Rec.Association_ID = objectAssociationItem.Association_ID;
                        Rec.Project_ID = Globals.Project_ID;
                        Rec.AssociationType = objectAssociationItem.AssociationType;
                        objectAssociationItem.IsNew = false;
                        switch (objectAssociationItem.AssociationType)
                        {
                            case "Template":
                                TemplateViewModel classVM = TemplateViewModelLocator.GetTemplateVM();
                                TemplateModel classItem = classVM.GetTemplate(objectAssociationItem.Association_ID);

                                break;
                        }
                    }
                    if (objectAssociationItem.IsChanged)
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
                    if (objectAssociationItem.IsDeleted)
                    {
                        tblObjectAssociation Rec = eDB.tblObjectAssociations.Where(o => (o.Object_ID == objectAssociationItem.Object_ID && o.Association_ID == objectAssociationItem.Association_ID)).FirstOrDefault();
                        if (Rec != null)
                            eDB.tblObjectAssociations.Remove(Rec);
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
            ObjectAssociations.Clear();
            FilteredObjectAssociations.View.Refresh();
            Load(null);
        }

        public void AssociateWithObject(TreeListViewRow destination)
        {
            //if (destination != null)
            //{
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
                                    Project_ID = Globals.Project_ID,
                                    Object_ID = objectItem.ID,
                                    AssociationType = Globals.DraggedItem.Type,
                                    Value = "",
                                    ChildAssociations = new ObservableCollectionWithItemChanged<ObjectAssociationModel>(),
                                };
                                objectAssociationItem.Association_ID = templateItem.ID;
                                objectAssociationItem.Name = templateItem.TemplateName;
                                objectAssociationItem.Description = templateItem.Description;

                                foreach (var childItem in templateItem.ChildTemplates)
                                {
                                    ObjectAssociationModel objectAssociationChildItem = new ObjectAssociationModel
                                    {
                                        IsNew = true,
                                        IsChanged = false,
                                        IsDeleted = false,
                                        AssociationType = "Template",
                                        Project_ID = Globals.Project_ID,
                                        Object_ID = objectItem.ID,
                                        Association_ID = childItem.ID,
                                        Name = childItem.TemplateName,
                                        Description = childItem.Description,
                                        Value = "",
                                        ChildAssociations = new ObservableCollectionWithItemChanged<ObjectAssociationModel>()
                                    };
                                    objectAssociationItem.ChildAssociations.Add(objectAssociationChildItem);
                                };
                                ObjectAssociations.Add(objectAssociationItem);
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
                                    Project_ID = Globals.Project_ID,
                                    Object_ID = objectItem.ID,
                                    AssociationType = Globals.DraggedItem.Type,
                                    Value = "",
                                    ChildAssociations = new ObservableCollectionWithItemChanged<ObjectAssociationModel>(),
                                };

                                objectAssociationItem.Association_ID = propertyItem.ID;
                                objectAssociationItem.Name = propertyItem.PropertyName;
                                objectAssociationItem.Description = propertyItem.Description;

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
                                        Value = "",
                                        ChildAssociations = new ObservableCollectionWithItemChanged<ObjectAssociationModel>()
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
            //}
        }

        public void ObjectFilter(object sender, FilterEventArgs e)
        {
            ObjectViewModel ovm = ObjectViewModelLocator.GetObjectVM();
            ObjectModel om = ovm.SelectedItem;
            if (e.Item != null && om != null)
                e.Accepted = (e.Item as ObjectAssociationModel).Object_ID == ObjectViewModelLocator.GetObjectVM().SelectedItem.ID &&
                             (e.Item as ObjectAssociationModel).IsDeleted == false;

        }
    }
    #endregion

}
