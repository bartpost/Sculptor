using Sculptor.EDBEntityDataModel;
using Sculptor.DataModels;
using Sculptor.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;

namespace Sculptor.ViewModels
{
    public class ObjectViewModel : ViewModelBase, INotifyPropertyChanged, IRequestFocus
    {
        #region private declarations
        private ObservableCollectionWithItemChanged<ObjectModel> objects = new ObservableCollectionWithItemChanged<ObjectModel>();
        private ObservableCollectionWithItemChanged<ObjectModel> backgroundObjects = new ObservableCollectionWithItemChanged<ObjectModel>();
        private ObservableCollection<ObjectTypeModel> objectTypes = new ObservableCollection<ObjectTypeModel>();
        private ObjectModel selectedItem;
        private ObservableCollectionWithItemChanged<ObjectModel> selectedItems;
        private ObservableCollectionWithItemChanged<ObjectModel> copiedItems;
        private bool isChanged;
        private bool isBusy=true;
        private bool isObjectTypePopupOpen;
        private bool isAddObjectTypePopupOpen;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand addSiblingCommand;
        private ICommand addChildCommand;
        private ICommand deleteCommand;
        private ICommand changeObjectTypeCommand;
        private ICommand selectObjectTypeCommand;
        private ICommand addObjectTypeCommand;
        private ICommand cutCommand;
        private ICommand copyCommand;
        private ICommand pasteCommand;
        #endregion

