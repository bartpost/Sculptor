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
using System.Windows.Data;

namespace Sculptor.ViewModels
{
    public class TypeViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region private declarations
        private ObservableCollection<TypeModel> objectTypes = new ObservableCollection<TypeModel>();
        private ObservableCollection<TypeModel> backgroundObjectTypes = new ObservableCollection<TypeModel>();
        private ObservableCollection<TypeModel> templateTypes = new ObservableCollection<TypeModel>();
        private ObservableCollection<TypeModel> backgroundTemplateTypes = new ObservableCollection<TypeModel>();
        private ObservableCollection<TypeModel> propertyTypes = new ObservableCollection<TypeModel>();
        private ObservableCollection<TypeModel> backgroundPropertyTypes = new ObservableCollection<TypeModel>();
        private ObservableCollection<TypeModel> requirementTypes = new ObservableCollection<TypeModel>();
        private ObservableCollection<TypeModel> backgroundRequirementTypes = new ObservableCollection<TypeModel>();
        private ObservableCollection<TypeModel> types = new ObservableCollection<TypeModel>();
        private ObservableCollection<TypeModel> backgroundTypes = new ObservableCollection<TypeModel>();
        private TypeModel selectedItem;
        private ObservableCollectionWithItemChanged<TypeModel> selectedItems;
        private CollectionViewSource filteredTypes;
        private bool isChanged;
        private bool isObjectTypePopupOpen;
        private bool isTemplateTypePopupOpen;
        private bool isPropertyTypePopupOpen;
        private bool isRequirementTypePopupOpen;
        private string typeGroup;
        private ICommand saveCommand;
        private ICommand addCommand;
        private ICommand deleteCommand;
        private ICommand changeTypeCommand;
        private ICommand cancelTypeCommand;
        private ICommand editTypeCommand;
        private ICommand addTypeCommand;
        private ICommand getImageFromFileCommand;
        #endregion

