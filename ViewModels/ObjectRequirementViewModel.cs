using Sculptor.DataModels;
using Sculptor.EDBEntityDataModel;
using Sculptor.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Media;
using Telerik.Windows.Controls.TreeListView;
using Telerik.Windows.Documents.FormatProviders.Xaml;
using Telerik.Windows.Documents.Model;

namespace Sculptor.ViewModels
{
    public class ObjectRequirementViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ObservableCollection<ObjectRequirementModel> objectRequirements;
        private ObservableCollection<ObjectRequirementModel> backgroundObjectRequirements = new ObservableCollection<ObjectRequirementModel>();
        private CollectionViewSource filteredObjectRequirements;
        private ObjectRequirementModel selectedItem;
        private ObservableCollection<ObjectRequirementModel> selectedItems;
        private bool isChanged;
        private ICommand showArticleCommand;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand deleteCommand;

        #region Constructor
        public ObjectRequirementViewModel()
        {
            //Load the properties in the background
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            backgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Properties

        public ObservableCollection<ObjectRequirementModel> ObjectRequirements
        {
            get
            {
                return objectRequirements;
            }
            set
            {
                objectRequirements = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ObjectRequirementModel> BackgroundObjectRequirements
        {
            get
            {
                return backgroundObjectRequirements;
            }
            set
            {
                backgroundObjectRequirements = value;
                OnPropertyChanged();
            }
        }

        public CollectionViewSource FilteredObjectRequirements
        {
            get
            {
                return filteredObjectRequirements;
            }
            set
            {
                if (value != filteredObjectRequirements)
                {
                    filteredObjectRequirements = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ObjectRequirementModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<ObjectRequirementModel>();
                }
                return selectedItems;
            }
        }

        public ObjectRequirementModel SelectedItem
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
        public ICommand ShowArticleCommand
        {
            get
            {
                if (showArticleCommand == null)
                {
                    showArticleCommand = new RelayCommand(
                        p => this.CanShowArticle(),
                        p => this.ShowArticle(p));
                }
                return showArticleCommand;
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
           TypeViewModelLocator.GetTypeVM();
           Load();
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= this.OnLoadInBackgroundCompleted;

            ObjectRequirements = BackgroundObjectRequirements;

            // Create a collection that holds the functionalities of the selected object and add the filter event handler
            // Note: the FilteredObjectFunctionalities collection is updated every time a new object is selected in the object tree 
            // (triggered in the setter of the SelectedItem property)
            FilteredObjectRequirements = new CollectionViewSource
            {
                Source = ObjectRequirements
            };
            FilteredObjectRequirements.Filter += ObjectFilter;
        }
        #endregion

        #region Methods

        private void Load()
        {
            // Check if the classes and properties have ben loaded
            while (!RequirementViewModelLocator.IsLoaded());

            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblObjectRequirement Rec in (from o in eDB.tblObjectRequirements where (o.Project_ID == Globals.Project_ID) select o))
                {

                    ObjectRequirementModel objectRequirementItem = new ObjectRequirementModel
                    {
                        Project_ID = Rec.Project_ID,
                        Object_ID = Rec.Object_ID,
                        Requirement_ID = Rec.Requirement_ID,
                        RequirementType = Rec.RequirementType,
                        PreFATOk = Rec.PreFATOk,
                        FATOk = Rec.FATOk,
                        FATBy = Rec.FATBy,
                        FATDate = Rec.FATDate,
                        SATOk = Rec.SATOk,
                        SATBy = Rec.SATBy,
                        SATDate = Rec.SATDate,
                        IsChanged = false,
                        IsNew = false,
                        IsDeleted = false,
                        ChildRequirements = new ObservableCollection<ObjectRequirementModel>()
                    };

                    var requirementItem = RequirementViewModelLocator.GetRequirementVM().GetRequirement(objectRequirementItem.Requirement_ID, null);

                    if (requirementItem != null)
                    {
                        objectRequirementItem.ArticleNo = requirementItem.ArticleNo;
                        objectRequirementItem.ArticleHeader = requirementItem.ArticleHeader;
                        objectRequirementItem.RequirementType_ID = requirementItem.RequirementType_ID;

                        foreach (var childItem in requirementItem.ChildRequirements)
                        {
                            ObjectRequirementModel item = new ObjectRequirementModel
                            {
                                Project_ID = childItem.Project_ID,
                                Object_ID = objectRequirementItem.Object_ID,
                                Requirement_ID = childItem.ID,
                                ArticleNo = childItem.ArticleNo,
                                ArticleHeader = childItem.ArticleHeader,
                                RequirementType = "Requirement",
                                ChildRequirements = new ObservableCollection<ObjectRequirementModel>(),
                                RequirementType_ID = childItem.RequirementType_ID
                            };
                            objectRequirementItem.ChildRequirements.Add(item);
                        }

                        BackgroundObjectRequirements.Add(objectRequirementItem);
                    }
                }
            }
        }

        private bool CanShowArticle()
        {
            return true;
        }

        private void ShowArticle(Object element)
        {
            StackPanel sp = new StackPanel();
            RadRichTextBox rtb = new RadRichTextBox();

            var reqItem = RequirementViewModelLocator.GetRequirementVM().GetRequirement(SelectedItem.Requirement_ID, null);
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

                Header = SelectedItem.ArticleHeader,
                Content = sp,
                Width = 700,
                Height = 500,
            };
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            Point mousePos = Mouse.GetPosition((IInputElement)element);
            window.Left = mousePos.X;
            window.Top = mousePos.Y;
            window.Show();
        }

        private bool CanDelete()
        {
            return true;
        }

        private void Delete()
        {
            if (SelectedItem != null)
            {
                SelectedItem.IsChanged = false;
                SelectedItem.IsNew = false;
                SelectedItem.IsDeleted = true;
                if (SelectedItem.ChildRequirements != null)
                    foreach (var childItem in SelectedItem.ChildRequirements)
                    {
                        childItem.IsChanged = false;
                        childItem.IsNew = false;
                        childItem.IsDeleted = true;
                    }

                // Because the FilteredObjectRequirements collection doesn't refresh on PropertyChanged, we have to refresh the collection
                FilteredObjectRequirements.View.Refresh();
            }
        }

        private bool CanSave()
        {
            return true;
        }

        public void Save()
        {

            EDBEntities eDB = new EDBEntities();
            SaveLevel(ObjectRequirements, eDB);
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

        private void SaveLevel(ObservableCollection<ObjectRequirementModel> treeLevel, EDBEntities eDB)
        {
            if (treeLevel != null)
            {
                foreach (var objectRequirementItem in treeLevel)
                {
                    if (objectRequirementItem.IsNew)
                    {
                        tblObjectRequirement NewRec = new tblObjectRequirement();
                        var Rec = eDB.tblObjectRequirements.Add(NewRec);
                        Rec.Object_ID = objectRequirementItem.Object_ID;
                        Rec.Requirement_ID = objectRequirementItem.Requirement_ID;
                        Rec.PreFATOk = objectRequirementItem.PreFATOk;
                        Rec.FATOk = objectRequirementItem.FATOk;
                        Rec.FATBy = objectRequirementItem.FATBy;
                        Rec.FATDate = objectRequirementItem.FATDate;
                        Rec.SATOk = objectRequirementItem.SATOk;
                        Rec.SATBy = objectRequirementItem.SATBy;
                        Rec.SATDate = objectRequirementItem.SATDate;
                        Rec.Project_ID = Globals.Project_ID;
                        //Rec.RequirementType_ID = objectRequirementItem.RequirementType_ID;
                        objectRequirementItem.IsNew = false;
                        RequirementViewModel requirementVM = RequirementViewModelLocator.GetRequirementVM();
                        RequirementModel requirementItem = requirementVM.GetRequirement(objectRequirementItem.Requirement_ID);
                    }
                    if (objectRequirementItem.IsChanged)
                    {
                        tblObjectRequirement Rec = eDB.tblObjectRequirements.Where(o => o.Object_ID == objectRequirementItem.Object_ID && o.Requirement_ID == objectRequirementItem.Requirement_ID).FirstOrDefault();
                        Rec.PreFATOk = objectRequirementItem.PreFATOk;
                        Rec.FATOk = objectRequirementItem.FATOk;
                        Rec.FATBy = objectRequirementItem.FATBy;
                        Rec.FATDate = objectRequirementItem.FATDate;
                        Rec.SATOk = objectRequirementItem.SATOk;
                        Rec.SATBy = objectRequirementItem.SATBy;
                        Rec.SATDate = objectRequirementItem.SATDate;
                        objectRequirementItem.IsChanged = false;
                    }
                    if (objectRequirementItem.IsDeleted)
                    {
                        tblObjectRequirement Rec = eDB.tblObjectRequirements.Where(o => (o.Object_ID == objectRequirementItem.Object_ID && o.Requirement_ID == objectRequirementItem.Requirement_ID)).FirstOrDefault();
                        if (Rec != null)
                            eDB.tblObjectRequirements.Remove(Rec);
                    }
                    // Recursive call
                }
            }
        }

        private bool CanRefresh()
        {
            return true;
        }

        public void Refresh()
        {
            ObjectRequirements.Clear();
            FilteredObjectRequirements.View.Refresh();
            Load();
        }

        public void AssociateWithObject(TreeListViewRow destination)
        {
            //if (destination != null)
            //{
            try
            {
                // Associate all selected objects
                foreach (var objectItem in ObjectViewModelLocator.GetObjectVM().SelectedItems)
                    // With all selected requirements (m:n)
                    foreach (var requirementItem in RequirementViewModelLocator.GetRequirementVM().SelectedItems)
                    {
                        ObjectRequirementModel objectRequirementItem = new ObjectRequirementModel
                        {
                            IsNew = true,
                            IsChanged = false,
                            IsDeleted = false,
                            Project_ID = Globals.Project_ID,
                            Object_ID = objectItem.ID,
                            Requirement_ID = requirementItem.ID,
                            RequirementType_ID = requirementItem.RequirementType_ID,
                            ArticleNo = requirementItem.ArticleNo,
                            ArticleHeader = requirementItem.ArticleHeader,
                            //RequirementType = Globals.DraggedItem.Type,
                            ChildRequirements = new ObservableCollection<ObjectRequirementModel>(),
                        };

                        foreach (var childItem in requirementItem.ChildRequirements)
                        {
                            ObjectRequirementModel objectRequirementChildItem = new ObjectRequirementModel
                            {
                                IsNew = true,
                                IsChanged = false,
                                IsDeleted = false,
                                Project_ID = Globals.Project_ID,
                                Object_ID = objectRequirementItem.Object_ID,
                                Requirement_ID = childItem.ID,
                                RequirementType_ID = childItem.RequirementType_ID,
                                //objectFunctionalityChildItem.FunctionParent_ID = childItem.Parent_ID;
                                ArticleNo = childItem.ArticleNo,
                                ArticleHeader = childItem.ArticleHeader,
                                ChildRequirements = new ObservableCollection<ObjectRequirementModel>()
                            };
                            objectRequirementItem.ChildRequirements.Add(objectRequirementChildItem);
                        };

                        ObjectRequirements.Add(objectRequirementItem);
                    }
            }
            catch (Exception ex)
            {
                RadWindow.Alert(ex.Message);
            }
        }

        public void ObjectFilter(object sender, FilterEventArgs e)
        {
            ObjectViewModel ovm = ObjectViewModelLocator.GetObjectVM();
            ObjectModel om = ovm.SelectedItem;
            if (e.Item != null && om != null)
                e.Accepted = (e.Item as ObjectRequirementModel).Object_ID == ObjectViewModelLocator.GetObjectVM().SelectedItem.ID &&
                             (e.Item as ObjectRequirementModel).IsDeleted == false;
        }
        #endregion
    }
}
