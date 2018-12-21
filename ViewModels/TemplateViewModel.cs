using Sculptor.EDBEntityDataModel;
using Sculptor.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;

namespace Sculptor
{
    public class TemplateViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ObservableCollectionWithItemChanged<TemplateModel> templates;
        private ObservableCollectionWithItemChanged<TemplateModel> backgroundTemplates = new ObservableCollectionWithItemChanged<TemplateModel>();
        private ObservableCollection<TemplateTypeModel> templateTypes = new ObservableCollection<TemplateTypeModel>();
        private TemplateModel selectedItem;
        private ObservableCollectionWithItemChanged<TemplateModel> selectedItems;
        private ObservableCollectionWithItemChanged<TemplateModel> copiedItems;
        private bool isChanged;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand addSiblingCommand;
        private ICommand addChildCommand;
        private ICommand deleteCommand;
        private ICommand changeTypeCommand;
        private ICommand cutCommand;
        private ICommand copyCommand;
        private ICommand pasteCommand;

        #region Constructor
        public TemplateViewModel()
        {
            // Load the templates in background
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            backgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Properties

        public ObservableCollectionWithItemChanged<TemplateModel> Templates
        {
            get
            {
                return templates;
            }
            set
            {
                templates = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionWithItemChanged<TemplateModel> BackgroundTemplates
        {
            get
            {
                return backgroundTemplates;
            }
            set
            {
                backgroundTemplates = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionWithItemChanged<TemplateModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<TemplateModel>();
                }
                return selectedItems;
            }
        }

        public ObservableCollectionWithItemChanged<TemplateModel> CopiedItems
        {
            get
            {
                if (copiedItems == null)
                {
                    copiedItems = new ObservableCollectionWithItemChanged<TemplateModel>();
                }
                return copiedItems;
            }
        }

        public ObservableCollection<TemplateTypeModel> TemplateTypes
        {
            get
            {
                if (templateTypes == null)
                {
                    templateTypes = new ObservableCollection<TemplateTypeModel>();
                }
                return templateTypes;
            }

        }

        public TemplateModel SelectedItem
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
                    TemplateAssociationViewModel TemplateAssociationVM = TemplateAssociationViewModelLocator.GetTemplateAssociationVM();
                    if (TemplateAssociationVM.FilteredTemplateAssociations != null)
                        TemplateAssociationVM.FilteredTemplateAssociations.View.Refresh();
                    TemplateRequirementViewModel TemplateRequirementVM = TemplateRequirementViewModelLocator.GetTemplateRequirementVM();
                    if (TemplateRequirementVM.FilteredTemplateRequirements != null)
                        TemplateRequirementVM.FilteredTemplateRequirements.View.Refresh();
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

        public ICommand ChangeTypeCommand
        {
            get
            {
                if (changeTypeCommand == null)
                {
                    changeTypeCommand = new RelayCommand(
                        p => this.CanChangeType(),
                        p => this.ChangeType(p));
                }
                return changeTypeCommand;
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

        private void OnLoadInBackground(object sender, DoWorkEventArgs e)
        {
            TypeViewModelLocator.GetTypeVM();
            Load(null);
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= this.OnLoadInBackgroundCompleted;

            Templates = BackgroundTemplates;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads the records from the DbSet into the ViewModel. This function designed for recursive use
        /// </summary>
        /// <param name="Project_ID"></param>
        /// <param name="Parent_ID"></param>
        /// <returns>Observable collection of VMObjects</returns>
        private ObservableCollectionWithItemChanged<TemplateModel> Load(Guid? Parent_ID)
        {
            ObservableCollectionWithItemChanged<TemplateModel> childTemplates = new ObservableCollectionWithItemChanged<TemplateModel>();

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

                    //        // If the parent ID is null, this is a root object and needs to be added to the VM class
                    //        // Else it is a child object which needs to be added to the childobjectlist
                    if (Rec.Parent_ID == null)
                        BackgroundTemplates.Add(templateItem);
                    else
                        childTemplates.Add(templateItem);

                }
            }
                return childTemplates;
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
                ChildTemplates = new ObservableCollectionWithItemChanged<TemplateModel>()
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

        private bool CanAddChild()
        {
            return true;
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
                ChildTemplates = new ObservableCollectionWithItemChanged<TemplateModel>()
            };
            if (SelectedItem != null)
            {
                templateItem.Parent_ID = SelectedItem.ID;
                SelectedItem.ChildTemplates.Add(templateItem);
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
            if (SelectedItem.ChildTemplates != null)
                foreach (var childItem in SelectedItem.ChildTemplates)
                {
                    childItem.IsChanged = false;
                    childItem.IsNew = false;
                    childItem.IsDeleted = true;
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
            EDBEntities eDB = new EDBEntities();
            SaveLevel(Templates, eDB);
            try
            { 
                eDB.SaveChanges();
            }
            catch (Exception ex)
            { 
                RadWindow.Alert("Fault while saving templates: " + ex.Message);
            }
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(ObservableCollection<TemplateModel> treeLevel, EDBEntities eDB)
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
                    if (templateItem.IsDeleted)
                    {
                        tblTemplate Rec = eDB.tblTemplates.Where(o => o.ID == templateItem.ID).FirstOrDefault();
                        if (Rec != null)
                            eDB.tblTemplates.Remove(Rec);
                    }
                    // Recursive call
                    if (templateItem.ChildTemplates != null) SaveLevel(templateItem.ChildTemplates, eDB);
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
            Templates.Clear();
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
            foreach (var templateItem in CopiedItems)
            {
                TemplateModel copiedTemplateItem = new TemplateModel();
                copiedTemplateItem.ID = Guid.NewGuid();
                copiedTemplateItem.Project_ID = templateItem.Project_ID;
                copiedTemplateItem.TemplateName = templateItem.TemplateName;
                copiedTemplateItem.Description = templateItem.Description;
                copiedTemplateItem.IsChanged = false;
                copiedTemplateItem.IsNew = true;
                copiedTemplateItem.ChildTemplates = new ObservableCollectionWithItemChanged<TemplateModel>();

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
                    ObservableCollectionWithItemChanged<TemplateModel> selectedItems = new ObservableCollectionWithItemChanged<TemplateModel>();
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
        private Boolean FindObject(ObservableCollection<TemplateModel> treeLevel, string searchItem)
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
        public TemplateModel GetTemplate(Guid? searchItemID, ObservableCollectionWithItemChanged<TemplateModel> treeLevel = null)
        {
            // Select the root level if the treeLevel = null
            if (treeLevel == null) treeLevel = Templates;
            foreach (var templateItem in treeLevel)
            {
                // return the item if found on this level
                if (templateItem.ID == searchItemID) return templateItem;

                if (templateItem.ChildTemplates != null)
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
            TypeViewModelLocator.GetTypeVM().IsTemplateTypePopupOpen = true;
        }

    }
    #endregion

}
