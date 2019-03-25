using Sculptor.EDBEntityDataModel;
using Sculptor.DataModels;
using Sculptor.Interfaces;
using Sculptor.Views;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;
using System.Windows;
using System.IO;
using System.Xml.Serialization;
using TD = Telerik.Windows.Data;
using System.Windows.Data;

namespace Sculptor.ViewModels
{
    public class ObjectViewModel : ViewModelBase, INotifyPropertyChanged, IRequestFocus
    {
        #region Constructor
        public ObjectViewModel()
        {
            Objects.SuspendNotifications();
            Load(null);
            LoadTreeState();
            Objects.ResumeNotifications();
        }
        #endregion

        #region Properties

        private TD.ObservableItemCollection<ObjectModel> objects = new TD.ObservableItemCollection<ObjectModel>();
        public TD.ObservableItemCollection<ObjectModel> Objects
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

        private TD.ObservableItemCollection<ObjectModel> selectedItems;
        public TD.ObservableItemCollection<ObjectModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new TD.ObservableItemCollection<ObjectModel>();
                }
                return selectedItems;
            }
        }

        private TD.ObservableItemCollection<ObjectModel> copiedItems;
        public TD.ObservableItemCollection<ObjectModel> CopiedItems
        {
            get
            {
                if (copiedItems == null)
                {
                    copiedItems = new TD.ObservableItemCollection<ObjectModel>();
                }
                return copiedItems;
            }
        }

        private CollectionViewSource filteredObjects;
        public CollectionViewSource FilteredObjects
        {
            get { return filteredObjects; }
            set
            {
                if (value != filteredObjects)
                {
                    filteredObjects = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObjectModel selectedItem;
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
                    OnPropertyChanged();
                }
            }
        }

        private bool isLoaded = false;
        public bool IsLoaded
        {
            get { return this.isLoaded; }
            set
            {
                if (value != this.isLoaded)
                {
                    this.isLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isBusy;
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

        public string Error => throw new NotImplementedException();

        public string this[string columnName] => throw new NotImplementedException();
        #endregion

        #region Commands

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

        private ICommand addSiblingCommand;
        public ICommand AddSiblingCommand
        {
            get
            {
                if (addSiblingCommand == null)
                    addSiblingCommand = new RelayCommand(p => true, p => this.AddSibling());
                return addSiblingCommand;
            }
        }

        private ICommand addChildCommand;
        public ICommand AddChildCommand
        {
            get
            {
                if (addChildCommand == null)
                    addChildCommand = new RelayCommand(p => true, p  => this.AddChild());
                return addChildCommand;
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

        private ICommand changeTypeCommand;
        public ICommand ChangeTypeCommand
        {
            get
            {
                if (changeTypeCommand == null)
                    changeTypeCommand = new RelayCommand(p => true, p => this.ChangeType(p));
                return changeTypeCommand;
            }
        }

        private ICommand cutCommand;
        public ICommand CutCommand
        {
            get
            {
                if (cutCommand == null)
                    cutCommand = new RelayCommand(p => this.CanCut(), p => this.Cut());
                return cutCommand;
            }
        }

        private ICommand copyCommand;
        public ICommand CopyCommand
        {
            get
            {
                if (copyCommand == null)
                    copyCommand = new RelayCommand(p => this.CanCopy(), p => this.Copy());
                return copyCommand;
            }
        }

        private ICommand pasteCommand;
        public ICommand PasteCommand
        {
            get
            {
                if (pasteCommand == null)
                    pasteCommand = new RelayCommand(p => this.CanPaste(), p => this.Paste());
                return pasteCommand;
            }
        }

        private ICommand loadTreeStateCommand;
        public ICommand LoadTreeStateCommand
        {
            get
            {
                if (loadTreeStateCommand == null)
                    loadTreeStateCommand = new RelayCommand(p => true, p => this.LoadTreeState());
                return loadTreeStateCommand;
            }
        }

        private ICommand saveTreeStateCommand;
        public ICommand SaveTreeStateCommand
        {
            get
            {
                if (saveTreeStateCommand == null)
                    saveTreeStateCommand = new RelayCommand(p => true, p => this.SaveTreeState());
                return saveTreeStateCommand;
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
            Objects.SuspendNotifications();

            // Load Object Types;
            TypeViewModelLocator.GetTypeVM();
            // Load Objects
            Load(null);
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            // CollectionView to filter the TreeListView
            // Note: Data is manipulated in the Objects collection
            //FilteredObjects = new CollectionViewSource { Source = Objects };
            //FilteredObjects.Filter += ObjectFilter;

            Objects.ResumeNotifications();
            //LoadTreeState();
            IsLoaded = true;
            this.IsBusy = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the records from the DbSet into the ViewModel. This function designed for recursive use
        /// </summary>
        /// <param name="Project_ID"></param>
        /// <param name="Parent_ID"></param>
        /// <returns>Observable collection of VMObjects</returns>
        private TD.ObservableItemCollection<ObjectModel> Load(Guid? Parent_ID)
        {
            TD.ObservableItemCollection<ObjectModel> childObjects = new TD.ObservableItemCollection<ObjectModel>();
           // TD.ObservableItemCollection<ObjectModel> personalLayout = new TD.ObservableItemCollection<ObjectModel>();

            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblObject Rec in (from o in eDB.tblObjects where (o.Project_ID == Globals.Project_ID && o.Parent_ID == Parent_ID) orderby o.ObjectName select o))
                {
                    ObjectModel objectItem = new ObjectModel
                    {
                        ID = Rec.ID,
                        Parent_ID = Rec.Parent_ID,
                        Project_ID = Rec.Project_ID,
                        ObjectName = Rec.ObjectName,
                        Description = Rec.Description,
                        ObjectType_ID = (int)Rec.ObjectType_ID,
                        IsChanged = false,
                    };

                    // Load objects with a parent_ID equal to the ID of this object
                    objectItem.ChildObjects = Load(Rec.ID);

                    // If the parent ID is null, this is a root object and needs to be added to the collection that is the itemsource of the object tree
                    // Else it is a child object which needs to be added to the childobjectlist
                    if (Rec.Parent_ID == null)
                        Objects.Add(objectItem);
                    else
                        childObjects.Add(objectItem);
                }
            }
            IsChanged = false;
            return childObjects;
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
                IsChanged = false,
                IsNew = true,
                ObjectType_ID = TypeViewModelLocator.GetTypeVM().GetTypeGroupID("Object"),
                ChildObjects = new TD.ObservableItemCollection<ObjectModel>()
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
                objectItem.ObjectType_ID = SelectedItem.ObjectType_ID;
                Objects.Insert(Objects.IndexOf(SelectedItem) + 1, objectItem);
            }
            // Otherwise get the parent object and add the new object as a child
            else
            {
                ObjectModel parentItem = GetObject(SelectedItem.Parent_ID);
                objectItem.Parent_ID = SelectedItem.Parent_ID;
                objectItem.ObjectType_ID = SelectedItem.ObjectType_ID;
                parentItem.ChildObjects.Insert(parentItem.ChildObjects.IndexOf(SelectedItem) + 1, objectItem);
            }
            IsChanged = true;
            OnFocusRequested("ObjectName");
        }

        public void AddChild()
        {
            ObjectModel objectItem = new ObjectModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                ObjectName = "New Object",
                Description = "New Object Description",
                IsChanged = false,
                IsNew = true,
                ChildObjects = new TD.ObservableItemCollection<ObjectModel>()
            };
            if (SelectedItem != null)
            {
                objectItem.Parent_ID = SelectedItem.ID;
                objectItem.ObjectType_ID = SelectedItem.ObjectType_ID + 1;
                SelectedItem.ChildObjects.Add(objectItem);
            }
            IsChanged = true;
        }

        private void Delete()
        {
            // ToDo: Deleting items in the collection only works using the Del key for now. 
            // Implement delete method to also provide option using context menu
            
        }

        public void Save()
        {
            EDBEntities eDB = new EDBEntities();

            // To determine which items have been deleted in the collection, get all objects of the project stored in the database table first
            var tblObjects = eDB.tblObjects.Where(p => p.Project_ID == Globals.Project_ID);

            // Check if each object of the table exists in the objects collection
            // if not, delete the object in the table
            foreach (var objectRec in tblObjects)
            {
                var objectItem = GetObject(objectRec.ID);
                if (objectItem == null) // object not found in collection
                    eDB.tblObjects.Remove(objectRec);
            }

            // Add and update objects recursively
            SaveLevel(Objects, eDB);

            try
            { 
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Fault while saving objects:\n" + ex.Message
                });
            }
            SaveTreeState();
            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(TD.ObservableItemCollection<ObjectModel> treeLevel, EDBEntities eDB)
        {
            try
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
                        // Recursive call
                        if (objectItem.ChildObjects != null) SaveLevel(objectItem.ChildObjects, eDB);
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

        public void Refresh()
        {
            Objects.Clear();
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
                copiedObjectItem.ChildObjects = new TD.ObservableItemCollection<ObjectModel>();

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
                    TD.ObservableItemCollection<ObjectModel> selectedItems = new TD.ObservableItemCollection<ObjectModel>();
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
                    RadWindow.Alert(new DialogParameters()
                    {
                        Header = "Error",
                        Content = "Error while moving object\n" + ex.Message
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeLevel"></param>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        private Boolean FindObject(TD.ObservableItemCollection<ObjectModel> treeLevel, string searchItem)
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
        private ObjectModel GetObject(Guid? searchItemID, TD.ObservableItemCollection<ObjectModel> treeLevel = null)
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

        private void ChangeType(object p)
        {
            // ToDo: Bad practice to call a view from the viewmodel. Fix using IOC
            var typeSelectionPopup = new TypeSelectionPopup();
            typeSelectionPopup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            TypeViewModel typeViewModel = TypeViewModelLocator.GetTypeVM();
            // Close the type selection box
            typeViewModel.CloseTrigger = false;
            typeViewModel.TypeGroup = "Object";
            // Filter the type collection on the type group
            typeViewModel.FilterText = typeViewModel.TypeGroup;
            // To have one popup for all type groups (object, template, property etc) the popup is embedded in a dialog
            typeSelectionPopup.ShowDialog();
        }

        private void LoadTreeState()
        {
            TD.ObservableItemCollection<ObjectModel> isExpandedCollection;

            XmlSerializer x = new XmlSerializer(typeof(TD.ObservableItemCollection<ObjectModel>));
            //ToDo: put filename in configuration
            var xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sculptor\\" + Globals.ContractNo + "_ObjectExpandedState.xml");
            if (File.Exists(xmlFileName))
            {
                try
                {
                    using (var stream = new FileStream(xmlFileName, FileMode.Open))
                    {
                        isExpandedCollection = x.Deserialize(stream) as TD.ObservableItemCollection<ObjectModel>;
                        LoadTreeStateRecursive(isExpandedCollection);
                    }
                }
                catch (Exception ex)
                {
                    RadWindow.Alert(new DialogParameters()
                    {
                        Header = "Error",
                        Content = "Error while opening expansion state\n" + ex.Message
                    });
                }
            }
        }

        private void LoadTreeStateRecursive(TD.ObservableItemCollection<ObjectModel> isExpandedCollectionLevel)
        {
            foreach (var item in isExpandedCollectionLevel)
            {
                var objectItem = GetObject(item.ID);
                if (objectItem != null)
                    objectItem.IsExpanded = item.IsExpanded;

                if (objectItem.ChildObjects.Count != 0)
                    LoadTreeStateRecursive(item.ChildObjects);
            }

        }

        private void SaveTreeState()
        {
            XmlSerializer x = new XmlSerializer(typeof(TD.ObservableItemCollection<ObjectModel>));
            //ToDo: put filename in configuration
            var xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sculptor\\" + Globals.ContractNo + "_ObjectExpandedState.xml");
            try
            {
                using (StreamWriter sw = new StreamWriter(xmlFileName))
                {
                    x.Serialize(sw, Objects);
                }
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Error while saving expansion state\n" + ex.Message
                });
            }
        }

        #endregion
    }

}