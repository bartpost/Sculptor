using Sculptor.EDBEntityDataModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;

namespace Sculptor
{
    public class PropertyViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ObservableCollectionWithItemChanged<PropertyModel> properties; // = new ObservableCollectionWithItemChanged<PropertyModel>();
        private ObservableCollectionWithItemChanged<PropertyModel> backgroundProperties = new ObservableCollectionWithItemChanged<PropertyModel>();
        private ObservableCollection<PropertyTypeModel> propertyTypes = new ObservableCollection<PropertyTypeModel>();
        private PropertyModel selectedItem;
        private ObservableCollectionWithItemChanged<PropertyModel> selectedItems;
        private bool isChanged;
        private bool isPropertyTypePopupOpen;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand addSiblingCommand;
        private ICommand addChildCommand;
        private ICommand deleteCommand;
        private ICommand changePropertyTypeCommand;
        private ICommand selectPropertyTypeCommand;


        #region Constructor
        public PropertyViewModel()
        {
            // Load the properties in the background
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            backgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Properties

        public ObservableCollectionWithItemChanged<PropertyModel> Properties
        {
            get
            {
                return properties;
            }
            set
            {
                properties = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionWithItemChanged<PropertyModel> BackgroundProperties
        {
            get
            {
                return backgroundProperties;
            }
            set
            {
                backgroundProperties = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionWithItemChanged<PropertyModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<PropertyModel>();
                }
                return selectedItems;
            }
        }

        public ObservableCollection<PropertyTypeModel> PropertyTypes
        {
            get
            {
                if (propertyTypes == null)
                {
                    propertyTypes = new ObservableCollection<PropertyTypeModel>();
                }
                return propertyTypes;
            }

        }

        public PropertyModel SelectedItem
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

        public bool IsPropertyTypePopupOpen
        {
            get
            {
                return this.isPropertyTypePopupOpen;
            }
            set
            {
                if (value != this.isPropertyTypePopupOpen)
                {
                    this.isPropertyTypePopupOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public AspectViewModel Aspects = new AspectViewModel();
        #endregion

        #region Commands

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

        public ICommand AddSiblingCommand
        {
            get
            {
                if (addSiblingCommand == null)
                {
                    addSiblingCommand = new RelayCommand(
                        p => this.CanAddSibling(),
                        p => this.AddSibling());
                }
                return addSiblingCommand;
            }
        }

        public ICommand AddChildCommand
        {
            get
            {
                if (addChildCommand == null)
                {
                    addChildCommand = new RelayCommand(
                        p => this.CanAddChild(),
                        p => this.AddChild());
                }
                return addChildCommand;
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

        public ICommand ChangePropertyTypeCommand
        {
            get
            {
                if (changePropertyTypeCommand == null)
                {
                    changePropertyTypeCommand = new RelayCommand(
                        p => this.CanChangePropertyType(),
                        p => this.ChangePropertyType(p));
                }
                return changePropertyTypeCommand;
            }
        }

        public ICommand SelectPropertyTypeCommand
        {
            get
            {
                if (selectPropertyTypeCommand == null)
                {
                    selectPropertyTypeCommand = new RelayCommand(
                        p => this.CanSelectPropertyType(),
                        p => this.SelectPropertyType(p));
                }
                return selectPropertyTypeCommand;
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
            LoadPropertyTypes();
            Load(null);
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            Properties = BackgroundProperties;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads the records from the DbSet into the ViewModel. This function designed for recursive use
        /// </summary>
        /// <param name="Project_ID"></param>
        /// <param name="Parent_ID"></param>
        /// <returns>Observable collection of VMObjects</returns>
        private ObservableCollectionWithItemChanged<PropertyModel> Load(Guid? Parent_ID)
        {

            ObservableCollectionWithItemChanged<PropertyModel> childProperties = new ObservableCollectionWithItemChanged<PropertyModel>();

            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblProperty Rec in (from o in eDB.tblProperties where (o.Project_ID == Globals.Project_ID && o.Parent_ID == Parent_ID) orderby o.PropertyName select o))
                {

                    PropertyModel propertyItem = new PropertyModel
                    {
                        ID = Rec.ID,
                        Parent_ID = Rec.Parent_ID,
                        Project_ID = Rec.Project_ID,
                        PropertyName = Rec.PropertyName,
                        Description = Rec.Description,
                        PropertyType_ID = (int)Rec.PropertyType_ID,
                        Aspect = Rec.Aspect,
                        Attribute1 = Rec.Attribute1,
                        Attribute2 = Rec.Attribute2,
                        Attribute3 = Rec.Attribute3,
                        IsChanged = false,
                        IsNew = false,
                        IsDeleted = false
                    };

                    propertyItem.ChildProperties = Load(Rec.ID);

                    // If the parent ID is null, this is a root object and needs to be added to the VM class
                    // Else it is a child object which needs to be added to the childobjectlist
                    if (Rec.Parent_ID == null)
                        BackgroundProperties.Add(propertyItem);
                    else
                        childProperties.Add(propertyItem);

                }
            }
            return childProperties;
        }

        private void LoadPropertyTypes()
        {
            PropertyTypes.Clear();
            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblPropertyType Rec in (from o in eDB.tblPropertyTypes where o.Project_ID == Globals.Project_ID select o))
                {
                    PropertyTypeModel propertyTypeItem = new PropertyTypeModel
                    {
                        ID = Rec.ID,
                        PropertyType = Rec.PropertyType,
                        Description = Rec.Description,
                        Image = Rec.Image
                    };
                    PropertyTypes.Add(propertyTypeItem);
                }
            }
        }

        private bool CanAddSibling()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddSibling()
        {
            PropertyModel propertyItem = new PropertyModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                PropertyName = "New Property",
                Description = "New Property Description",
                PropertyType_ID = 5,
                IsChanged = false,
                IsNew = true,
                IsDeleted = false,
                ChildProperties = new ObservableCollectionWithItemChanged<PropertyModel>()
            };

            if (SelectedItem == null)
            {
                propertyItem.Parent_ID = null;
                Properties.Add(propertyItem);
            }
            else if (SelectedItem.Parent_ID == null)
            {
                propertyItem.Parent_ID = null;
                Properties.Insert(Properties.IndexOf(SelectedItem) + 1, propertyItem);
            }
            else
            {
                PropertyModel parentItem = GetProperty(SelectedItem.Parent_ID);
                propertyItem.Parent_ID = SelectedItem.Parent_ID;
                parentItem.ChildProperties.Insert(parentItem.ChildProperties.IndexOf(SelectedItem) + 1, propertyItem);
            }
        }

        private bool CanAddChild()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddChild()
        {
            PropertyModel propertyItem = new PropertyModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                PropertyName = "New Property",
                Description = "New Property Description",
                PropertyType_ID = 5,
                IsChanged = false,
                IsNew = true,
                IsDeleted = false,
                ChildProperties = new ObservableCollectionWithItemChanged<PropertyModel>()
            };
            if (SelectedItem != null)
            {
                propertyItem.Parent_ID = SelectedItem.ID;
                SelectedItem.ChildProperties.Add(propertyItem);
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
            SelectedItem.IsChanged = false;
            SelectedItem.IsNew = false;
            SelectedItem.IsDeleted = true;
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
            EDBEntities eDB = new EDBEntities();

            SaveLevel(Properties, eDB);
            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert("Fault while saving properties: " + ex.Message);
            }
            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(ObservableCollection<PropertyModel> treeLevel, EDBEntities eDB)
        {
            if (treeLevel != null)
            {
                foreach (var propertyItem in treeLevel)
                {

                    if (propertyItem.IsNew)
                    {
                        tblProperty NewRec = new tblProperty();
                        var Rec = eDB.tblProperties.Add(NewRec);
                        Rec.ID = propertyItem.ID;
                        Rec.Parent_ID = propertyItem.Parent_ID;
                        Rec.PropertyName = propertyItem.PropertyName;
                        Rec.Description = propertyItem.Description;
                        Rec.Project_ID = propertyItem.Project_ID;
                        Rec.PropertyType_ID = propertyItem.PropertyType_ID;
                        Rec.Aspect = propertyItem.Aspect;
                        Rec.Attribute1 = propertyItem.Attribute1;
                        Rec.Attribute2 = propertyItem.Attribute2;
                        Rec.Attribute3 = propertyItem.Attribute3;
                        propertyItem.IsNew = false;
                    }
                    if (propertyItem.IsChanged)
                    {
                        tblProperty Rec = eDB.tblProperties.Where(o => o.ID == propertyItem.ID).FirstOrDefault();
                        Rec.Parent_ID = propertyItem.Parent_ID;
                        Rec.PropertyName = propertyItem.PropertyName;
                        Rec.Description = propertyItem.Description;
                        Rec.Project_ID = propertyItem.Project_ID;
                        Rec.PropertyType_ID = propertyItem.PropertyType_ID;
                        Rec.Aspect = propertyItem.Aspect;
                        Rec.Attribute1 = propertyItem.Attribute1;
                        Rec.Attribute2 = propertyItem.Attribute2;
                        Rec.Attribute3 = propertyItem.Attribute3;
                        propertyItem.IsChanged = false;
                    }
                    if (propertyItem.IsDeleted)
                    {
                        tblProperty Rec = eDB.tblProperties.Where(o => o.ID == propertyItem.ID).FirstOrDefault();
                        if (Rec != null)
                            eDB.tblProperties.Remove(Rec);
                    }
                    // Recursive call
                    if (propertyItem.ChildProperties != null) SaveLevel(propertyItem.ChildProperties, eDB);
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
            Properties.Clear();
            Load(null);
        }

        /// <summary>
        /// Method triggered by the TreeListViewDragDropBehavior Class. Takes care of moving on item in the tree, which can be from 
        /// any level to any level
        /// </summary>
        /// <param name="destination"></param>
        public void MoveSelection(TreeListViewRow destination) // Collection<PropertyModel> selectedItems, PropertyModel destination )
        {
            PropertyModel destinationItem = (destination.DataContext) as PropertyModel;
            try
            {
                // Setup a private collection with the selected items only. This is because the SelectedItems that are part of the view model collection
                // will change as soon as we start removing and adding objects
                ObservableCollectionWithItemChanged<PropertyModel> selectedItems = new ObservableCollectionWithItemChanged<PropertyModel>();
                foreach (PropertyModel item in SelectedItems)
                {
                    selectedItems.Add(item);
                }

                foreach (PropertyModel item in selectedItems)
                {
                    // find the original parent of the object that's moved
                    PropertyModel parentSourceItem = GetProperty(item.Parent_ID);

                    // If the parent is in the root level
                    if (parentSourceItem == null)
                        // Remove the item in the root level
                        Properties.Remove(item);
                    else
                        // Otherwise remove the item from the child collection
                        parentSourceItem.ChildProperties.Remove(item);

                    TreeListViewDropPosition relativeDropPosition = (TreeListViewDropPosition)destination.GetValue(RadTreeListView.DropPositionProperty);

                    // If put on top of destination
                    if (relativeDropPosition == TreeListViewDropPosition.Inside)
                    {
                        // the Parent_ID of the item will become the ID of the destination
                        item.Parent_ID = destinationItem.ID;
                        destinationItem.ChildProperties.Add(item);
                    }
                    // If put before or after the destination
                    else
                    {
                        // if the desitination is in the root collection
                        if (destinationItem.Parent_ID == null)
                        {
                            // The parent_ID of the item will also be null
                            item.Parent_ID = null;
                            Properties.Insert(Properties.IndexOf(destinationItem), item);
                        }
                        else
                        {
                            // otherwise the Parent_ID of the item will be the same as that of the destination item
                            item.Parent_ID = destinationItem.Parent_ID;
                            // find the Parent of the destination item
                            parentSourceItem = GetProperty(destinationItem.Parent_ID);
                            // Insert the item above the destination item in the ChildObject collection of the parent of the destination
                            if (relativeDropPosition == TreeListViewDropPosition.Before)
                                parentSourceItem.ChildProperties.Insert(parentSourceItem.ChildProperties.IndexOf(destinationItem), item);
                            else
                                parentSourceItem.ChildProperties.Insert(parentSourceItem.ChildProperties.IndexOf(destinationItem) + 1, item);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                RadWindow.Alert(ex.Message);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeLevel"></param>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        private Boolean FindObject(ObservableCollection<PropertyModel> treeLevel, string searchItem)
        {
            if (treeLevel == null) treeLevel = Properties;
            foreach (var propertyItem in treeLevel)
            {
                if (propertyItem.PropertyName == searchItem) return true;
                if (propertyItem.ChildProperties != null)
                    if (FindObject(propertyItem.ChildProperties, searchItem)) return true;
            }
            return false;
        }

        /// <summary>
        /// Find the Object with the searchItemID. This is a recursive function which looks through all levels of the tree
        /// If the treeLevel is omitted, the search will start in the root items
        /// </summary>
        /// <param name="searchItemID"></param>
        /// <param name="treeLevel"></param>
        /// <returns></returns>
        public PropertyModel GetProperty(Guid? searchItemID, ObservableCollectionWithItemChanged<PropertyModel> treeLevel = null)
        {
            // Select the root level if the treeLevel = null
            if (treeLevel == null) treeLevel = Properties;
            foreach (var propertyItem in treeLevel)
            {
                // return the item if found on this level
                if (propertyItem.ID == searchItemID) return propertyItem;

                if (propertyItem.ChildProperties != null)
                {
                    // Recursively call the method to find the item in the ChildProperties
                    PropertyModel childPropertyItem = GetProperty(searchItemID, propertyItem.ChildProperties);
                    if (childPropertyItem != null) return childPropertyItem;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyItem"></param>
        /// <param name="S88Type"></param>
        public void SetPropertyType(PropertyModel propertyItem, string S88Type)
        {
            propertyItem.PropertyType_ID = GetPropertyType_ID(S88Type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        private int GetPropertyType_ID(string propertyType)
        {
            //PropertyTypeModel propertyTypeItem = PropertyTypes.PropertyTypes.Single(x => x.ObjectType == propertyType);
            //return propertyTypeItem.ID;
            return 1;
        }

        private bool CanChangePropertyType()
        {
            return true;
        }

        private void ChangePropertyType(object p)
        {
            IsPropertyTypePopupOpen = true;
        }

        private bool CanSelectPropertyType()
        {
            return true;
        }

        private void SelectPropertyType(object p)
        {
            if (p != null)
                foreach (var item in selectedItems)
                    item.PropertyType_ID = (p as PropertyTypeModel).ID;
            IsPropertyTypePopupOpen = false;
        }
    }
    #endregion
 }
