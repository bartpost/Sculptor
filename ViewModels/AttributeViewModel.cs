using Sculptor.EDBEntityDataModel;
using Sculptor.DataModels;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Telerik.Windows.Controls;
using System;
using TD = Telerik.Windows.Data;

namespace Sculptor
{
    public class AttributeViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Constructor
        public AttributeViewModel()
        {
            //Load the objects in the background
            //var backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += this.OnLoadInBackground;
            //backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            //backgroundWorker.RunWorkerAsync();
            Load();
        }
        #endregion

        #region Properties
        private TD.ObservableItemCollection<AttributeModel> attributes = new TD.ObservableItemCollection<AttributeModel>();
        public TD.ObservableItemCollection<AttributeModel> Attributes
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

        private AttributeModel selectedItem;
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

        private TD.ObservableItemCollection<AttributeModel> selectedItems;
        public TD.ObservableItemCollection<AttributeModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new TD.ObservableItemCollection<AttributeModel>();
                }
                return selectedItems;
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
            get
            {
                return this.IsLoaded;
            }
            set
            {
                if (value != this.IsLoaded)
                {
                    this.IsLoaded = value;
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
        #endregion

        #region Commands

        private ICommand refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                    refreshCommand = new RelayCommand(p => this.CanRefresh(), p => this.Refresh());
                return refreshCommand;
            }
        }

        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new RelayCommand(p => this.CanSave(), p => this.Save());
                 return saveCommand;
            }
        }

        private ICommand addCommand;
        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                     addCommand = new RelayCommand(p => this.CanAdd(), p => this.Add());
                return addCommand;
            }
        }

        private ICommand deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)

                    deleteCommand = new RelayCommand(p => this.CanDelete(), p => this.Delete());
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
            //Attributes.SuspendNotifications();
            Load();
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            //Attributes.ResumeNotifications();
            this.IsBusy = false;
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
            foreach (var item in SelectedItems)
                Attributes.Remove(item);
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
        private void SaveLevel(TD.ObservableItemCollection<AttributeModel> treeLevel, EDBEntities eDB)
        {
            try
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
            catch (Exception ex)
            {
                RadWindow.Alert("Fault while saving to database: " + ex.Message);
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
            PropertyViewModelLocator.GetPropertyVM().LoadCBAttributes();
        }
        #endregion
    }

}
