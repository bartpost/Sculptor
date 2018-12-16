using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;

namespace Sculptor
{
    public class PropertyModel : ViewModelBase, INotifyPropertyChanged
    {
        private Guid id;
        private Nullable<Guid> parent_id;
        private int project_id;
        private string propertyName;
        private string description;
        private int propertyType_ID;
        private string value;
        private string aspect;
        private string attribute1;
        private string attribute2;
        private string attribute3;
        private bool isNew;
        private bool isChanged;
        private bool isDeleted;
        private ObservableCollectionWithItemChanged<PropertyModel> childProperties;

        #region Properties
        public PropertyModel()
        {
        }

        public Guid ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public Nullable<Guid> Parent_ID
        {
            get
            {
                return this.parent_id;
            }
            set
            {
                this.parent_id = value;
                OnPropertyChanged();
            }
        }

        public int Project_ID
        {
            get
            {
                return this.project_id;
            }
            set
            {
                this.project_id = value;
                OnPropertyChanged();
            }
        }

        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
            set
            {
                if (value != this.propertyName)
                {
                    this.propertyName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (value != this.description)
                {
                    this.description = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PropertyType_ID
        {
            get
            {
                return this.propertyType_ID;
            }
            set
            {
                if (value != this.propertyType_ID)
                {
                    this.propertyType_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Aspect
        {
            get
            {
                return this.aspect;
            }
            set
            {
                if (value != this.aspect)
                {
                    this.aspect = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Attribute1
        {
            get
            {
                return this.attribute1;
            }
            set
            {
                if (value != this.attribute1)
                {
                    this.attribute1 = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Attribute2
        {
            get
            {
                return this.attribute2;
            }
            set
            {
                if (value != this.attribute2)
                {
                    this.attribute2 = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Attribute3
        {
            get
            {
                return this.attribute3;
            }
            set
            {
                if (value != this.attribute3)
                {
                    this.attribute3 = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollectionWithItemChanged<PropertyModel> ChildProperties
        {
            get
            {
                return this.childProperties;
            }
            set
            {
                this.childProperties = value;
                OnPropertyChanged();
            }
        }

        public bool IsNew
        {
            get
            {
                return this.isNew;
            }
            set
            {
                if (value != this.isNew)
                {
                    this.isNew = value;
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

        public bool IsDeleted
        {
            get
            {
                return this.isDeleted;
            }
            set
            {
                if (value != this.isDeleted)
                {
                    this.isDeleted = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Events
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (!isNew && !IsDeleted) isChanged = true;
        }
        #endregion
    }
}
