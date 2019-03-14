using Sculptor.EDBEntityDataModel;
using Sculptor.ViewModels;
using Sculptor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;
using TD = Telerik.Windows.Data;

namespace Sculptor
{
    public class PropertyViewModel : ViewModelBase
    {
        #region Constructor
        public PropertyViewModel()
        {
            // Load the properties in the background
            //var backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += this.OnLoadInBackground;
            //backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            //backgroundWorker.RunWorkerAsync();
            LoadCBAspects();
            LoadCBAttributes();
            Load(null);
        }
        #endregion

        #region Properties
        private TD.ObservableItemCollection<PropertyModel> properties = new TD.ObservableItemCollection<PropertyModel>();
        public TD.ObservableItemCollection<PropertyModel> Properties
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

        // Aspects and Attributes are linked to the Aspect and Attribute view models. The only reason for declaring them in the
        // property view model is that they need to be used in the combo boxes of the property view
        private readonly TD.ObservableItemCollection<AspectModel> aspects = new TD.ObservableItemCollection<AspectModel>();
        public TD.ObservableItemCollection<AspectModel> CBAspects { get { return aspects; } }

        private readonly TD.ObservableItemCollection<AttributeModel> attributes = new TD.ObservableItemCollection<AttributeModel>();
        public TD.ObservableItemCollection<AttributeModel> CBAttributes { get { return attributes; } }

