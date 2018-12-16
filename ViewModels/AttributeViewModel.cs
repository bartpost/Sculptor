using Sculptor.EDBEntityDataModel;
using Sculptor.DataModels;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Telerik.Windows.Controls;
using System;

namespace Sculptor
{
    public class AttributeViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ObservableCollectionWithItemChanged<AttributeModel> attributes = new ObservableCollectionWithItemChanged<AttributeModel>();
        private ObservableCollectionWithItemChanged<AttributeModel> backgroundAttributes = new ObservableCollectionWithItemChanged<AttributeModel>();
        private AttributeModel selectedItem;
        private ObservableCollectionWithItemChanged<AttributeModel> selectedItems;
        private bool isChanged;
        private bool isBusy;
        private ICommand refreshCommand;
        private ICommand saveCommand;
        private ICommand addCommand;
        private ICommand deleteCommand;

        #region Constructor
        public AttributeViewModel()
        {
            // Load the root objects
            Load();
        }
        #endregion

        #region Properties
        public ObservableCollectionWithItemChanged<AttributeModel> Attributes
        {
            get
            { 
                return attributes;
            }
            set
            {
                attributes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionWithItemChanged<AttributeModel> BackgroundAttributes
        {
            get
            {
                return backgroundAttributes;
            }
            set
            {
                backgroundAttributes = value;
                OnPropertyChanged();
            }
        }

        public AttributeModel SelectedItem
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

        public ObservableCollectionWithItemChanged<AttributeModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<AttributeModel>();
                }
                return selectedItems;
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

        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                {
                    addCommand = new RelayCommand(
                        p => this.CanAdd(),
                        p => this.Add());
                }
                return addCommand;
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
            this.IsBusy = true;
            Load();
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            this.IsBusy = false;

            Attributes = BackgroundAttributes;
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Load all object types defined in tblObjectTypes table (This is project independent
        /// </summary>
        public void Load()
        {
            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblAttribute Rec in (from a in eDB.tblAttributes where (a.Project_ID == Globals.Project_ID) orderby a.Attribute select a))
                {
                    AttributeModel attributeItem = new AttributeModel
                    {
                        ID = Rec.ID,
                        Project_ID = Rec.Project_ID,
                        Attribute = Rec.Attribute,
                        Description = Rec.Description,
                        IsDeleted = false
                    };
                    Attributes.Add(attributeItem);
                }
            }
        }

        private bool CanAdd()
        {
            return SelectedItem != null;
        }

        public void Add()
        {
            AttributeModel attributeItem = new AttributeModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                Attribute = "New Attribute",
                Description = "New Attribute Description",
                IsChanged = false,
                IsNew = true,
            };
            Attributes.Add(attributeItem);
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
            SaveLevel(Attributes, eDB);

            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert("Fault while saving attributes: " + ex.Message);
            }

            IsChanged = false;
        }

        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void SaveLevel(ObservableCollectionWithItemChanged<AttributeModel> treeLevel, EDBEntities eDB)
        {
            if (treeLevel != null)
            {
                foreach (var attributeItem in treeLevel)
                {

                    if (attributeItem.IsNew)
                    {
                        tblAttribute NewRec = new tblAttribute();
                        var Rec = eDB.tblAttributes.Add(NewRec);
                        Rec.ID = attributeItem.ID;
                        Rec.Attribute = attributeItem.Attribute;
                        Rec.Description = attributeItem.Description;
                        Rec.Project_ID = Globals.Project_ID;
                        attributeItem.IsNew = false;
                    }
                    if (attributeItem.IsChanged)
                    {
                        tblAttribute Rec = eDB.tblAttributes.Where(o => o.ID == attributeItem.ID).FirstOrDefault();
                        Rec.Attribute = attributeItem.Attribute;
                        Rec.Description = attributeItem.Description;
                        Rec.Project_ID = attributeItem.Project_ID;
                        attributeItem.IsChanged = false;
                    }
                    if (attributeItem.IsDeleted)
                    {
                        tblAttribute Rec = eDB.tblAttributes.Where(o => o.Attribute == attributeItem.Attribute).FirstOrDefault();
                        if (Rec != null)
                            eDB.tblAttributes.Remove(Rec);
                    }
                }
            }
        }

        private bool CanRefresh()
        {
            return true;
        }

        public void Refresh()
        {
            Attributes.Clear();
            Load();
        }
        #endregion
    }

}
