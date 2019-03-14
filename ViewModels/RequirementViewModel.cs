using Sculptor.DataModels;
using Sculptor.EDBEntityDataModel;
using Sculptor.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;
using Telerik.Windows.Documents.FormatProviders.Xaml;
using Telerik.Windows.Documents.Model;
using TD = Telerik.Windows.Data;

namespace Sculptor.ViewModels
{
    public class RequirementViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Constructor
        public RequirementViewModel()
        {
            // Load the requirements in the background
            //var backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += this.OnLoadInBackground;
            //backgroundWorker.RunWorkerCompleted += this.OnLoadInBackgroundCompleted;
            //backgroundWorker.RunWorkerAsync();
            Load(null);
        }
        #endregion

        #region Properties

        private TD.ObservableItemCollection<RequirementModel> requirements = new TD.ObservableItemCollection<RequirementModel>();
        public TD.ObservableItemCollection<RequirementModel> Requirements
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

        private TD.ObservableItemCollection<RequirementModel> selectedItems;
        public TD.ObservableItemCollection<RequirementModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new TD.ObservableItemCollection<RequirementModel>();
                }
                return selectedItems;
            }
        }

        private CollectionViewSource filteredRequirements;
        public CollectionViewSource FilteredRequirements
        {
            get { return filteredRequirements; }
            set
            {
                if (value != filteredRequirements)
                {
                    filteredRequirements = value;
                    OnPropertyChanged();
                }
            }
        }

        private RequirementModel selectedItem;
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
                    OnPropertyChanged();
                }
            }
        }

        private bool isChanged;
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

        private ICommand showArticleCommand;
        public ICommand ShowArticleCommand
        {
            get
            {
                if (showArticleCommand == null)
                    showArticleCommand = new RelayCommand(p => this.CanShowArticle(), p => this.ShowArticle());
                return showArticleCommand;
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
            IsBusy = true;
            IsLoaded = false;
            Requirements.SuspendNotifications();
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
            backgroundWorker.RunWorkerCompleted -= this.OnLoadInBackgroundCompleted;

            // CollectionView to filter the TreeListView
            // Note: Data is manipulated in the Objects collection
            FilteredRequirements = new CollectionViewSource { Source = Requirements };
            FilteredRequirements.Filter += RequirementFilter;

            Requirements.ResumeNotifications();
            //LoadTreeState();
            IsLoaded = true;
            this.IsBusy = false;
        }
        #endregion

        #region Methods
        private TD.ObservableItemCollection<RequirementModel> Load(Guid? Parent_ID)
        {
            TD.ObservableItemCollection<RequirementModel> childRequirements = new TD.ObservableItemCollection<RequirementModel>();

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
                        Requirements.Add(requirementItem);
                    else
                        childRequirements.Add(requirementItem);

                }
            }
            IsChanged = false;
            return childRequirements;
        }


        public void Refresh()
        {
            // For some reason we have to unselect the selected row before we clear the ItemSource
            SelectedItem = null;

            Requirements.Clear();
            Load(null);
        }

        public void Save()
        {
            EDBEntities eDB = new EDBEntities();

            // To determine which items have been deleted in the collection, get all requirements of the project stored in the database table first
            var tblRequirements = eDB.tblRequirements.Where(p => p.Project_ID == Globals.Project_ID);

            // Check if each requirement of the table exists in the requirements collection
            // if not, delete the requirement in the table
            foreach (var requirementRec in tblRequirements)
            {
                var requirementItem = GetRequirement(requirementRec.ID);
                if (requirementItem == null) // requirement not found in collection
                    eDB.tblRequirements.Remove(requirementRec);
            }

            // Add and update requirements recursively
            SaveLevel(Requirements, eDB);
            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Fault while saving requirements:\n" + ex.Message
                });
            }
            IsChanged = false;
            SaveTreeState();
        }

        private void SaveLevel(ObservableCollection<RequirementModel> treeLevel, EDBEntities eDB)
        {
            try
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
                        // Recursive call
                        if (requirementItem.ChildRequirements != null) SaveLevel(requirementItem.ChildRequirements, eDB);
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

        public void AddSibling()
        {
            {
                // Instanciate a new requirement
                RequirementModel requirementItem = new RequirementModel
                {
                    ID = Guid.NewGuid(),
                    Project_ID = Globals.Project_ID,
                    ArticleNo = "xxx",
                    ArticleHeader = "New Article",
                    Version = "",
                    IsChanged = false,
                    IsNew = true,
                    RequirementType_ID = TypeViewModelLocator.GetTypeVM().GetTypeGroupID("Requirement"),
                    ChildRequirements = new TD.ObservableItemCollection<RequirementModel>()
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

        public void AddChild()
        {
            RequirementModel requirementItem = new RequirementModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                ArticleNo = "xxx",
                ArticleHeader = "New Requirement",
                Version = "",
                IsChanged = false,
                IsNew = true,
                RequirementType_ID = TypeViewModelLocator.GetTypeVM().GetTypeGroupID("Requirement"),
                ChildRequirements = new TD.ObservableItemCollection<RequirementModel>()
            };
            if (SelectedItem != null)
            {
                requirementItem.Parent_ID = SelectedItem.ID;
                SelectedItem.ChildRequirements.Add(requirementItem);
            }
            IsChanged = true;
        }

        private void Delete()
        {
            // ToDo: Deleting items in the collection only works using the Del key for now. 
            // Implement delete method to also provide option using context menu
        }

        public RequirementModel GetRequirement(Guid? searchItemID, TD.ObservableItemCollection<RequirementModel> treeLevel = null)
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
                    TD.ObservableItemCollection<RequirementModel> selectedItems = new TD.ObservableItemCollection<RequirementModel>();
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

        public void RequirementFilter(object sender, FilterEventArgs e)
        {
            if (e.Item != null)
                e.Accepted = (e.Item as RequirementModel).IsDeleted == false;
        }

        private void ChangeType(object p)
        {
            // ToDo: Bad practice to call a view from the viewmodel. Fix using IOC
            var typeSelectionPopup = new TypeSelectionPopup();
            typeSelectionPopup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            TypeViewModel typeViewModel = TypeViewModelLocator.GetTypeVM();
            // Close the type selection box
            typeViewModel.CloseTrigger = false;
            typeViewModel.TypeGroup = "Requirement";
            // Filter the type collection on the type group
            typeViewModel.FilterText = typeViewModel.TypeGroup;
            // To have one popup for all type groups (object, template, property etc) the popup is embedded in a dialog
            typeSelectionPopup.ShowDialog();
        }

        private bool CanShowArticle()
        {
            return true;
        }

        private void ShowArticle()
        {
            StackPanel sp = new StackPanel();
            RadRichTextBox rtb = new RadRichTextBox();

            var reqItem = RequirementViewModelLocator.GetRequirementVM().GetRequirement(SelectedItem.ID, null);
            XamlFormatProvider provider = new XamlFormatProvider();
            RadDocument document;
            if (reqItem.Content != null)
                document = provider.Import(reqItem.Content);
            else
                document = new RadDocument();
            rtb.Document = document;
            rtb.Document.LayoutMode = DocumentLayoutMode.Flow;
            sp.Children.Add(rtb);

            var window = new RadWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Header = SelectedItem.ArticleHeader,
                Content = sp,
                Width = 700,
                Height = 500,
            };
            window.Show();
        }

        private void LoadTreeState()
        {
            TD.ObservableItemCollection<RequirementModel> isExpandedCollection;

            XmlSerializer x = new XmlSerializer(typeof(TD.ObservableItemCollection<RequirementModel>));
            //ToDo: put filename in configuration
            var xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sculptor\\" + Globals.ContractNo + "_RequirementExpandedState.xml");
            if (File.Exists(xmlFileName))
            {
                try
                {
                    using (var stream = new FileStream(xmlFileName, FileMode.Open))
                    {
                        isExpandedCollection = x.Deserialize(stream) as TD.ObservableItemCollection<RequirementModel>;
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

        private void LoadTreeStateRecursive(TD.ObservableItemCollection<RequirementModel> isExpandedCollectionLevel)
        {
            foreach (var item in isExpandedCollectionLevel)
            {
                var requirementItem = GetRequirement(item.ID);
                if (requirementItem != null)
                    requirementItem.IsExpanded = item.IsExpanded;

                if (requirementItem.ChildRequirements.Count != 0)
                    LoadTreeStateRecursive(item.ChildRequirements);
            }

        }

        private void SaveTreeState()
        {
            XmlSerializer x = new XmlSerializer(typeof(TD.ObservableItemCollection<RequirementModel>));
            //ToDo: put filename in configuration
            var xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sculptor\\" + Globals.ContractNo + "_RequirementExpandedState.xml");
            try
            {
                using (StreamWriter sw = new StreamWriter(xmlFileName))
                {
                    x.Serialize(sw, Requirements);
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
