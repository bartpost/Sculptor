using Sculptor.EDBEntityDataModel;
using Sculptor.Views;
using Sculptor.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TreeListView;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media;
using System.Drawing;
using System.IO;
using Sculptor.Properties;

namespace Sculptor.ViewModels
{
    public class ObjectTypeViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region private declarations
        private ObservableCollection<ObjectTypeModel> objectTypes = new ObservableCollection<ObjectTypeModel>();
        private ObservableCollection<ObjectTypeModel> backgroundObjectTypes = new ObservableCollection<ObjectTypeModel>();
        private ObjectTypeModel selectedItem;
        private ObservableCollectionWithItemChanged<ObjectTypeModel> selectedItems;
        private bool isChanged;
        private bool isObjectTypePopupOpen;
        private bool isEditObjectTypePopupOpen;
        private ICommand saveCommand;
        private ICommand addCommand;
        private ICommand deleteCommand;
        private ICommand selectObjectTypeCommand;
        private ICommand cancelObjectTypeCommand;
        private ICommand editObjectTypeCommand;
        private ICommand addObjectTypeCommand;
        private ICommand getImageFromFileCommand;
        #endregion

        #region Constructor
        public ObjectTypeViewModel()
        {
            // Load the objectTypes in the background
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            backgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Properties

        public ObservableCollection<ObjectTypeModel> ObjectTypes
        {
            get
            {
                    return objectTypes;
            }
            set
            {
                    objectTypes = value;
                    OnPropertyChanged();
            }

        }

        public ObservableCollection<ObjectTypeModel> BackgroundObjectTypes
        {
            get
            {
                return backgroundObjectTypes;
            }
            set
            {
                backgroundObjectTypes = value;
                OnPropertyChanged();
            }

        }

        public ObservableCollectionWithItemChanged<ObjectTypeModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<ObjectTypeModel>();
                }
                return selectedItems;
            }
        }


        public ObjectTypeModel SelectedItem
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
                    OnPropertyChanged();
                }
            }
        }


        public bool IsObjectTypePopupOpen
        {
            get
            {
                return this.isObjectTypePopupOpen;
            }
            set
            {
                if (value != this.isObjectTypePopupOpen)
                {
                    this.isObjectTypePopupOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsEditObjectTypePopupOpen
        {
            get
            {
                return this.isEditObjectTypePopupOpen;
            }
            set
            {
                if (value != this.isEditObjectTypePopupOpen)
                {
                    this.isEditObjectTypePopupOpen = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Commands

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

        public ICommand SelectObjectTypeCommand
        {
            get
            {
                if (selectObjectTypeCommand == null)
                {
                    selectObjectTypeCommand = new RelayCommand(
                        p => this.CanSelectObjectType(),
                        p => this.SelectObjectType(p));
                }
                return selectObjectTypeCommand;
            }
        }

        public ICommand CancelObjectTypeCommand
        {
            get
            {
                if (cancelObjectTypeCommand == null)
                {
                    cancelObjectTypeCommand = new RelayCommand(
                        p => this.CanCancelObjectType(),
                        p => this.CancelObjectType(p));
                }
                return cancelObjectTypeCommand;
            }
        }

        public ICommand EditObjectTypeCommand
        {
            get
            {
                if (editObjectTypeCommand == null)
                {
                    editObjectTypeCommand = new RelayCommand(
                        p => this.CanEditObjectType(),
                        p => this.EditObjectType(p));
                }
                return editObjectTypeCommand;
            }
        }

        public ICommand AddObjectTypeCommand
        {
            get
            {
                if (addObjectTypeCommand == null)
                {
                    addObjectTypeCommand = new RelayCommand(
                        p => this.CanAddObjectType(),
                        p => this.AddObjectType(p));
                }
                return addObjectTypeCommand;
            }
        }

        public ICommand GetImageFromFileCommand
        {
            get
            {
                if (getImageFromFileCommand == null)
                {
                    getImageFromFileCommand = new RelayCommand(
                        p => this.CanGetImageFromFile(),
                        p => this.GetImageFromFile(p));
                }
                return getImageFromFileCommand;
            }
        }

        #endregion

        #region Events
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<FocusRequestedEventArgs> FocusRequested;
        protected virtual void OnFocusRequested(string propertyName)
        {
            FocusRequested?.Invoke(this, new FocusRequestedEventArgs(propertyName));
        }

        private void OnLoadInBackground(object sender, DoWorkEventArgs e)
        {
            Load();
        }

        private void OnLoadInBackgroundCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            //Dispose events
            backgroundWorker.DoWork -= this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted -= OnLoadInBackgroundCompleted;

            ObjectTypes = BackgroundObjectTypes;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the records from the DbSet into the ViewModel. This function designed for recursive use
        /// </summary>
        /// <param name="Project_ID"></param>
        /// <param name="Parent_ID"></param>
        /// <returns>Observable collection of VMObjects</returns>
        private void Load()
        {
            using (EDBEntities eDB = new EDBEntities())
            {
                foreach (tblObjectType Rec in (from o in eDB.tblObjectTypes where (o.Project_ID == Globals.Project_ID) orderby o.ShowOrder select o))
                {
                    //var xmlRec = pl.SingleOrDefault(x => x.ID == Rec.ID);
                    //if ((xmlRec) != null) expanded = xmlRec.IsExpanded;

                    ObjectTypeModel objectTypeItem = new ObjectTypeModel
                    {
                        ID = Rec.ID,
                        Project_ID = Rec.Project_ID,
                        ObjectType = Rec.ObjectType,
                        Description = Rec.Description,
                        Image = Rec.Image,
                        ShowOrder = Rec.ShowOrder.GetValueOrDefault(999),
                        IsChanged = false,
                    };

                    BackgroundObjectTypes.Add(objectTypeItem);
                }
            }
            IsChanged = false;
        }

        private bool CanAdd()
        {
            return true;
        }

        public void Add()
        {

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

            foreach (var objectTypeItem in ObjectTypes)
            {

                if (objectTypeItem.IsNew)
                {
                    tblObjectType NewRec = new tblObjectType();
                    var Rec = eDB.tblObjectTypes.Add(NewRec);
                    Rec.ObjectType = objectTypeItem.ObjectType;
                    Rec.Description = objectTypeItem.Description;
                    Rec.Image = objectTypeItem.Image;
                    Rec.ShowOrder = objectTypeItem.ShowOrder;
                    Rec.Project_ID = Globals.Project_ID;
                    objectTypeItem.IsNew = false;
                    objectTypeItem.IsChanged = false;
                }
                if (objectTypeItem.IsChanged)
                {
                    tblObjectType Rec = eDB.tblObjectTypes.Where(o => o.ID == objectTypeItem.ID).FirstOrDefault();
                    Rec.ObjectType = objectTypeItem.ObjectType;
                    Rec.Description = objectTypeItem.Description;
                    Rec.Image = objectTypeItem.Image;
                    Rec.ShowOrder = objectTypeItem.ShowOrder;
                    objectTypeItem.IsChanged = false;
                }
                if (objectTypeItem.IsDeleted)
                {
                    tblObjectType Rec = eDB.tblObjectTypes.Where(o => o.ID == objectTypeItem.ID).FirstOrDefault();
                    if (Rec != null)
                        eDB.tblObjectTypes.Remove(Rec);
                }
            }
            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert("Fault while saving object types: " + ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        private int GetObjectType_ID(string objectType)
        {
            ObjectTypeModel objectTypeItem = ObjectTypes.Single(x => x.ObjectType == objectType);
            return objectTypeItem.ID;
        }

        private ObjectTypeModel GetObjectType(int searchItemID)
        {
            foreach (var objectItem in ObjectTypes)
            {
                if (objectItem.ID == searchItemID) return objectItem;
            }
            return null;
        }

        private bool CanSelectObjectType()
        {
            return true;
        }

        private void SelectObjectType(object p)
        {
            if (p != null)
            {
                foreach (var item in ObjectViewModelLocator.GetObjectVM().SelectedItems)
                    item.ObjectType_ID = (p as ObjectTypeModel).ID;
                IsObjectTypePopupOpen = false;
            }
        }

        private bool CanCancelObjectType()
        {
            return true;
        }

        private void CancelObjectType(object p)
        {
            IsObjectTypePopupOpen = false;
        }

        private bool CanEditObjectType()
        {
            return true;
        }

        private void EditObjectType(object p)
        {
            IsObjectTypePopupOpen = false;
            if (Convert.ToInt16(p) == 1)
            {
                var editObjectTypeDialog = new ObjectTypeView();
                editObjectTypeDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                editObjectTypeDialog.ShowDialog();
            }
        }

        private bool CanAddObjectType()
        {
            return true;
        }

        private void AddObjectType(object p)
        {
            BinaryToImage Converter = new BinaryToImage();
            ObjectTypeModel objectTypeItem = new ObjectTypeModel()
            {
                ObjectType = "New ObjectType",
                Description = "ObjectType Description",
                // Use the default image with question mark from resources 
                Image = (byte[])Converter.ConvertBack(Resources.NewItem, typeof(byte[]), null, null)
            };
            objectTypeItem.IsNew = true;
            objectTypes.Add(objectTypeItem);
        }

        private bool CanGetImageFromFile()
        {
            return true;
        }

        private void GetImageFromFile(object p)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                fldlg.Filter = "Image File (*.jpg;*.bmp;*.png)|*.jpg;*.bmp;*.png";
                fldlg.ShowDialog();
                {
                    byte[] image = File.ReadAllBytes(fldlg.FileName);
                    ObjectTypeModel otm = GetObjectType((int)p);
                    otm.Image = image;
                }
                fldlg = null;
            }
            catch (Exception ex)
            {
                RadWindow.Alert(ex.Message.ToString());
            }
        }

    }
    #endregion
    
}