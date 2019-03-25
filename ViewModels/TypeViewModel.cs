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
using System.Windows;
using Microsoft.Win32;
using System.IO;
using Sculptor.Properties;
using System.Windows.Data;
using TD = Telerik.Windows.Data;

namespace Sculptor.ViewModels
{
    public class TypeViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Constructor
        public TypeViewModel()
        {
            Load();
        }
        #endregion

        #region Properties

        private TD.ObservableItemCollection<TypeModel> types = new TD.ObservableItemCollection<TypeModel>();
        public TD.ObservableItemCollection<TypeModel> Types
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

        private ObservableItemCollection<TypeModel> selectedItems;
        public ObservableItemCollection<TypeModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableItemCollection<TypeModel>();
                }
                return selectedItems;
            }
        }

        private TypeModel selectedItem;
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

        private CollectionViewSource filteredTypes;
        public CollectionViewSource FilteredTypes
        {
            get { return filteredTypes; }
            set { filteredTypes = value; }
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
                    OnPropertyChanged();
                }
            }
        }

        private bool closeTrigger;
        public bool CloseTrigger
        {
            get
            {
                return this.closeTrigger;
            }
            set
            {
                if (value != this.closeTrigger)
                {
                    this.closeTrigger = value;
                    OnPropertyChanged();
                }
            }
        }

        private string typeGroup;
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

        private string filterText;
        public string FilterText
        {
            get { return this.filterText; }
            set
            {
                if (value == this.filterText) return;
                this.filterText = value;
                this.OnPropertyChanged();
                this.Filter = string.IsNullOrEmpty(this.filterText) ? (Predicate<object>)null : this.IsMatch;
            }
        }

        private Predicate<object> filter;
        public Predicate<object> Filter
        {
            get { return this.filter; }
            private set
            {
                this.filter = value;
                this.OnPropertyChanged();
            }
        }

        private ObservableCollection<string> typeGroups = new ObservableCollection<string>();
        public ObservableCollection<string> TypeGroups
        {
            get { return typeGroups; }
            set
            {
                typeGroups = value;
                OnPropertyChanged();
            }

        }

        #endregion

        #region Commands
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

        private ICommand addCommand;
        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                    addCommand = new RelayCommand(p => true, p => this.Add());
                return addCommand;
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

        private ICommand selectTypeCommand;
        public ICommand SelectTypeCommand
        {
            get
            {
                if (selectTypeCommand == null)
                    selectTypeCommand = new RelayCommand( p => true, p => this.SelectType(p));
                return selectTypeCommand;
            }
        }

        private ICommand cancelTypeCommand;
        public ICommand CancelTypeCommand
        {
            get
            {
                if (cancelTypeCommand == null)
                    cancelTypeCommand = new RelayCommand( p => true, p => this.CancelType(p));
                return cancelTypeCommand;
            }
        }

        private ICommand editTypeCommand;
        public ICommand EditTypeCommand
        {
            get
            {
                if (editTypeCommand == null)
                    editTypeCommand = new RelayCommand( p => true, p => this.EditType(p));
                return editTypeCommand;
            }
        }

        private ICommand addTypeCommand;
        public ICommand AddTypeCommand
        {
            get
            {
                if (addTypeCommand == null)
                    addTypeCommand = new RelayCommand( p => true, p => this.AddType(p));
                return addTypeCommand;
            }
        }

        private ICommand getImageFromFileCommand;
        public ICommand GetImageFromFileCommand
        {
            get
            {
                if (getImageFromFileCommand == null)
                    getImageFromFileCommand = new RelayCommand( p => true, p => this.GetImageFromFile(p));
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
                    Types.Add(typeItem);
                }

                TypeGroups.Add("Object");
                TypeGroups.Add("ControlObject");
                TypeGroups.Add("Property");
                TypeGroups.Add("ControlProperty");
                TypeGroups.Add("Template");
                TypeGroups.Add("Requirement");
            }
            IsChanged = false;
        }

        public void Add()
        {

        }


        private void Delete()
        {
            // ToDo: Deleting items in the collection only works using the Del key for now. 
            // Implement delete method to also provide option using context menu
        }

        public void Save()
        {
            EDBEntities eDB = new EDBEntities();
            foreach (var typeItem in Types)
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
            TypeModel typeItem = Types.Single(x => x.Type == objectType);
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

        public TypeModel GetType(string type)
        {
            foreach (var typeItem in Types)
            {
                if (typeItem.Type == type) return typeItem;
            }
            return null;
        }

        private void SelectType(object p)
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
                        break;
                    case "ControlObject":
                        foreach (var item in ControlObjectViewModelLocator.GetControlObjectVM().SelectedItems)
                            item.ControlObjectType_ID = type.ID;
                        break;
                    case "Template":
                        foreach (var item in TemplateViewModelLocator.GetTemplateVM().SelectedItems)
                            item.TemplateType_ID = type.ID;
                        break;
                    case "Property":
                        foreach (var item in PropertyViewModelLocator.GetPropertyVM().SelectedItems)
                            item.PropertyType_ID = type.ID;
                        break;
                    case "ControlProperty":
                        foreach (var item in PropertyViewModelLocator.GetPropertyVM().SelectedItems)
                            item.PropertyType_ID = type.ID;
                        break;
                    case "Requirement":
                        foreach (var item in RequirementViewModelLocator.GetRequirementVM().SelectedItems)
                            item.RequirementType_ID = type.ID;
                        break;
                }
                CloseTrigger = true;

            }
        }

        private void CancelType(object p)
        {
            CloseTrigger = true;
        }

        private void EditType(object p)
        {
            TypeGroup = p as string;
            var typeEditDialog = new TypeEditDialog();
            typeEditDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CloseTrigger = true;
            typeEditDialog.ShowDialog();
        }

        private void AddType(object p)
        {
            BinaryToImage Converter = new BinaryToImage();
            TypeModel typeItem = new TypeModel()
            {
                Type = "New Type",
                Description = "Type Description",
                // Use the default image with question mark from resources 
                Image = (byte[])Converter.ConvertBack(Resources.NewItem, typeof(byte[]), null, null),
                TypeGroup = "Object",
                IsNew = true
            };
            typeItem.IsNew = true;
            Types.Add(typeItem);
        }

        public int GetTypeGroupID(string TypeGroup)
        {
            return Types.LastOrDefault(t => t.TypeGroup == TypeGroup).ID;
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

        private bool IsMatch(object item)
        {
            return IsMatch((TypeModel)item, this.filterText);
        }

        /// <summary>
        /// Filters out Types that are part of a group (e.g. Object, Template etc). The FilterText is the TypeGroup
        /// </summary>
        /// <param name="item"></param>
        /// <param name="filterText"></param>
        /// <returns></returns>
        private static bool IsMatch(TypeModel item, string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
            {
                return true;
            }

            var name = item.TypeGroup;
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (filterText.Length == 1)
            {
                return name.StartsWith(filterText, StringComparison.OrdinalIgnoreCase);
            }

            return name.IndexOf(filterText, 0, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        #endregion
    }
}