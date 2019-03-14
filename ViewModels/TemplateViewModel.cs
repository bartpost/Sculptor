using Sculptor.EDBEntityDataModel;
using Sculptor.ViewModels;
using Sculptor.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;
using Telerik.Windows.Data;
using TD = Telerik.Windows.Data;

namespace Sculptor
{
    public class TemplateViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Constructor
        public TemplateViewModel()
        {
            // Load the templates in background
            //var backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += this.OnLoadInBackground;
            //backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            //backgroundWorker.RunWorkerAsync();
            Load(null);
        }
        #endregion

        #region Properties

        private TD.ObservableItemCollection<TemplateModel> templates = new TD.ObservableItemCollection<TemplateModel>();
        public TD.ObservableItemCollection<TemplateModel> Templates
        {
            get { return templates; }
            set
            {
                templates = value;
                OnPropertyChanged();
            }
        }

        private TD.ObservableItemCollection<TemplateModel> selectedItems;
        public TD.ObservableItemCollection<TemplateModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new TD.ObservableItemCollection<TemplateModel>();
                }
                return selectedItems;
            }
        }

        private TD.ObservableItemCollection<TemplateModel> copiedItems;
        public TD.ObservableItemCollection<TemplateModel> CopiedItems
        {
            get
            {
                if (copiedItems == null)
                {
                    copiedItems = new TD.ObservableItemCollection<TemplateModel>();
                }
                return copiedItems;
            }
        }

        private CollectionViewSource filteredTemplates;
        public CollectionViewSource FilteredTemplates
        {
            get { return filteredTemplates; }
            set
            {
                if (value != filteredTemplates)
                {
                    filteredTemplates = value;
                    OnPropertyChanged();
                }
            }
        }

        private TemplateModel selectedItem;
        public TemplateModel SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                if (value != this.selectedItem)
                {
                    this.selectedItem = value;
                    OnPropertyChanged();

                    //refresh the collection that holds the functionalities and requirements for the selected object. The collection is binded to the ObjectFunctionalityTree.
                    TemplateAssociationViewModel TemplateAssociationVM = TemplateAssociationViewModelLocator.GetTemplateAssociationVM();
                    if (TemplateAssociationVM.FilteredTemplateAssociations != null)
                        TemplateAssociationVM.FilteredTemplateAssociations.View.Refresh();
                    TemplateRequirementViewModel TemplateRequirementVM = TemplateRequirementViewModelLocator.GetTemplateRequirementVM();
                    if (TemplateRequirementVM.FilteredTemplateRequirements != null)
                        TemplateRequirementVM.FilteredTemplateRequirements.View.Refresh();
                }
            }
        }

        private bool isChanged;
        public bool IsChanged
        {
            get { return this.isChanged; }
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

        private void OnLoadInBackground(object sender, DoWorkEventArgs e)
        {
            IsBusy = true;
            Templates.SuspendNotifications();

            // Load Object Types;
            TypeViewModelLocator.GetTypeVM();
            // Load Objects
            Load(null);
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // CollectionView to filter the TreeListView
            // Note: Data is manipulated in the Templates collection
            FilteredTemplates = new CollectionViewSource { Source = Templates };
            FilteredTemplates.Filter += TemplateFilter;

            Templates.ResumeNotifications();
            //LoadTreeState();
            IsLoaded = true;
            this.IsBusy = false;

            //Dispose events
            var backgroundWorker = sender as BackgroundWorker;
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= this.OnLoadInBackgroundCompleted;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads the records from the DbSet into the ViewModel. This function designed for recursive use
        /// </summary>
        /// <param name="Project_ID"></param>
        /// <param name="Parent_ID"></param>
        /// <returns>Observable collection of VMObjects</returns>
        private TD.ObservableItemCollection<TemplateModel> Load(Guid? Parent_ID)
        {
            TD.ObservableItemCollection<TemplateModel> childTemplates = new TD.ObservableItemCollection<TemplateModel>();

            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblTemplate Rec in (from o in eDB.tblTemplates where (o.Project_ID == Globals.Project_ID && o.Parent_ID == Parent_ID) select o))
                {

                    TemplateModel templateItem = new TemplateModel
                    {
                        ID = Rec.ID,
                        Parent_ID = Rec.Parent_ID,
                        Project_ID = Rec.Project_ID,
                        TemplateName = Rec.TemplateName,
                        Description = Rec.Description,
                        TemplateType_ID = (int)Rec.TemplateType_ID,
                        IsChanged = false,
                        IsNew = false,
                        IsDeleted = false
                    };

                    templateItem.ChildTemplates = Load(Rec.ID);

                    //        // If the parent ID is null, this is a root template and needs to be added to the VM class
                    //        // Else it is a child object which needs to be added to the ChildTemplate list
                    if (Rec.Parent_ID == null)
                        Templates.Add(templateItem);
                    else
                        childTemplates.Add(templateItem);

                }
            }
            return childTemplates;
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddSibling()
        {
            TemplateModel templateItem = new TemplateModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                TemplateName = "New Template",
                Description = "New Template Description",
                IsChanged = false,
                IsNew = true,
                IsDeleted = false,
                TemplateType_ID = TypeViewModelLocator.GetTypeVM().GetTypeGroupID("Template"),
                ChildTemplates = new TD.ObservableItemCollection<TemplateModel>()
            };


            if (SelectedItem == null)
            {
                templateItem.Parent_ID = null;
                Templates.Add(templateItem);
            }
            else if (SelectedItem.Parent_ID == null)
            {
                templateItem.Parent_ID = null;
                Templates.Insert(Templates.IndexOf(SelectedItem) + 1, templateItem);
            }
            else
            {
                TemplateModel parentItem = GetTemplate(SelectedItem.Parent_ID);
                templateItem.Parent_ID = SelectedItem.Parent_ID;
                parentItem.ChildTemplates.Insert(parentItem.ChildTemplates.IndexOf(SelectedItem) + 1, templateItem);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void AddChild()
        {
            TemplateModel templateItem = new TemplateModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                TemplateName = "New Template",
                Description = "New Template Description",
                IsChanged = false,
                IsNew = true,
                IsDeleted = false,
                TemplateType_ID = TypeViewModelLocator.GetTypeVM().GetTypeGroupID("Template"),
                ChildTemplates = new TD.ObservableItemCollection<TemplateModel>()
            };
            if (SelectedItem != null)
            {
                templateItem.Parent_ID = SelectedItem.ID;
                SelectedItem.ChildTemplates.Add(templateItem);
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

            // To determine which items have been deleted in the collection, get all objects of the project stored in the database table first
            var tblTemplates = eDB.tblTemplates.Where(p => p.Project_ID == Globals.Project_ID);

            // Check if each template of the table exists in the templates collection
            // if not, delete the template in the table
            foreach (var templateRec in tblTemplates)
            {
                var templateItem = GetTemplate(templateRec.ID);
                if (templateItem == null) // template not found in collection
                    eDB.tblTemplates.Remove(templateRec);
            }

            // Add and update templates recursively
            SaveLevel(Templates, eDB);
            try
            { 
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters()
                {
                    Header = "Error",
                    Content = "Fault while saving templates:\n" + ex.Message
                });
            }
            SaveTreeState();
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(ObservableCollection<TemplateModel> treeLevel, EDBEntities eDB)
        {
            try
            {
                if (treeLevel != null)
                {
                    foreach (var templateItem in treeLevel)
                    {

                        if (templateItem.IsNew)
                        {
                            tblTemplate NewRec = new tblTemplate();
                            var Rec = eDB.tblTemplates.Add(NewRec);
                            Rec.ID = templateItem.ID;
                            Rec.Parent_ID = templateItem.Parent_ID;
                            Rec.TemplateName = templateItem.TemplateName;
                            Rec.Description = templateItem.Description;
                            Rec.TemplateType_ID = templateItem.TemplateType_ID;
                            Rec.Project_ID = templateItem.Project_ID;
                            templateItem.IsNew = false;
                        }
                        if (templateItem.IsChanged)
                        {
                            tblTemplate Rec = eDB.tblTemplates.Where(o => o.ID == templateItem.ID).FirstOrDefault();
                            Rec.Parent_ID = templateItem.Parent_ID;
                            Rec.TemplateName = templateItem.TemplateName;
                            Rec.Description = templateItem.Description;
                            Rec.TemplateType_ID = templateItem.TemplateType_ID;
                            Rec.Project_ID = templateItem.Project_ID;
                            templateItem.IsChanged = false;
                        }
                        // Recursive call
                        if (templateItem.ChildTemplates != null) SaveLevel(templateItem.ChildTemplates, eDB);
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

        private bool CanRefresh()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            Templates.Clear();
            Load(null);
            LoadTreeState();
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
            foreach (var templateItem in CopiedItems)
            {
                TemplateModel copiedTemplateItem = new TemplateModel();
                copiedTemplateItem.ID = Guid.NewGuid();
                copiedTemplateItem.Project_ID = templateItem.Project_ID;
                copiedTemplateItem.TemplateName = templateItem.TemplateName;
                copiedTemplateItem.Description = templateItem.Description;
                copiedTemplateItem.IsChanged = false;
                copiedTemplateItem.IsNew = true;
                copiedTemplateItem.ChildTemplates = new TD.ObservableItemCollection<TemplateModel>();

                if (SelectedItem == null)
                {
                    copiedTemplateItem.Parent_ID = null;
                    Templates.Add(copiedTemplateItem);
                }
                // Otherwise get the parent object and add the new template as a child
                else
                {
                    copiedTemplateItem.Parent_ID = SelectedItem.ID;
                    SelectedItem.ChildTemplates.Add(copiedTemplateItem);
                }
                //SelectedItem = copiedTemplateItem;
            }

            IsChanged = true;
        }

        /// <summary>
        /// Method triggered by the TreeListViewDragDropBehavior Template. Takes care of moving on item in the tree, which can be from 
        /// any level to any level
        /// </summary>
        /// <param name="destination"></param>
        public void MoveSelection(TreeListViewRow destination) 
        {
            if (destination != null)
            {
                TemplateModel destinationItem = (destination.DataContext) as TemplateModel;
                try
                {
                    // Setup a private collection with the selected items only. This is because the SelectedItems that are part of the view model collection
                    // will change as soon as we start removing and adding objects
                    TD.ObservableItemCollection<TemplateModel> selectedItems = new TD.ObservableItemCollection<TemplateModel>();
                    foreach (TemplateModel item in SelectedItems)
                    {
                        selectedItems.Add(item);
                    }

                    foreach (TemplateModel item in selectedItems)
                    {
                        // find the original parent of the object that's moved
                        TemplateModel parentSourceItem = GetTemplate(item.Parent_ID);

                        // If the parent is in the root level
                        if (parentSourceItem == null)
                            // Remove the item in the root level
                            Templates.Remove(item);
                        else
                            // Otherwise remove the item from the child collection
                            parentSourceItem.ChildTemplates.Remove(item);

                        TreeListViewDropPosition relativeDropPosition = (TreeListViewDropPosition)destination.GetValue(RadTreeListView.DropPositionProperty);

                        // If put on top of destination
                        if (relativeDropPosition == TreeListViewDropPosition.Inside)
                        {
                            // the Parent_ID of the item will become the ID of the destination
                            item.Parent_ID = destinationItem.ID;
                            destinationItem.ChildTemplates.Add(item);
                        }
                        // If put before or after the destination
                        else
                        {
                            // if the desitination is in the root collection
                            if (destinationItem.Parent_ID == null)
                            {
                                // The parent_ID of the item will also be null
                                item.Parent_ID = null;
                                Templates.Insert(Templates.IndexOf(destinationItem), item);
                            }
                            else
                            {
                                // otherwise the Parent_ID of the item will be the same as that of the destination item
                                item.Parent_ID = destinationItem.Parent_ID;
                                // find the Parent of the destination item
                                parentSourceItem = GetTemplate(destinationItem.Parent_ID);
                                // Insert the item above the destination item in the ChildObject collection of the parent of the destination
                                if (relativeDropPosition == TreeListViewDropPosition.Before)
                                    parentSourceItem.ChildTemplates.Insert(parentSourceItem.ChildTemplates.IndexOf(destinationItem), item);
                                else
                                    parentSourceItem.ChildTemplates.Insert(parentSourceItem.ChildTemplates.IndexOf(destinationItem) + 1, item);
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
        private Boolean FindObject(TD.ObservableItemCollection<TemplateModel> treeLevel, string searchItem)
        {
            if (treeLevel == null) treeLevel = Templates;
            foreach (var templateItem in treeLevel)
            {
                if (templateItem.TemplateName == searchItem) return true;
                if (templateItem.ChildTemplates != null)
                    if (FindObject(templateItem.ChildTemplates, searchItem)) return true;
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
        public TemplateModel GetTemplate(Guid? searchItemID, TD.ObservableItemCollection<TemplateModel> treeLevel = null)
        {
            // Select the root level if the treeLevel = null
            if (treeLevel == null) treeLevel = Templates;
            foreach (var templateItem in treeLevel)
            {
                // return the item if found on this level
                if (templateItem.ID == searchItemID) return templateItem;

                if (templateItem.ChildTemplates != null && templateItem.ChildTemplates.Count != 0)
                {
                    // Recursively call the method to find the item in the ChildProperties
                    TemplateModel childTemplateItem = GetTemplate(searchItemID, templateItem.ChildTemplates);
                    if (childTemplateItem != null) return childTemplateItem;
                }
            }
            return null;
        }

        private bool CanChangeType()
        {
            return true;
        }

        private void ChangeType(object p)
        {
            // ToDo: Bad practice to call a view from the viewmodel. Fix using IOC
            var typeSelectionPopup = new TypeSelectionPopup();
            typeSelectionPopup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            TypeViewModel typeViewModel = TypeViewModelLocator.GetTypeVM();
            // Close the type selection box
            typeViewModel.CloseTrigger = false;
            typeViewModel.TypeGroup = "Template";
            // Filter the type collection on the type group
            typeViewModel.FilterText = typeViewModel.TypeGroup;
            // To have one popup for all type groups (object, template, property etc) the popup is embedded in a dialog
            typeSelectionPopup.ShowDialog();
        }

        public void TemplateFilter(object sender, FilterEventArgs e)
        {
            if (e.Item != null)
                e.Accepted = (e.Item as TemplateModel).IsDeleted == false;
        }


        private void LoadTreeState()
        {
            TD.ObservableItemCollection<TemplateModel> isExpandedCollection;

            XmlSerializer x = new XmlSerializer(typeof(TD.ObservableItemCollection<TemplateModel>));
            //ToDo: put filename in configuration
            var xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sculptor\\" + Globals.ContractNo + "_TemplateExpandedState.xml");
            if (File.Exists(xmlFileName))
            {
                try
                {
                    using (var stream = new FileStream(xmlFileName, FileMode.Open))
                    {
                        isExpandedCollection = x.Deserialize(stream) as TD.ObservableItemCollection<TemplateModel>;
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

        private void LoadTreeStateRecursive(TD.ObservableItemCollection<TemplateModel> isExpandedCollectionLevel)
        {
            foreach (var item in isExpandedCollectionLevel)
            {
                var templateItem = GetTemplate(item.ID);
                if (templateItem != null)
                    templateItem.IsExpanded = item.IsExpanded;

                if (templateItem.ChildTemplates.Count != 0)
                    LoadTreeStateRecursive(item.ChildTemplates);
            }

        }

        private void SaveTreeState()
        {
            XmlSerializer x = new XmlSerializer(typeof(TD.ObservableItemCollection<TemplateModel>));
            //ToDo: put filename in configuration
            var xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sculptor\\" + Globals.ContractNo + "_TemplateExpandedState.xml");
            try
            {
                using (StreamWriter sw = new StreamWriter(xmlFileName))
                {
                    x.Serialize(sw, Templates);
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
