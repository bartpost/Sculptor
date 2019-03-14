using Sculptor.EDBEntityDataModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TD = Telerik.Windows.Data;



namespace Sculptor
{
    public class HardIOViewModel
    {
        #region Constructor
        public HardIOViewModel()
        {
            Load();
        }
        #endregion

        #region Properties
        private TD.ObservableItemCollection<HardIOModel> hardIO = new TD.ObservableItemCollection<HardIOModel>();
        public TD.ObservableItemCollection<HardIOModel> HardIO
        {
            get
            {
                return hardIO;
            }
            set
            {
                hardIO = value;
                OnPropertyChanged();
            }
        }

        private HardIOModel selectedItem;
        public HardIOModel SelectedItem
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

        private TD.ObservableItemCollection<HardIOModel> selectedItems;
        public TD.ObservableItemCollection<HardIOModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new TD.ObservableItemCollection<HardIOModel>();
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
                return this.isLoaded;
            }
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
            //Aspects.SuspendNotifications();
            Load();
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            //Aspects.ResumeNotifications();
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
                foreach (var Rec in (from o in eDB.tblObjects
                                     join a in eDB.tblObjectAssociations on o.ID equals a.Object_ID
                                     join p in eDB.tblProperties on a.Association_ID equals p.ID
                                     join ap in eDB.tblAspects on p.Aspect equals ap.AspectName
                                     join t in eDB.tblTypes on o.ObjectType_ID equals t.ID
                                     where (o.Project_ID == Globals.Project_ID && ap.Project_ID == Globals.Project_ID && ap.HardIO == true)
                                     orderby o.ObjectName
                                     select new { ObjectName = o.ObjectName, Description = o.Description, PropertyName = p.PropertyName} ))
                {
                    HardIOModel hardIOItem = new HardIOModel
                    {
                        ObjectName = Rec.ObjectName,
                        Description = Rec.Description,
                        PropertyName = Rec.PropertyName
                    };
                    HardIO.Add(hardIOItem);
                }
            }
        }


        private bool CanRefresh()
        {
            return true;
        }

        public void Refresh()
        {
            HardIO.Clear();
            Load();
        }

        #endregion
    }

}