        private TD.ObservableItemCollection<PropertyModel> selectedItems;
        public TD.ObservableItemCollection<PropertyModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new TD.ObservableItemCollection<PropertyModel>();
                }
                return selectedItems;
            }
        }

        private CollectionViewSource filteredProperties;
        public CollectionViewSource FilteredProperties
        {
            get { return filteredProperties; }
            set
            {
                if (value != filteredProperties)
                {
                    filteredProperties = value;
                    OnPropertyChanged();
                }
            }
        }

        private PropertyModel selectedItem;
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

        private bool isLoaded;
        public bool IsLoaded
        {
            get { return this.isLoaded; }
            set
            {
                if (value != this.isLoaded)
                {
                    this.isLoaded = value;
                }
            }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return this.isBusy; }
            set
            {
                if (value != this.isBusy)
                {
                    this.isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private void OnLoadInBackground(object sender, DoWorkEventArgs e)
        {
            this.IsBusy = false;
            IsLoaded = false;
            Properties.SuspendNotifications();

            // Load Object Types;
            TypeViewModelLocator.GetTypeVM();
            // Load Objects
            LoadCBAspects();
            LoadCBAttributes();
            Load(null);

            FilteredProperties = new CollectionViewSource { Source = Properties };
            FilteredProperties.Filter += PropertyFilter;

        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // CollectionView to filter the TreeListView
            // Note: Data is manipulated in the Objects collection
            FilteredProperties = new CollectionViewSource { Source = Properties };
            FilteredProperties.Filter += PropertyFilter;

            Properties.ResumeNotifications();
            //LoadTreeState();
            IsLoaded = true;
            this.IsBusy = false;

            //Dispose events
            var backgroundWorker = sender as BackgroundWorker;
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads the records from the DbSet into the ViewModel. This function designed for recursive use
        /// </summary>
        /// <param name="Project_ID"></param>
        /// <param name="Parent_ID"></param>
        /// <returns>Observable collection of VMObjects</returns>
        private TD.ObservableItemCollection<PropertyModel> Load(Guid? Parent_ID)
        {
            TD.ObservableItemCollection<PropertyModel> childProperties = new TD.ObservableItemCollection<PropertyModel>();
            
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
                        Value = Rec.Value,
                        IsChanged = false,
                        IsNew = false,
                        IsDeleted = false
                    };

                    propertyItem.ChildProperties = Load(Rec.ID);

                    // If the parent ID is null, this is a root object and needs to be added to the VM class
                    // Else it is a child object which needs to be added to the childobjectlist
                    if (Rec.Parent_ID == null)
                        Properties.Add(propertyItem);
                    else
                        childProperties.Add(propertyItem);

                }
            }

            return childProperties;
        }

        public void LoadCBAspects()
        {
            AspectViewModel aspectVM = AspectViewModelLocator.GetAspectVM();
            CBAspects.Clear();
            foreach (var item in aspectVM.Aspects)
                CBAspects.Add(item);
        }

        public void LoadCBAttributes()
        {
            AttributeViewModel attributeVM = AttributeViewModelLocator.GetAttributeVM();
                CBAttributes.Clear();
            foreach (var item in attributeVM.Attributes)
                CBAttributes.Add(item);
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
                IsChanged = false,
                IsNew = true,
                IsDeleted = false,
                PropertyType_ID = TypeViewModelLocator.GetTypeVM().GetTypeGroupID("Property"),
                ChildProperties = new TD.ObservableItemCollection<PropertyModel>()
            };

            if (SelectedItem == null)
            {
                propertyItem.Parent_ID = null;
                Properties.Add(propertyItem);
            }
            else if (SelectedItem.Parent_ID == null)
            {
                propertyItem.Parent_ID = null;
                propertyItem.PropertyType_ID = SelectedItem.PropertyType_ID;
                Properties.Insert(Properties.IndexOf(SelectedItem) + 1, propertyItem);
            }
            else
            {
                PropertyModel parentItem = GetProperty(SelectedItem.Parent_ID);
                propertyItem.Parent_ID = SelectedItem.Parent_ID;
                propertyItem.PropertyType_ID = SelectedItem.PropertyType_ID;
                parentItem.ChildProperties.Insert(parentItem.ChildProperties.IndexOf(SelectedItem) + 1, propertyItem);
            }
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
                IsChanged = false,
                IsNew = true,
                IsDeleted = false,
                PropertyType_ID = TypeViewModelLocator.GetTypeVM().GetTypeGroupID("Property"),
                ChildProperties = new TD.ObservableItemCollection<PropertyModel>()
            };
            if (SelectedItem != null)
            {
                propertyItem.Parent_ID = SelectedItem.ID;
                SelectedItem.ChildProperties.Add(propertyItem);
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

            // To determine which items have been deleted in the collection, get all properties of the project stored in the database table first
            var tblProperties = eDB.tblProperties.Where(p => p.Project_ID == Globals.Project_ID);

            // Check if each property of the table exists in the Properties collection
            // if not, delete the property in the table
            foreach (var propertyRec in tblProperties)
            {
                var propertyItem = GetProperty(propertyRec.ID);
                if (propertyItem == null) // property not found in collection
                    eDB.tblProperties.Remove(propertyRec);
            }

            // Add and update properties recursively
            SaveLevel(Properties, eDB);
            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Fault while saving properties:\n" + ex.Message
                });
            }
            SaveTreeState();
            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(TD.ObservableItemCollection<PropertyModel> treeLevel, EDBEntities eDB)
        {
            try
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
                            Rec.Value = propertyItem.Value;
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
                            Rec.Value = propertyItem.Value;
                            propertyItem.IsChanged = false;
                        }
                        // DeleteRecursive call to add/Update children
                        if (propertyItem.ChildProperties != null) SaveLevel(propertyItem.ChildProperties, eDB);
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
            LoadCBAspects();
            LoadCBAttributes();
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
            if (destination != null)
            {
                PropertyModel destinationItem = (destination.DataContext) as PropertyModel;
                try
                {
                    // Setup a private collection with the selected items only. This is because the SelectedItems that are part of the view model collection
                    // will change as soon as we start removing and adding objects
                    TD.ObservableItemCollection<PropertyModel> selectedItems = new TD.ObservableItemCollection<PropertyModel>();
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
                        destination.UpdateLayout();
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
                                // Insert the item before or after the destination item in the ChildObject collection of the parent of the destination
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
            //try
            //{
            //    // Setup a private collection with the selected items only. This is because the SelectedItems that are part of the view model collection
            //    // will change as soon as we start removing and adding objects
            //    TD.ObservableItemCollection<PropertyModel> selectedItems = new TD.ObservableItemCollection<PropertyModel>();
            //    foreach (PropertyModel item in SelectedItems)
            //    {
            //        selectedItems.Add(item);
            //    }

            //    foreach (PropertyModel item in selectedItems)
            //    {
            //        // find the original parent of the object that's moved
            //        PropertyModel parentSourceItem = GetProperty(item.Parent_ID);

            //        // If the parent is in the root level
            //        if (parentSourceItem == null)
            //            // Remove the item in the root level
            //            Properties.Remove(item);
            //        else
            //            // Otherwise remove the item from the child collection
            //            parentSourceItem.ChildProperties.Remove(item);


            //        if (destination != null)
            //        {
            //            TreeListViewDropPosition relativeDropPosition = (TreeListViewDropPosition)destination.GetValue(RadTreeListView.DropPositionProperty);
            //            PropertyModel destinationItem = (destination.DataContext) as PropertyModel;
            //            // If put on top of destination
            //            if (relativeDropPosition == TreeListViewDropPosition.Inside)
            //            {
            //                // the Parent_ID of the item will become the ID of the destination
            //                item.Parent_ID = destinationItem.ID;
            //                destinationItem.ChildProperties.Add(item);
            //            }
            //            // If put before or after the destination
            //            else
            //            {
            //                // if the desitination is in the root collection
            //                if (destinationItem.Parent_ID == null)
            //                {
            //                    // The parent_ID of the item will also be null
            //                    item.Parent_ID = null;
            //                    Properties.Insert(Properties.IndexOf(destinationItem), item);
            //                }
            //                else
            //                {
            //                    // otherwise the Parent_ID of the item will be the same as that of the destination item
            //                    item.Parent_ID = destinationItem.Parent_ID;
            //                    // find the Parent of the destination item
            //                    parentSourceItem = GetProperty(destinationItem.Parent_ID);
            //                    // Insert the item above the destination item in the ChildObject collection of the parent of the destination
            //                    if (relativeDropPosition == TreeListViewDropPosition.Before)
            //                        parentSourceItem.ChildProperties.Insert(parentSourceItem.ChildProperties.IndexOf(destinationItem), item);
            //                    else
            //                        parentSourceItem.ChildProperties.Insert(parentSourceItem.ChildProperties.IndexOf(destinationItem) + 1, item);
            //                }
            //            }

            //        }
            //        else // destination is null, i.e. below the tree
            //        {
            //            item.Parent_ID = null;
            //            Properties.Add(item);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    RadWindow.Alert(ex.Message);
            //}


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeLevel"></param>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        private Boolean FindObject(TD.ObservableItemCollection<PropertyModel> treeLevel, string searchItem)
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
        public PropertyModel GetProperty(Guid? searchItemID, TD.ObservableItemCollection<PropertyModel> treeLevel = null)
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
        public void SetPropertyType(PropertyModel propertyItem, string propertyType)
        {
            propertyItem.PropertyType_ID = GetPropertyType_ID(propertyType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        private int GetPropertyType_ID(string propertyType)
        {
            TypeModel propertyTypeItem = TypeViewModelLocator.GetTypeVM().Types.Single(x => x.TypeGroup == propertyType);
            return propertyTypeItem.ID;
        }

        public void PropertyFilter(object sender, FilterEventArgs e)
        {
            if (e.Item != null)
                e.Accepted = (e.Item as PropertyModel).IsDeleted == false;
        }

        private void ChangeType(object p)
        {
            // ToDo: Bad practice to call a view from the viewmodel. Fix using IOC
            var typeSelectionPopup = new TypeSelectionPopup();
            typeSelectionPopup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            TypeViewModel typeViewModel = TypeViewModelLocator.GetTypeVM();
            // Close the type selection box
            typeViewModel.CloseTrigger = false;
            typeViewModel.TypeGroup = "Property";
            // Filter the type collection on the type group
            typeViewModel.FilterText = typeViewModel.TypeGroup;
            // To have one popup for all type groups (object, template, property etc) the popup is embedded in a dialog
            typeSelectionPopup.ShowDialog();
        }

        private void LoadTreeState()
        {
            TD.ObservableItemCollection<PropertyModel> isExpandedCollection;

            XmlSerializer x = new XmlSerializer(typeof(TD.ObservableItemCollection<PropertyModel>));
            //ToDo: put filename in configuration
            var xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sculptor\\" + Globals.ContractNo + "_PropertyExpandedState.xml");
            if (File.Exists(xmlFileName))
            {
                try
                {
                    using (var stream = new FileStream(xmlFileName, FileMode.Open))
                    {
                        isExpandedCollection = x.Deserialize(stream) as TD.ObservableItemCollection<PropertyModel>;
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

        private void LoadTreeStateRecursive(TD.ObservableItemCollection<PropertyModel> isExpandedCollectionLevel)
        {
            foreach (var item in isExpandedCollectionLevel)
            {
                var objectItem = GetProperty(item.ID);
                if (objectItem != null)
                    objectItem.IsExpanded = item.IsExpanded;

                if (objectItem.ChildProperties.Count != 0)
                    LoadTreeStateRecursive(item.ChildProperties);
            }

        }

        private void SaveTreeState()
        {
            XmlSerializer x = new XmlSerializer(typeof(TD.ObservableItemCollection<PropertyModel>));
            //ToDo: put filename in configuration
            var xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sculptor\\" + Globals.ContractNo + "_PropertyExpandedState.xml");
            try
            {
                using (StreamWriter sw = new StreamWriter(xmlFileName))
                {
                    x.Serialize(sw, Properties);
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
    }
    #endregion
 }
