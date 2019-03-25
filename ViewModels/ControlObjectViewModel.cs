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
using System.IO;
using System.Xml.Serialization;
using TD = Telerik.Windows.Data;
using System.Windows.Data;

namespace Sculptor.ViewModels
{
    public class ControlObjectViewModel : ViewModelBase, INotifyPropertyChanged, IRequestFocus
    {
        #region Constructor
        public ControlObjectViewModel()
        {
            ControlObjects.SuspendNotifications();
            Load(null);
            LoadTreeState();
            ControlObjects.ResumeNotifications();
        }
        #endregion

        #region Properties

        private TD.ObservableItemCollection<ControlObjectModel> controlObjects = new TD.ObservableItemCollection<ControlObjectModel>();
        public TD.ObservableItemCollection<ControlObjectModel> ControlObjects
        {
            get
            {
                return controlObjects;
            }
            set
            {
                controlObjects = value;
                OnPropertyChanged();
            }
        }

        private TD.ObservableItemCollection<ControlObjectModel> selectedItems;
        public TD.ObservableItemCollection<ControlObjectModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new TD.ObservableItemCollection<ControlObjectModel>();
                }
                return selectedItems;
            }
        }

        private TD.ObservableItemCollection<ControlObjectModel> copiedItems;
        public TD.ObservableItemCollection<ControlObjectModel> CopiedItems
        {
            get
            {
                if (copiedItems == null)
                {
                    copiedItems = new TD.ObservableItemCollection<ControlObjectModel>();
                }
                return copiedItems;
            }
        }

        private CollectionViewSource filteredControlObjects;
        public CollectionViewSource FilteredControlObjects
        {
            get { return filteredControlObjects; }
            set
            {
                if (value != filteredControlObjects)
                {
                    filteredControlObjects = value;
                    OnPropertyChanged();
                }
            }
        }

        private ControlObjectModel selectedItem;
        public ControlObjectModel SelectedItem
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
                    //ObjectAssociationViewModel ObjectAssociationVM = ObjectAssociationViewModelLocator.GetControllerAssociationVM();
                    //if (ObjectAssociationVM.FilteredObjectAssociations != null)
                    //    ObjectAssociationVM.FilteredObjectAssociations.View.Refresh();
                    //ObjectRequirementViewModel ObjectRequirementVM = ObjectRequirementViewModelLocator.GetControllerRequirementVM();
                    //if (ObjectRequirementVM.FilteredObjectRequirements != null)
                    //    ObjectRequirementVM.FilteredObjectRequirements.View.Refresh();
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
                    addChildCommand = new RelayCommand(p => true, p => this.AddChild());
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
        #endregion

        #region Methods

        /// <summary>
        /// Loads the records from the DbSet into the ViewModel. This function designed for recursive use
        /// </summary>
        /// <param name="Project_ID"></param>
        /// <param name="Parent_ID"></param>
        /// <returns>Observable collection of VMControlObject</returns>
        private TD.ObservableItemCollection<ControlObjectModel> Load(Guid? Parent_ID)
        {
            TD.ObservableItemCollection<ControlObjectModel> ChildControlObjects = new TD.ObservableItemCollection<ControlObjectModel>();
            // TD.ObservableItemCollection<ControlObjectModel> personalLayout = new TD.ObservableItemCollection<ControlObjectModel>();

            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblControlObject Rec in (from o in eDB.tblControlObjects where (o.Project_ID == Globals.Project_ID && o.Parent_ID == Parent_ID) orderby o.ObjectName select o))
                {
                    ControlObjectModel controllerItem = new ControlObjectModel
                    {
                        ID = Rec.ID,
                        Parent_ID = Rec.Parent_ID,
                        Project_ID = Rec.Project_ID,
                        ObjectName = Rec.ObjectName,
                        Description = Rec.Description,
                        ControlObjectType_ID = (int)Rec.ControlObjectType_ID,
                        IsChanged = false,
                    };

                    // Load ControlObject with a parent_ID equal to the ID of this object
                    controllerItem.ChildControlObjects = Load(Rec.ID);

                    // If the parent ID is null, this is a root object and needs to be added to the collection that is the itemsource of the object tree
                    // Else it is a child object which needs to be added to the childobjectlist
                    if (Rec.Parent_ID == null)
                        ControlObjects.Add(controllerItem);
                    else
                        ChildControlObjects.Add(controllerItem);
                }
            }
            IsChanged = false;
            return ChildControlObjects;
        }

        public void AddSibling()
        {
            // Instanciate a new object
            ControlObjectModel controllerItem = new ControlObjectModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                ObjectName = "New Object",
                Description = "New Object Description",
                IsChanged = false,
                IsNew = true,
                ControlObjectType_ID = TypeViewModelLocator.GetTypeVM().GetTypeGroupID("Object"),
                ChildControlObjects = new TD.ObservableItemCollection<ControlObjectModel>()
            };

            // If no item has been selected, put the object in the root of the tree
            if (SelectedItem == null)
            {
                controllerItem.Parent_ID = null;
                ControlObjects.Add(controllerItem);
            }
            // If the selected item is in the root, put the new object in the root also
            else if (SelectedItem.Parent_ID == null)
            {
                controllerItem.Parent_ID = null;
                controllerItem.ControlObjectType_ID = SelectedItem.ControlObjectType_ID;
                ControlObjects.Insert(ControlObjects.IndexOf(SelectedItem) + 1, controllerItem);
            }
            // Otherwise get the parent object and add the new object as a child
            else
            {
                ControlObjectModel parentItem = GetControlObject(SelectedItem.Parent_ID);
                controllerItem.Parent_ID = SelectedItem.Parent_ID;
                controllerItem.ControlObjectType_ID = SelectedItem.ControlObjectType_ID;
                parentItem.ChildControlObjects.Insert(parentItem.ChildControlObjects.IndexOf(SelectedItem) + 1, controllerItem);
            }
            IsChanged = true;
            OnFocusRequested("ObjectName");
        }

        public void AddChild()
        {
            ControlObjectModel controllerItem = new ControlObjectModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                ObjectName = "New Object",
                Description = "New Object Description",
                IsChanged = false,
                IsNew = true,
                ChildControlObjects = new TD.ObservableItemCollection<ControlObjectModel>()
            };
            if (SelectedItem != null)
            {
                controllerItem.Parent_ID = SelectedItem.ID;
                controllerItem.ControlObjectType_ID = SelectedItem.ControlObjectType_ID + 1;
                SelectedItem.ChildControlObjects.Add(controllerItem);
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

            // To determine which items have been deleted in the collection, get all ControlObject of the project stored in the database table first
            var tblControlObjects = eDB.tblControlObjects.Where(p => p.Project_ID == Globals.Project_ID);

            // Check if each object of the table exists in the ControlObject collection
            // if not, delete the object in the table
            foreach (var ControllerRec in tblControlObjects)
            {
                var controllerItem = GetControlObject(ControllerRec.ID);
                if (controllerItem == null) // object not found in collection
                    eDB.tblControlObjects.Remove(ControllerRec);
            }

            // Add and update ControlObject recursively
            SaveLevel(ControlObjects, eDB);

            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Fault while saving ControlObject:\n" + ex.Message
                });
            }
            SaveTreeState();
            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(TD.ObservableItemCollection<ControlObjectModel> treeLevel, EDBEntities eDB)
        {
            try
            {
                if (treeLevel != null)
                {
                    foreach (var controllerItem in treeLevel)
                    {

                        if (controllerItem.IsNew)
                        {
                            tblControlObject NewRec = new tblControlObject();
                            var Rec = eDB.tblControlObjects.Add(NewRec);
                            Rec.ID = controllerItem.ID;
                            Rec.Parent_ID = controllerItem.Parent_ID;
                            Rec.ObjectName = controllerItem.ObjectName;
                            Rec.Description = controllerItem.Description;
                            Rec.Project_ID = Globals.Project_ID;
                            Rec.ControlObjectType_ID = controllerItem.ControlObjectType_ID;
                            Rec.IsExpanded = controllerItem.IsExpanded;
                            controllerItem.IsNew = false;
                        }
                        if (controllerItem.IsChanged)
                        {
                            tblControlObject Rec = eDB.tblControlObjects.Where(o => o.ID == controllerItem.ID).FirstOrDefault();
                            Rec.Parent_ID = controllerItem.Parent_ID;
                            Rec.ObjectName = controllerItem.ObjectName;
                            Rec.Description = controllerItem.Description;
                            Rec.Project_ID = controllerItem.Project_ID;
                            Rec.ControlObjectType_ID = controllerItem.ControlObjectType_ID;
                            Rec.IsExpanded = controllerItem.IsExpanded;
                            controllerItem.IsChanged = false;
                        }
                        // Recursive call
                        if (controllerItem.ChildControlObjects != null) SaveLevel(controllerItem.ChildControlObjects, eDB);
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
            ControlObjects.Clear();
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
            foreach (var controllerItem in CopiedItems)
            {
                ControlObjectModel copiedcontrollerItem = new ControlObjectModel();
                copiedcontrollerItem.ID = Guid.NewGuid();
                copiedcontrollerItem.Project_ID = controllerItem.Project_ID;
                copiedcontrollerItem.ObjectName = controllerItem.ObjectName + "_1";
                copiedcontrollerItem.Description = controllerItem.Description;
                copiedcontrollerItem.ControlObjectType_ID = controllerItem.ControlObjectType_ID;
                copiedcontrollerItem.IsChanged = false;
                copiedcontrollerItem.IsNew = true;
                copiedcontrollerItem.ChildControlObjects = new TD.ObservableItemCollection<ControlObjectModel>();

                if (SelectedItem == null)
                {
                    copiedcontrollerItem.Parent_ID = null;
                    ControlObjects.Add(copiedcontrollerItem);
                }
                // Otherwise get the parent object and add the new object as a child
                else
                {
                    copiedcontrollerItem.Parent_ID = SelectedItem.ID;
                    SelectedItem.ChildControlObjects.Add(copiedcontrollerItem);
                }
                //SelectedItem = copiedcontrollerItem;
            }

            IsChanged = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            if (IsChanged) RadWindow.Confirm("Save ControlObject?", OnClosed);
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
        public void MoveSelection(TreeListViewRow destination) // Collection<ControlObjectModel> selectedItems, ControlObjectModel destination )
        {
            if (destination != null)
            {
                ControlObjectModel destinationItem = (destination.DataContext) as ControlObjectModel;
                try
                {
                    // Setup a private collection with the selected items only. This is because the SelectedItems that are part of the view model collection
                    // will change as soon as we start removing and adding ControlObject
                    TD.ObservableItemCollection<ControlObjectModel> selectedItems = new TD.ObservableItemCollection<ControlObjectModel>();
                    foreach (ControlObjectModel item in SelectedItems)
                    {
                        selectedItems.Add(item);
                    }

                    foreach (ControlObjectModel item in selectedItems)
                    {
                        // find the original parent of the object that's moved
                        ControlObjectModel parentSourceItem = GetControlObject(item.Parent_ID);

                        // If the parent is in the root level
                        if (parentSourceItem == null)
                            // Remove the item in the root level
                            ControlObjects.Remove(item);
                        else
                            // Otherwise remove the item from the child collection
                            parentSourceItem.ChildControlObjects.Remove(item);

                        TreeListViewDropPosition relativeDropPosition = (TreeListViewDropPosition)destination.GetValue(RadTreeListView.DropPositionProperty);
                        destination.UpdateLayout();
                        // If put on top of destination
                        if (relativeDropPosition == TreeListViewDropPosition.Inside)
                        {
                            // the Parent_ID of the item will become the ID of the destination
                            item.Parent_ID = destinationItem.ID;
                            destinationItem.ChildControlObjects.Add(item);
                        }
                        // If put before or after the destination
                        else
                        {
                            // if the desitination is in the root collection
                            if (destinationItem.Parent_ID == null)
                            {
                                // The parent_ID of the item will also be null
                                item.Parent_ID = null;
                                ControlObjects.Insert(ControlObjects.IndexOf(destinationItem), item);
                            }
                            else
                            {
                                // otherwise the Parent_ID of the item will be the same as that of the destination item
                                item.Parent_ID = destinationItem.Parent_ID;
                                // find the Parent of the destination item
                                parentSourceItem = GetControlObject(destinationItem.Parent_ID);
                                // Insert the item before or after the destination item in the ChildObject collection of the parent of the destination
                                if (relativeDropPosition == TreeListViewDropPosition.Before)
                                    parentSourceItem.ChildControlObjects.Insert(parentSourceItem.ChildControlObjects.IndexOf(destinationItem), item);
                                else
                                    parentSourceItem.ChildControlObjects.Insert(parentSourceItem.ChildControlObjects.IndexOf(destinationItem) + 1, item);
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
        private Boolean FindObject(TD.ObservableItemCollection<ControlObjectModel> treeLevel, string searchItem)
        {
            if (treeLevel == null) treeLevel = ControlObjects;
            foreach (var controllerItem in treeLevel)
            {
                if (controllerItem.ObjectName == searchItem) return true;
                if (controllerItem.ChildControlObjects != null)
                    if (FindObject(controllerItem.ChildControlObjects, searchItem)) return true;
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
        private ControlObjectModel GetControlObject(Guid? searchItemID, TD.ObservableItemCollection<ControlObjectModel> treeLevel = null)
        {
            // Select the root level if the treeLevel = null
            if (treeLevel == null) treeLevel = ControlObjects;
            foreach (var controlObjectItem in treeLevel)
            {
                // return the item if found on this level
                if (controlObjectItem.ID == searchItemID) return controlObjectItem;

                if (controlObjectItem.ChildControlObjects != null)
                {
                    // Recursively call the method to find the item in the ChildControlObjects
                    ControlObjectModel childcontrollerItem = GetControlObject(searchItemID, controlObjectItem.ChildControlObjects);
                    if (childcontrollerItem != null) return childcontrollerItem;
                }
            }
            return null;
        }

        private void ChangeType(object p)
        {
            // ToDo: Bad practice to call a view from the viewmodel. Fix using IOC
            var typeSelectionPopup = new TypeSelectionPopup();
            TypeViewModel typeViewModel = TypeViewModelLocator.GetTypeVM();
            // Close the type selection box
            typeViewModel.CloseTrigger = false;
            typeViewModel.TypeGroup = "ControlObject";
            // Filter the type collection on the type group
            typeViewModel.FilterText = typeViewModel.TypeGroup;
            // To have one popup for all type groups (object, template, property etc) the popup is embedded in a dialog
            typeSelectionPopup.ShowDialog();
        }

        private void LoadTreeState()
        {
            TD.ObservableItemCollection<ControlObjectModel> isExpandedCollection;

            XmlSerializer x = new XmlSerializer(typeof(TD.ObservableItemCollection<ControlObjectModel>));
            //ToDo: put filename in configuration
            var xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sculptor\\" + Globals.ContractNo + "_ControlObjectExpandedState.xml");
            if (File.Exists(xmlFileName))
            {
                try
                {
                    using (var stream = new FileStream(xmlFileName, FileMode.Open))
                    {
                        isExpandedCollection = x.Deserialize(stream) as TD.ObservableItemCollection<ControlObjectModel>;
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

        private void LoadTreeStateRecursive(TD.ObservableItemCollection<ControlObjectModel> isExpandedCollectionLevel)
        {
            foreach (var item in isExpandedCollectionLevel)
            {
                var controllerItem = GetControlObject(item.ID);
                if (controllerItem != null)
                    controllerItem.IsExpanded = item.IsExpanded;

                if (controllerItem.ChildControlObjects.Count != 0)
                    LoadTreeStateRecursive(item.ChildControlObjects);
            }

        }

        private void SaveTreeState()
        {
            XmlSerializer x = new XmlSerializer(typeof(TD.ObservableItemCollection<ControlObjectModel>));
            //ToDo: put filename in configuration
            var xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sculptor\\" + Globals.ContractNo + "_ControlObjectExpandedState.xml");
            try
            {
                using (StreamWriter sw = new StreamWriter(xmlFileName))
                {
                    x.Serialize(sw, ControlObjects);
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
