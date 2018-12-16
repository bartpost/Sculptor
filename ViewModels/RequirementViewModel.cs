using Sculptor.DataModels;
using Sculptor.EDBEntityDataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;

namespace Sculptor.ViewModels
{
    public class RequirementViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ObservableCollectionWithItemChanged<RequirementModel> requirements = new ObservableCollectionWithItemChanged<RequirementModel>();
        private ObservableCollectionWithItemChanged<RequirementModel> backgroundRequirements = new ObservableCollectionWithItemChanged<RequirementModel>();
        private RequirementModel selectedItem;
        private ObservableCollectionWithItemChanged<RequirementModel> selectedItems;
        private ObservableCollectionWithItemChanged<RequirementTypeModel> requirementTypes;
        private bool isChanged;
        private bool isBusy;
        private bool hasLoaded = false;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand addSiblingCommand;
        private ICommand addChildCommand;
        private ICommand deleteCommand;
        private ICommand changeRequirementTypeCommand;

        #region Constructor
        public RequirementViewModel()
        {
            IsBusy = true;
            // Load the objects in the background
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            backgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Properties

        public ObservableCollectionWithItemChanged<RequirementModel> Requirements
        {
            get
            {
                return requirements;
            }
            set
            {
                if (value != requirements)
                {
                    requirements = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollectionWithItemChanged<RequirementModel> BackgroundRequirements
        {
            get
            {
                return backgroundRequirements;
            }
            set
            {
                if (value != backgroundRequirements)
                {
                    requirements = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollectionWithItemChanged<RequirementModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<RequirementModel>();
                }
                return selectedItems;
            }
        }

        public ObservableCollectionWithItemChanged<RequirementTypeModel> RequirementTypes
        {
            get
            {
                if (requirementTypes == null)
                {
                    requirementTypes = new ObservableCollectionWithItemChanged<RequirementTypeModel>();
                }
                return requirementTypes;
            }

        }

        public RequirementModel SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                if (value != selectedItem)
                {
                    selectedItem = value;
                    //ObjectRequirementViewModelLocator.GetObjectRequirementVM().FilteredObjectRequirements.View.Refresh();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (value != isBusy)
                {
                    isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsChanged
        {
            get
            {
                return isChanged;
            }
            set
            {
                if (value != isChanged)
                {
                    isChanged = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasLoaded
        {
            get
            {
                return hasLoaded;
            }
            set
            {
                if (value != hasLoaded)
                {
                    hasLoaded = value;
                    OnPropertyChanged();
                }
            }
        }
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

        public ICommand ChangeRequirementTypeCommand
        {
            get
            {
                if (changeRequirementTypeCommand == null)
                {
                    changeRequirementTypeCommand = new RelayCommand(
                        p => this.CanChangeRequirementType(),
                        p => this.ChangeRequirementType(p));
                }
                return changeRequirementTypeCommand;
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

            Requirements = BackgroundRequirements;

            IsBusy = false;
            HasLoaded = true;
        }
        #endregion

        #region Methods
        private ObservableCollectionWithItemChanged<RequirementModel> Load(Guid? Parent_ID)
        {
            ObservableCollectionWithItemChanged<RequirementModel> childRequirements = new ObservableCollectionWithItemChanged<RequirementModel>();

            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblRequirement Rec in (from o in eDB.tblRequirements where (o.Project_ID == Globals.Project_ID && o.Parent_ID == Parent_ID) orderby o.ArticleNo select o ))
                {

                    RequirementModel requirementItem = new RequirementModel
                    {
                        ID = Rec.ID,
                        Parent_ID = Rec.Parent_ID,
                        Project_ID = Globals.Project_ID, // Rec.Project_ID,
                        ArticleNo = Rec.ArticleNo,
                        ArticleHeader = Rec.ArticleHeader,
                        RequirementType_ID = (int)Rec.RequirementType_ID,
                        Content = Rec.Content,
                        Version = Rec.Version,
                        IsChanged = false
                    };

                    // Load objects with a parent_ID equal to the ID of this object
                    requirementItem.ChildRequirements = Load(Rec.ID);

                    // If the parent ID is null, this is a root object and needs to be added to the collection that is the itemsource of the object tree
                    // Else it is a child object which needs to be added to the childobjectlist
                    if (Rec.Parent_ID == null)
                        BackgroundRequirements.Add(requirementItem);
                    else
                        childRequirements.Add(requirementItem);

                }
            }
            IsChanged = false;
            return childRequirements;
        }

        private void LoadRequirementTypes()
        {
            RequirementTypes.Clear();
            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblRequirementType Rec in (from o in eDB.tblRequirementTypes select o))
                {
                    RequirementTypeModel requirementTypeItem = new RequirementTypeModel { ID = Rec.ID, RequirementType = Rec.RequirementType };
                    RequirementTypes.Add(requirementTypeItem);
                }
            }
        }

        private bool CanRefresh()
        {
            return true;
        }

        public void Refresh()
        {
            // For some reason we have to unselect the selected row before we clear the ItemSource
            SelectedItem = null;

            Requirements.Clear();
            Load(null);
        }

        private bool CanSave()
        {
            return true;
        }

        public void Save()
        {
            EDBEntities eDB = new EDBEntities();
            SaveLevel(Requirements, eDB);
            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert("Fault while saving requirements: " + ex.Message);
            }
            IsChanged = false;
        }

        private void SaveLevel(ObservableCollection<RequirementModel> treeLevel, EDBEntities eDB)
        {
            if (treeLevel != null)
            {
                foreach (var requirementItem in treeLevel)
                {

                    if (requirementItem.IsNew)
                    {
                        tblRequirement NewRec = new tblRequirement();
                        var Rec = eDB.tblRequirements.Add(NewRec);
                        Rec.ID = requirementItem.ID;
                        Rec.Parent_ID = requirementItem.Parent_ID;
                        Rec.ArticleNo = requirementItem.ArticleNo;
                        Rec.ArticleHeader = requirementItem.ArticleHeader;
                        Rec.Project_ID = Globals.Project_ID;
                        Rec.RequirementType_ID = requirementItem.RequirementType_ID;
                        Rec.Content = requirementItem.Content;
                        requirementItem.IsNew = false;
                    }
                    if (requirementItem.IsChanged)
                    {
                        tblRequirement Rec = eDB.tblRequirements.Where(o => o.ID == requirementItem.ID).FirstOrDefault();
                        Rec.Parent_ID = requirementItem.Parent_ID;
                        Rec.ArticleNo = requirementItem.ArticleNo;
                        Rec.ArticleHeader = requirementItem.ArticleHeader;
                        Rec.Project_ID = requirementItem.Project_ID;
                        Rec.RequirementType_ID = requirementItem.RequirementType_ID;
                        Rec.Content = requirementItem.Content;
                        requirementItem.IsChanged = false;
                    }
                    if (requirementItem.IsDeleted)
                    {
                        tblRequirement Rec = eDB.tblRequirements.Where(o => o.ID == requirementItem.ID).FirstOrDefault();
                        if (Rec != null)
                            eDB.tblRequirements.Remove(Rec);
                    }
                    // Recursive call
                    if (requirementItem.ChildRequirements != null) SaveLevel(requirementItem.ChildRequirements, eDB);
                }
            }
        }

        private bool CanAddSibling()
        {
            return true;
        }

        public void AddSibling()
        {
            {
                // Instanciate a new requirement
                RequirementModel requirementItem = new RequirementModel
                {
                    ID = Guid.NewGuid(),
                    Project_ID = Globals.Project_ID,
                    ArticleHeader = "New Article",
                    Version = "",
                    RequirementType_ID = 1,
                    IsChanged = false,
                    IsNew = true,
                    ChildRequirements = new ObservableCollectionWithItemChanged<RequirementModel>()
                };

                // If no item has been selected, put the object in the root of the tree
                if (SelectedItem == null)
                {
                    requirementItem.Parent_ID = null;//
                    Requirements.Add(requirementItem);
                }
                // If the selected item is in the root, put the new object in the root also
                else if (SelectedItem.Parent_ID == null)
                {
                    requirementItem.Parent_ID = null;
                    Requirements.Insert(Requirements.IndexOf(SelectedItem) + 1, requirementItem);
                }
                // Otherwise het the parent object and add the new object as a child
                else
                {
                    RequirementModel parentItem = GetRequirement(SelectedItem.Parent_ID);
                    requirementItem.Parent_ID = SelectedItem.Parent_ID;
                    parentItem.ChildRequirements.Insert(parentItem.ChildRequirements.IndexOf(SelectedItem) + 1, requirementItem);
                }
                IsChanged = true;
            }
        }

        private bool CanAddChild()
        {
            return true;
        }

        public void AddChild()
        {
            RequirementModel requirementItem = new RequirementModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                ArticleHeader = "New Requirement",
                Version = "",
                RequirementType_ID = 1,
                IsChanged = false,
                IsNew = true,
                ChildRequirements = new ObservableCollectionWithItemChanged<RequirementModel>()
            };
            if (SelectedItem != null)
            {
                requirementItem.Parent_ID = SelectedItem.ID;
                SelectedItem.ChildRequirements.Add(requirementItem);
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

        public RequirementModel GetRequirement(Guid? searchItemID, ObservableCollectionWithItemChanged<RequirementModel> treeLevel = null)
        {
            // Select the root level if the treeLevel = null
            if (treeLevel == null) treeLevel = Requirements;
            foreach (var requirementItem in treeLevel)
            {
                // return the item if found on this level
                if (requirementItem.ID == searchItemID) return requirementItem;

                if (requirementItem.ChildRequirements != null)
                {
                    // Recursively call the method to find the item in the ChildObjects
                    RequirementModel childRequirementItem = GetRequirement(searchItemID, requirementItem.ChildRequirements);
                    if (childRequirementItem != null) return childRequirementItem;
                }
            }
            return null;
        }

        public void MoveSelection(TreeListViewRow destination)
        {
            if (destination != null)
            {
                RequirementModel destinationItem = (destination.DataContext) as RequirementModel;
                try
                {
                    // Setup a private collection with the selected items only. This is because the SelectedItems that are part of the view model collection
                    // will change as soon as we start removing and adding objects
                    ObservableCollectionWithItemChanged<RequirementModel> selectedItems = new ObservableCollectionWithItemChanged<RequirementModel>();
                    foreach (RequirementModel item in SelectedItems)
                    {
                        selectedItems.Add(item);
                    }

                    foreach (RequirementModel item in selectedItems)
                    {
                        // find the original parent of the object that's moved
                        RequirementModel parentSourceItem = GetRequirement(item.Parent_ID);

                        // If the parent is in the root level
                        if (parentSourceItem == null)
                            // Remove the item in the root level
                            Requirements.Remove(item);
                        else
                            // Otherwise remove the item from the child collection
                            parentSourceItem.ChildRequirements.Remove(item);

                        TreeListViewDropPosition relativeDropPosition = (TreeListViewDropPosition)destination.GetValue(RadTreeListView.DropPositionProperty);

                        // If put on top of destination
                        if (relativeDropPosition == TreeListViewDropPosition.Inside)
                        {
                            // the Parent_ID of the item will become the ID of the destination
                            item.Parent_ID = destinationItem.ID;
                            destinationItem.ChildRequirements.Add(item);
                        }
                        // If put before or after the destination
                        else
                        {
                            // if the desitination is in the root collection
                            if (destinationItem.Parent_ID == null)
                            {
                                // The parent_ID of the item will also be null
                                item.Parent_ID = null;
                                Requirements.Insert(Requirements.IndexOf(destinationItem), item);
                            }
                            else
                            {
                                // otherwise the Parent_ID of the item will be the same as that of the destination item
                                item.Parent_ID = destinationItem.Parent_ID;
                                // find the Parent of the destination item
                                parentSourceItem = GetRequirement(destinationItem.Parent_ID);
                                // Insert the item above the destination item in the ChildObject collection of the parent of the destination
                                if (relativeDropPosition == TreeListViewDropPosition.Before)
                                    parentSourceItem.ChildRequirements.Insert(parentSourceItem.ChildRequirements.IndexOf(destinationItem), item);
                                else
                                    parentSourceItem.ChildRequirements.Insert(parentSourceItem.ChildRequirements.IndexOf(destinationItem) + 1, item);
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

        private int GetRequirementType_ID(string requirementType)
        {
            RequirementTypeModel requirementTypeItem = RequirementTypes.Single(x => x.RequirementType == requirementType);
            return requirementTypeItem.ID;
        }

        private bool CanChangeRequirementType()
        {
            return true;
        }

        private void ChangeRequirementType(object p)
        {
            selectedItems[0].RequirementType_ID = Convert.ToInt32(p.ToString());
        }
        #endregion
    }
}
