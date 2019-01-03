using Sculptor.EDBEntityDataModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Sculptor
{
    public class AspectViewModel
    {
        #region Constructor
        public AspectViewModel()
        {
            // Load the objects in the background
            //var backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += OnLoadInBackground;
            //backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            //backgroundWorker.RunWorkerAsync();
            Load();
        }
        #endregion

        #region Properties
        private ObservableCollectionWithItemChanged<AspectModel> aspects = new ObservableCollectionWithItemChanged<AspectModel>();
        public ObservableCollectionWithItemChanged<AspectModel> Aspects
        {
            get
            { 
                return aspects;
            }
            set
            {
                aspects = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollectionWithItemChanged<AspectModel> backgroundAspects = new ObservableCollectionWithItemChanged<AspectModel>();
        public ObservableCollectionWithItemChanged<AspectModel> BackgroundAspects
        {
            get
            {
                return backgroundAspects;
            }
            set
            {
                backgroundAspects = value;
                OnPropertyChanged();
            }
        }

        private AspectModel selectedItem;
        public AspectModel SelectedItem
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

        private ObservableCollectionWithItemChanged<AspectModel> selectedItems;
        public ObservableCollectionWithItemChanged<AspectModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<AspectModel>();
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
                    saveCommand = new RelayCommand(p=> this.CanSave(), p => this.Save());
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
            Load();
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            this.IsBusy = false;

            Aspects = BackgroundAspects;
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
                foreach (tblAspect Rec in (from a in eDB.tblAspects where (a.Project_ID == Globals.Project_ID ) orderby a.AspectName select a))
                {
                    AspectModel aspectItem = new AspectModel
                    {
                        ID = Rec.ID,
                        AspectName = Rec.AspectName,
                        Description = Rec.Description,
                        HardIO = Rec.HardIO.GetValueOrDefault(),
                        ExtIO = Rec.ExtIO.GetValueOrDefault(),
                        PLCTag = Rec.PLCTag.GetValueOrDefault(),
                        SCADATag = Rec.SCADATag.GetValueOrDefault(),
                        AlarmTag = Rec.AlarmTag.GetValueOrDefault(),
                        TrendTag = Rec.TrendTag.GetValueOrDefault(),
                        Note = Rec.Note
                    };
                    Aspects.Add(aspectItem);
                }
            }
        }

        private bool CanAdd()
        {
            return true;
        }

        public void Add()
        {
            // Instanciate a new object
            AspectModel aspectItem = new AspectModel
            {
                ID = Guid.NewGuid(),
                Project_ID = Globals.Project_ID,
                AspectName = "New Aspect",
                Description = "New Aspect Description",
                HardIO = false,
                ExtIO = false,
                PLCTag = false,
                SCADATag = false, 
                AlarmTag = false,
                TrendTag = false,
                Note = "",
                IsChanged = false,
                IsNew = true,
            };
            // If no item has been selected, put the object in the root of the tree
            Aspects.Add(aspectItem);
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
            SaveLevel(Aspects, eDB);
            try
            { 
                eDB.SaveChanges();
            }
            catch (Exception ex)
            { 
                RadWindow.Alert("Fault while saving aspects: " + ex.Message);
            }
            IsChanged = false;
        }

        private void SaveLevel(ObservableCollectionWithItemChanged<AspectModel> treeLevel, EDBEntities eDB)
        {
            if (treeLevel != null)
            {
                foreach (var aspectItem in treeLevel)
                {

                    if (aspectItem.IsNew)
                    {
                        tblAspect NewRec = new tblAspect();
                        var Rec = eDB.tblAspects.Add(NewRec);
                        Rec.ID = aspectItem.ID;
                        Rec.AspectName = aspectItem.AspectName;
                        Rec.Description = aspectItem.Description;
                        Rec.Project_ID = Globals.Project_ID;
                        Rec.HardIO = aspectItem.HardIO;
                        Rec.ExtIO = aspectItem.ExtIO;
                        Rec.PLCTag = aspectItem.PLCTag;
                        Rec.SCADATag = aspectItem.SCADATag;
                        Rec.AlarmTag = aspectItem.AlarmTag;
                        Rec.TrendTag = aspectItem.TrendTag;
                        Rec.Note = aspectItem.Note;
                        aspectItem.IsNew = false;
                    }
                    if (aspectItem.IsChanged)
                    {
                        tblAspect Rec = eDB.tblAspects.Where(o => o.ID == aspectItem.ID).FirstOrDefault();
                        Rec.AspectName = aspectItem.AspectName;
                        Rec.Description = aspectItem.Description;
                        Rec.Project_ID = Globals.Project_ID;
                        Rec.HardIO = aspectItem.HardIO;
                        Rec.ExtIO = aspectItem.ExtIO;
                        Rec.PLCTag = aspectItem.PLCTag;
                        Rec.SCADATag = aspectItem.SCADATag;
                        Rec.AlarmTag = aspectItem.AlarmTag;
                        Rec.TrendTag = aspectItem.TrendTag;
                        Rec.Note = aspectItem.Note;
                        aspectItem.IsChanged = false;
                    }
                    if (aspectItem.IsDeleted)
                    {
                        tblAspect Rec = eDB.tblAspects.Where(o => o.ID == aspectItem.ID).FirstOrDefault();
                        if (Rec != null)
                            eDB.tblAspects.Remove(Rec);
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
            Aspects.Clear();
            Load();
        }

        #endregion
    }

}