        #region Constructor
        public ObjectViewModel()
        {
            // Load the objects in the background
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            backgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Properties

        public ObservableCollectionWithItemChanged<ObjectModel> Objects
        {
            get
            {
                return objects;
            }
            set
            {
                objects = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionWithItemChanged<ObjectModel> BackgroundObjects
        {
            get
            {
                return backgroundObjects;
            }
            set
            {
                backgroundObjects = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionWithItemChanged<ObjectModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<ObjectModel>();
                }
                return selectedItems;
            }
        }

        public ObservableCollectionWithItemChanged<ObjectModel> CopiedItems
        {
            get
            {
                if (copiedItems == null)
                {
                    copiedItems = new ObservableCollectionWithItemChanged<ObjectModel>();
                }
                return copiedItems;
            }
        }

        public ObservableCollection<ObjectTypeModel> ObjectTypes
        {
            get
            {
                if (objectTypes == null)
                {
                    objectTypes = new ObservableCollection<ObjectTypeModel>();
                }
                return objectTypes;
            }

        }

        public ObjectModel SelectedItem
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

                    //refresh the collection that holds the functionalities and requirements for the selected object. The collection is binded to the ObjectFunctionalityTree.
                    ObjectAssociationViewModel ObjectAssociationVM = ObjectAssociationViewModelLocator.GetObjectAssociationVM();
                    if (ObjectAssociationVM.FilteredObjectAssociations != null)
                        ObjectAssociationVM.FilteredObjectAssociations.View.Refresh();
                    ObjectRequirementViewModel ObjectRequirementVM = ObjectRequirementViewModelLocator.GetObjectRequirementVM();
                    if (ObjectRequirementVM.FilteredObjectRequirements != null)
                        ObjectRequirementVM.FilteredObjectRequirements.View.Refresh();
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
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }
            set
            {
                if (value != this.isBusy)
                {
                    this.isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsObjectTypePopupOpen
        {
            get
            {
                return this.isObjectTypePopupOpen;
            }
            set
            {
                if (value != this.isObjectTypePopupOpen)
                {
                    this.isObjectTypePopupOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAddObjectTypePopupOpen
        {
            get
            {
                return this.isObjectTypePopupOpen;
            }
            set
            {
                if (value != this.isObjectTypePopupOpen)
                {
                    this.isObjectTypePopupOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Error => throw new NotImplementedException();

        public string this[string columnName] => throw new NotImplementedException();
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

        public ICommand ChangeObjectTypeCommand
        {
            get
            {
                if (changeObjectTypeCommand == null)
                {
                    changeObjectTypeCommand = new RelayCommand(
                        p => this.CanChangeObjectType(),
                        p => this.ChangeObjectType(p));
                }
                return changeObjectTypeCommand;
            }
        }

        public ICommand SelectObjectTypeCommand
        {
            get
            {
                if (selectObjectTypeCommand == null)
                {
                    selectObjectTypeCommand = new RelayCommand(
                        p => this.CanSelectObjectType(),
                        p => this.SelectObjectType(p));
                }
                return selectObjectTypeCommand;
            }
        }

        public ICommand AddObjectTypeCommand
        {
            get
            {
                if (addObjectTypeCommand == null)
                {
                    addObjectTypeCommand = new RelayCommand(
                        p => this.CanAddObjectType(),
                        p => this.AddObjectType(p));
                }
                return addObjectTypeCommand;
            }
        }

        public ICommand CutCommand
        {
            get
            {
                if (cutCommand == null)
                {
                    cutCommand = new RelayCommand(
                        p => this.CanCut(),
                        p => this.Cut());
                }
                return cutCommand;
            }
        }

        public ICommand CopyCommand
        {
            get
            {
                if (copyCommand == null)
                {
                    copyCommand = new RelayCommand(
                        p => this.CanCopy(),
                        p => this.Copy());
                }
                return copyCommand;
            }
        }

        public ICommand PasteCommand
        {
            get
            {
                if (pasteCommand == null)
                {
                    pasteCommand = new RelayCommand(
                        p => this.CanPaste(),
                        p => this.Paste());
                }
                return pasteCommand;
            }
        }

        #endregion

        #region Events
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<FocusRequestedEventArgs> FocusRequested;
        protected virtual void OnFocusRequested(string propertyName)
        {
            FocusRequested?.Invoke(this, new FocusRequestedEventArgs(propertyName));
        }

        private void OnLoadInBackground(object sender, DoWorkEventArgs e)
        {
            this.IsBusy = true;
            LoadObjectTypes();
            Load(null);
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            this.IsBusy = false;

            Objects = BackgroundObjects;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the records from the DbSet into the ViewModel. This function designed for recursive use
        /// </summary>
        /// <param name="Project_ID"></param>
        /// <param name="Parent_ID"></param>
        /// <returns>Observable collection of VMObjects</returns>
        private ObservableCollectionWithItemChanged<ObjectModel> Load(Guid? Parent_ID)
        {
            ObservableCollectionWithItemChanged<ObjectModel> childObjects = new ObservableCollectionWithItemChanged<ObjectModel>();
            ObservableCollectionWithItemChanged<ObjectModel> personalLayout = new ObservableCollectionWithItemChanged<ObjectModel>();

            // Future local XML file for IsExpanded property
            //ObservableCollectionWithItemChanged<ObjectModel> pl;
            //bool expanded = false;

            //string localXMLPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ObjectView.xml");
            //var serializer = new XmlSerializer(personalLayout.GetType(), new XmlRootAttribute("PersonalLayout"));
            //if (File.Exists(localXMLPath))
            //{
            //    var stream = new FileStream(localXMLPath, FileMode.Open);
            //    pl = serializer.Deserialize(stream) as ObservableCollectionWithItemChanged<ObjectModel>;
            //}
            //else
            //    pl = new ObservableCollectionWithItemChanged<ObjectModel>();

            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblObject Rec in (from o in eDB.tblObjects where (o.Project_ID == Globals.Project_ID && o.Parent_ID == Parent_ID) orderby o.ObjectName select o))
                {
                    //var xmlRec = pl.SingleOrDefault(x => x.ID == Rec.ID);
                    //if ((xmlRec) != null) expanded = xmlRec.IsExpanded;

                    ObjectModel objectItem = new ObjectModel
                    {
                        ID = Rec.ID,
                        Parent_ID = Rec.Parent_ID,
                        Project_ID = Rec.Project_ID,
                        ObjectName = Rec.ObjectName,
                        Description = Rec.Description,
                        ObjectType_ID = (int)Rec.ObjectType_ID,
                        IsChanged = false,
                        IsExpanded = Rec.IsExpanded.GetValueOrDefault()
                    };

                    // Load objects with a parent_ID equal to the ID of this object
                    objectItem.ChildObjects = Load(Rec.ID);

                    // If the parent ID is null, this is a root object and needs to be added to the collection that is the itemsource of the object tree
                    // Else it is a child object which needs to be added to the childobjectlist
                    if (Rec.Parent_ID == null)
                        BackgroundObjects.Add(objectItem);
                    else
                        childObjects.Add(objectItem);
                }
            }
            IsChanged = false;
            return childObjects;
        }

        private void LoadObjectTypes()
        {
            ObjectTypes.Clear();
            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblObjectType Rec in (from o in eDB.tblObjectTypes where o.Project_ID == Globals.Project_ID select o))
                {
                    ObjectTypeModel objectTypeItem = new ObjectTypeModel
                    {
                        ID = Rec.ID,
                        ObjectType = Rec.ObjectType,
                        Description = Rec.Description,
                        Image = Rec.Image
                    };
                    ObjectTypes.Add(objectTypeItem);
                }
            }
        }

        private bool CanAddSibling()
        {
            return true;
        }

        public void AddSibling()
        {
            // Instanciate a new object
            ObjectModel objectItem = new ObjectModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                ObjectName = "New Object",
                Description = "New Object Description",
                ObjectType_ID = 5,
                IsChanged = false,
                IsNew = true,
                ChildObjects = new ObservableCollectionWithItemChanged<ObjectModel>()
            };

            // If no item has been selected, put the object in the root of the tree
            if (SelectedItem == null)
            {
                objectItem.Parent_ID = null;
                Objects.Add(objectItem);
            }
            // If the selected item is in the root, put the new object in the root also
            else if (SelectedItem.Parent_ID == null)
            {
                objectItem.Parent_ID = null;
                Objects.Insert(Objects.IndexOf(SelectedItem) + 1, objectItem);
            }
            // Otherwise get the parent object and add the new object as a child
            else
            {
                ObjectModel parentItem = GetObject(SelectedItem.Parent_ID);
                objectItem.Parent_ID = SelectedItem.Parent_ID;
                parentItem.ChildObjects.Insert(parentItem.ChildObjects.IndexOf(SelectedItem) + 1, objectItem);
            }
            IsChanged = true;
            OnFocusRequested("ObjectName");
        }

        private bool CanAddChild()
        {
            return SelectedItem != null;
        }

        public void AddChild()
        {
            ObjectModel objectItem = new ObjectModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                ObjectName = "New Object",
                Description = "New Object Description",
                ObjectType_ID = 5,
                IsChanged = false,
                IsNew = true,
                ChildObjects = new ObservableCollectionWithItemChanged<ObjectModel>()
            };
            if (SelectedItem != null)
            {
                objectItem.Parent_ID = SelectedItem.ID;
                SelectedItem.ChildObjects.Add(objectItem);
            }
            IsChanged = true;
        }

        private bool CanDelete()
        {
            return true;
        }

        private void Delete()
        {
            SelectedItem.IsDeleted = true;
            SelectedItem.IsChanged = false;
            SelectedItem.IsNew = false;
            IsChanged = true;
        }

        private bool CanSave()
        {
            return true;
        }

        public void Save()
        {
            EDBEntities eDB = new EDBEntities();
            SaveLevel(Objects, eDB);

            try
            { 
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert("Fault while saving object requirements: " + ex.Message);
            }

            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(ObservableCollectionWithItemChanged<ObjectModel> treeLevel, EDBEntities eDB)
        {
            if (treeLevel != null)
            {
                foreach (var objectItem in treeLevel)
                {

                    if (objectItem.IsNew)
                    {
                        tblObject NewRec = new tblObject();
                        var Rec = eDB.tblObjects.Add(NewRec);
                        Rec.ID = objectItem.ID;
                        Rec.Parent_ID = objectItem.Parent_ID;
                        Rec.ObjectName = objectItem.ObjectName;
                        Rec.Description = objectItem.Description;
                        Rec.Project_ID = Globals.Project_ID;
                        Rec.ObjectType_ID = objectItem.ObjectType_ID;
                        Rec.IsExpanded = objectItem.IsExpanded;
                        objectItem.IsNew = false;
                    }
                    if (objectItem.IsChanged)
                    {
                        tblObject Rec = eDB.tblObjects.Where(o => o.ID == objectItem.ID).FirstOrDefault();
                        Rec.Parent_ID = objectItem.Parent_ID;
                        Rec.ObjectName = objectItem.ObjectName;
                        Rec.Description = objectItem.Description;
                        Rec.Project_ID = objectItem.Project_ID;
                        Rec.ObjectType_ID = objectItem.ObjectType_ID;
                        Rec.IsExpanded = objectItem.IsExpanded;
                        objectItem.IsChanged = false;
                    }
                    if (objectItem.IsDeleted)
                    {
                        tblObject Rec = eDB.tblObjects.Where(o => o.ID == objectItem.ID).FirstOrDefault();
                        if (Rec != null)
                            eDB.tblObjects.Remove(Rec);
                    }
                    // Recursive call
                    if (objectItem.ChildObjects != null) SaveLevel(objectItem.ChildObjects, eDB);
                }
            }
        }

        private bool CanRefresh()
        {
            return true;
        }

        public void Refresh()
        {
            BackgroundObjects.Clear();
            LoadObjectTypes();
            Load(null);
        }

        private bool CanCut()
        {
            return true;
        }

        public void Cut()
        {

        }

        private bool CanCopy()
        {
            return SelectedItems.Count > 0;
        }

        public void Copy()
        {
            CopiedItems.Clear();
            foreach (var item in SelectedItems)
            {
                CopiedItems.Add(item);
            }

        }

        private bool CanPaste()
        {
            return CopiedItems.Count > 0; ;
        }

        public void Paste()
        {
            foreach (var objectItem in CopiedItems)
            {
                ObjectModel copiedObjectItem = new ObjectModel();
                copiedObjectItem.ID = Guid.NewGuid();
                copiedObjectItem.Project_ID = objectItem.Project_ID;
                copiedObjectItem.ObjectName = objectItem.ObjectName + "_1";
                copiedObjectItem.Description = objectItem.Description;
                copiedObjectItem.ObjectType_ID = objectItem.ObjectType_ID;
                copiedObjectItem.IsChanged = false;
                copiedObjectItem.IsNew = true;
                copiedObjectItem.ChildObjects = new ObservableCollectionWithItemChanged<ObjectModel>();

                if (SelectedItem == null)
                {
                    copiedObjectItem.Parent_ID = null;
                    Objects.Add(copiedObjectItem);
                }
                // Otherwise get the parent object and add the new object as a child
                else
                {
                    copiedObjectItem.Parent_ID = SelectedItem.ID;
                    SelectedItem.ChildObjects.Add(copiedObjectItem);
                }
                //SelectedItem = copiedObjectItem;
            }
            
            IsChanged = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            if (IsChanged) RadWindow.Confirm("Save Objects?", OnClosed);
            Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnClosed(object sender, WindowClosedEventArgs e)
        {
            var result = e.DialogResult;
            if (result == true) Save();
        }

        /// <summary>
        /// Method triggered by the TreeListViewDragDropBehavior Class. Takes care of moving on item in the tree, which can be from 
        /// any level to any level
        /// </summary>
        /// <param name="destination"></param>
        public void MoveSelection(TreeListViewRow destination) // Collection<ObjectModel> selectedItems, ObjectModel destination )
        {
            if (destination != null)
            {
                ObjectModel destinationItem = (destination.DataContext) as ObjectModel;
                try
                {
                    // Setup a private collection with the selected items only. This is because the SelectedItems that are part of the view model collection
                    // will change as soon as we start removing and adding objects
                    ObservableCollectionWithItemChanged<ObjectModel> selectedItems = new ObservableCollectionWithItemChanged<ObjectModel>();
                    foreach (ObjectModel item in SelectedItems)
                    {
                        selectedItems.Add(item);
                    }

                    foreach (ObjectModel item in selectedItems)
                    {
                        // find the original parent of the object that's moved
                        ObjectModel parentSourceItem = GetObject(item.Parent_ID);

                        // If the parent is in the root level
                        if (parentSourceItem == null)
                            // Remove the item in the root level
                            Objects.Remove(item);
                        else
                            // Otherwise remove the item from the child collection
                            parentSourceItem.ChildObjects.Remove(item);

                        TreeListViewDropPosition relativeDropPosition = (TreeListViewDropPosition)destination.GetValue(RadTreeListView.DropPositionProperty);
                        destination.UpdateLayout();
                        // If put on top of destination
                        if (relativeDropPosition == TreeListViewDropPosition.Inside)
                        {
                            // the Parent_ID of the item will become the ID of the destination
                            item.Parent_ID = destinationItem.ID;
                            destinationItem.ChildObjects.Add(item);
                        }
                        // If put before or after the destination
                        else
                        {
                            // if the desitination is in the root collection
                            if (destinationItem.Parent_ID == null)
                            {
                                // The parent_ID of the item will also be null
                                item.Parent_ID = null;
                                Objects.Insert(Objects.IndexOf(destinationItem), item);
                            }
                            else
                            {
                                // otherwise the Parent_ID of the item will be the same as that of the destination item
                                item.Parent_ID = destinationItem.Parent_ID;
                                // find the Parent of the destination item
                                parentSourceItem = GetObject(destinationItem.Parent_ID);
                                // Insert the item before or after the destination item in the ChildObject collection of the parent of the destination
                                if (relativeDropPosition == TreeListViewDropPosition.Before)
                                    parentSourceItem.ChildObjects.Insert(parentSourceItem.ChildObjects.IndexOf(destinationItem), item);
                                else
                                    parentSourceItem.ChildObjects.Insert(parentSourceItem.ChildObjects.IndexOf(destinationItem) + 1, item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    RadWindow.Alert(ex.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeLevel"></param>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        private Boolean FindObject(ObservableCollectionWithItemChanged<ObjectModel> treeLevel, string searchItem)
        {
            if (treeLevel == null) treeLevel = Objects;
            foreach (var objectItem in treeLevel)
            {
                if (objectItem.ObjectName == searchItem) return true;
                if (objectItem.ChildObjects != null)
                    if (FindObject(objectItem.ChildObjects, searchItem)) return true;
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
        private ObjectModel GetObject(Guid? searchItemID, ObservableCollectionWithItemChanged<ObjectModel> treeLevel = null)
        {
            // Select the root level if the treeLevel = null
            if (treeLevel == null) treeLevel = Objects;
            foreach (var objectItem in treeLevel)
            {
                // return the item if found on this level
                if (objectItem.ID == searchItemID) return objectItem;

                if (objectItem.ChildObjects != null)
                {
                    // Recursively call the method to find the item in the ChildObjects
                    ObjectModel childObjectItem = GetObject(searchItemID, objectItem.ChildObjects);
                    if (childObjectItem != null) return childObjectItem;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectItem"></param>
        /// <param name="S88Type"></param>
        public void SetObjectType(ObjectModel objectItem, string S88Type)
        {
            objectItem.ObjectType_ID = GetObjectType_ID(S88Type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        private int GetObjectType_ID(string objectType)
        {
            ObjectTypeModel objectTypeItem = ObjectTypes.Single(x => x.ObjectType == objectType);
            return objectTypeItem.ID;
        }

        private bool CanChangeObjectType()
        {
            return true;
        }

        private void ChangeObjectType(object p)
        {
            IsObjectTypePopupOpen = true;
        }

        private bool CanSelectObjectType()
        {
            return true;
        }

        private void SelectObjectType(object p)
        {
            if (p != null)
                foreach (var item in selectedItems)
                    item.ObjectType_ID = (p as ObjectTypeModel).ID;
            IsObjectTypePopupOpen = false;
        }

        private bool CanAddObjectType()
        {
            return true;
        }

        private void AddObjectType(object p)
        {
            IsObjectTypePopupOpen = false;
            if (Convert.ToInt16(p) == 1)
                IsAddObjectTypePopupOpen = true;
            if (Convert.ToInt16(p) == 2)
                IsAddObjectTypePopupOpen = false;

        }
    }
    #endregion
    
}