        #region Constructor
        public TypeViewModel()
        {
            // Load the objectTypes in the background
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.OnLoadInBackground;
            backgroundWorker.RunWorkerCompleted += OnLoadInBackgroundCompleted;
            backgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region Properties

        public ObservableCollection<TypeModel> ObjectTypes
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

        public ObservableCollection<TypeModel> BackgroundObjectTypes
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

        public ObservableCollection<TypeModel> TemplateTypes
        {
            get
            {
                return templateTypes;
            }
            set
            {
                templateTypes = value;
                OnPropertyChanged();
            }

        }

        public ObservableCollection<TypeModel> BackgroundTemplateTypes
        {
            get
            {
                return backgroundTemplateTypes;
            }
            set
            {
                backgroundTemplateTypes = value;
                OnPropertyChanged();
            }

        }

        public ObservableCollection<TypeModel> PropertyTypes
        {
            get
            {
                return propertyTypes;
            }
            set
            {
                propertyTypes = value;
                OnPropertyChanged();
            }

        }

        public ObservableCollection<TypeModel> BackgroundPropertyTypes
        {
            get
            {
                return backgroundPropertyTypes;
            }
            set
            {
                backgroundPropertyTypes = value;
                OnPropertyChanged();
            }

        }

        public ObservableCollection<TypeModel> RequirementTypes
        {
            get
            {
                return requirementTypes;
            }
            set
            {
                requirementTypes = value;
                OnPropertyChanged();
            }

        }

        public ObservableCollection<TypeModel> BackgroundRequirementTypes
        {
            get
            {
                return backgroundRequirementTypes;
            }
            set
            {
                backgroundRequirementTypes = value;
                OnPropertyChanged();
            }

        }

        public ObservableCollection<TypeModel> Types
        {
            get
            {
                return types;
            }
            set
            {
                types = value;
                OnPropertyChanged();
            }

        }

        public ObservableCollection<TypeModel> BackgroundTypes
        {
            get
            {
                return backgroundTypes;
            }
            set
            {
                backgroundTypes = value;
                OnPropertyChanged();
            }

        }

        public ObservableCollectionWithItemChanged<TypeModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<TypeModel>();
                }
                return selectedItems;
            }
        }

        public TypeModel SelectedItem
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

        public CollectionViewSource FilteredTypes
        {
            get { return filteredTypes; }
            set { filteredTypes = value; }
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

        public bool IsTemplateTypePopupOpen
        {
            get
            {
                return this.isTemplateTypePopupOpen;
            }
            set
            {
                if (value != this.isTemplateTypePopupOpen)
                {
                    this.isTemplateTypePopupOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPropertyTypePopupOpen
        {
            get
            {
                return this.isPropertyTypePopupOpen;
            }
            set
            {
                if (value != this.isPropertyTypePopupOpen)
                {
                    this.isPropertyTypePopupOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRequirementTypePopupOpen
        {
            get
            {
                return this.isRequirementTypePopupOpen;
            }
            set
            {
                if (value != this.isRequirementTypePopupOpen)
                {
                    this.isRequirementTypePopupOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TypeGroup
        {
            get
            {
                return this.typeGroup;
            }
            set
            {
                if (value != this.typeGroup)
                {
                    this.typeGroup = value;
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

        public ICommand CancelTypeCommand
        {
            get
            {
                if (cancelTypeCommand == null)
                {
                    cancelTypeCommand = new RelayCommand(
                        p => this.CanCancelType(),
                        p => this.CancelType(p));
                }
                return cancelTypeCommand;
            }
        }

        public ICommand EditTypeCommand
        {
            get
            {
                if (editTypeCommand == null)
                {
                    editTypeCommand = new RelayCommand(
                        p => this.CanEditType(),
                        p => this.EditType(p));
                }
                return editTypeCommand;
            }
        }

        public ICommand AddTypeCommand
        {
            get
            {
                if (addTypeCommand == null)
                {
                    addTypeCommand = new RelayCommand(
                        p => this.CanAddType(),
                        p => this.AddType(p));
                }
                return addTypeCommand;
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
            TemplateTypes = BackgroundTemplateTypes;
            PropertyTypes = BackgroundPropertyTypes;
            RequirementTypes = BackgroundRequirementTypes;
            Types = BackgroundTypes;
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
                foreach (tblType Rec in (from o in eDB.tblTypes where (o.Project_ID == Globals.Project_ID) orderby o.ShowOrder select o))
                {
                    //var xmlRec = pl.SingleOrDefault(x => x.ID == Rec.ID);
                    //if ((xmlRec) != null) expanded = xmlRec.IsExpanded;

                    TypeModel typeItem = new TypeModel
                    {
                        ID = Rec.ID,
                        Project_ID = Rec.Project_ID,
                        Type = Rec.Type,
                        Description = Rec.Description,
                        Image = Rec.Image,
                        ShowOrder = Rec.ShowOrder.GetValueOrDefault(999),
                        TypeGroup = Rec.TypeGroup,
                        IsChanged = false,
                    };

                    switch (typeItem.TypeGroup)
                    {
                        case "Object":
                            BackgroundObjectTypes.Add(typeItem);
                            break;
                        case "Template":
                            BackgroundTemplateTypes.Add(typeItem);
                            break;
                        case "Property":
                            BackgroundPropertyTypes.Add(typeItem);
                            break;
                        case "Requirement":
                            BackgroundRequirementTypes.Add(typeItem);
                            break;
                    }
                    BackgroundTypes.Add(typeItem);
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
            // ToDo: fix this mess
            foreach (var typeItem in ObjectTypes)
                SaveTypeItem(eDB, typeItem);
            foreach (var typeItem in TemplateTypes)
                SaveTypeItem(eDB, typeItem);
            foreach (var typeItem in PropertyTypes)
                SaveTypeItem(eDB, typeItem);
            foreach (var typeItem in RequirementTypes)
                SaveTypeItem(eDB, typeItem);
            try
            {
                eDB.SaveChanges();
            }
            catch (Exception ex)
            {
                RadWindow.Alert("Fault while saving object types: " + ex.Message);
            }
        }

        private void SaveTypeItem(EDBEntities eDB, TypeModel typeItem)
        {
            if (typeItem.IsNew)
            {
                tblType NewRec = new tblType();
                var Rec = eDB.tblTypes.Add(NewRec);
                Rec.Type = typeItem.Type;
                Rec.Description = typeItem.Description;
                Rec.Image = typeItem.Image;
                Rec.TypeGroup = typeItem.TypeGroup;
                Rec.ShowOrder = typeItem.ShowOrder;
                Rec.Project_ID = Globals.Project_ID;
                typeItem.IsNew = false;
                typeItem.IsChanged = false;
            }
            if (typeItem.IsChanged)
            {
                tblType Rec = eDB.tblTypes.Where(o => o.ID == typeItem.ID).FirstOrDefault();
                Rec.Type = typeItem.Type;
                Rec.Description = typeItem.Description;
                Rec.Image = typeItem.Image;
                Rec.ShowOrder = typeItem.ShowOrder;
                typeItem.IsChanged = false;
            }
            if (typeItem.IsDeleted)
            {
                tblType Rec = eDB.tblTypes.Where(o => o.ID == typeItem.ID).FirstOrDefault();
                if (Rec != null)
                    eDB.tblTypes.Remove(Rec);
            }
        }

        private int GetObjectType_ID(string objectType)
        {
            TypeModel typeItem = ObjectTypes.Single(x => x.Type == objectType);
            return typeItem.ID;
        }

        private TypeModel GetType(int searchItemID)
        {
            foreach (var typeItem in Types)
            {
                if (typeItem.ID == searchItemID) return typeItem;
            }
            return null;
        }

        private bool CanChangeType()
        {
            return true;
        }

        private void ChangeType(object p)
        {
            if (p != null)
            {
                TypeModel type = (TypeModel)p;
                // Change the type of the selected item of the type group
                switch (type.TypeGroup) 
                { 
                    case "Object":
                        foreach (var item in ObjectViewModelLocator.GetObjectVM().SelectedItems)
                            item.ObjectType_ID = type.ID;
                        IsObjectTypePopupOpen = false;
                        break;
                    case "Template":
                        foreach (var item in TemplateViewModelLocator.GetTemplateVM().SelectedItems)
                            item.TemplateType_ID = type.ID;
                        IsTemplateTypePopupOpen = false;
                        break;
                    case "Property":
                        foreach (var item in PropertyViewModelLocator.GetPropertyVM().SelectedItems)
                            item.PropertyType_ID = type.ID;
                        IsPropertyTypePopupOpen = false;
                        break;
                    case "Requirement":
                        foreach (var item in RequirementViewModelLocator.GetRequirementVM().SelectedItems)
                            item.RequirementType_ID = type.ID;
                        IsRequirementTypePopupOpen = false;
                        break;
                }

            }
        }

        private bool CanCancelType()
        {
            return true;
        }

        private void CancelType(object p)
        {
            var group = (p as string);
            switch (group)
            {
                case "Object":
                    IsObjectTypePopupOpen = false;
                    break;
                case "Template":
                    IsTemplateTypePopupOpen = false;
                    break;
                case "Property":
                    IsPropertyTypePopupOpen = false;
                    break;
                case "Requirement":
                    IsRequirementTypePopupOpen = false;
                    break;
            }

        }

        private bool CanEditType()
        {
            return true;
        }

        private void EditType(object p)
        {
            TypeGroup = p as string;
            var typeEditDialog = new TypeEditDialog();
            typeEditDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            switch (TypeGroup)
            {
                case "Object":
                    IsObjectTypePopupOpen = false;
                    typeEditDialog.TypeListView.ItemsSource = ObjectTypes;
                    typeEditDialog.ShowDialog();
                    break;
                case "Template":
                    IsTemplateTypePopupOpen = false;
                    typeEditDialog.TypeListView.ItemsSource = TemplateTypes;
                    typeEditDialog.ShowDialog();
                    break;
                case "Property":
                    IsPropertyTypePopupOpen = false;
                    typeEditDialog.TypeListView.ItemsSource = PropertyTypes;
                    typeEditDialog.ShowDialog();
                    break;
                case "Requirement":
                    IsRequirementTypePopupOpen = false;
                    typeEditDialog.TypeListView.ItemsSource = RequirementTypes;
                    typeEditDialog.ShowDialog();
                    break;

            }
        }

        private bool CanAddType()
        {
            return true;
        }

        private void AddType(object p)
        {
            BinaryToImage Converter = new BinaryToImage();
            TypeModel typeItem = new TypeModel()
            {
                Type = "New Type",
                Description = "Type Description",
                // Use the default image with question mark from resources 
                Image = (byte[])Converter.ConvertBack(Resources.NewItem, typeof(byte[]), null, null)
            };
            typeItem.IsNew = true;
            switch (TypeGroup)
            {
                case "Object":
                    ObjectTypes.Add(typeItem);
                    break;
                case "Template":
                    TemplateTypes.Add(typeItem);
                    break;
                case "Property":
                    PropertyTypes.Add(typeItem);
                    break;
                case "Requirement":
                    RequirementTypes.Add(typeItem);
                    break;
            }
            Types.Add(typeItem);
        }

        public int GetTypeGroupID(string TypeGroup)
        {
            return Types.FirstOrDefault(t => t.TypeGroup == TypeGroup).ID;
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
                    TypeModel otm = GetType((int)p);
                    otm.Image = image;
                }
                fldlg = null;
            }
            catch (Exception ex)
            {
                RadWindow.Alert(ex.Message.ToString());
            }
        }
        #endregion
    }
}