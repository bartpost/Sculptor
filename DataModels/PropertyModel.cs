using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Telerik.Windows.Controls;
using TD = Telerik.Windows.Data;

namespace Sculptor
{
    [Serializable()]
    public class PropertyModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Constructor
        public PropertyModel()
        {
        }
        #endregion

        #region Properties


        private Guid id;
        [XmlElement("ID")]
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

        private Nullable<Guid> parent_id;
        [XmlIgnore]
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

        private int project_id;
        [XmlIgnore]
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

        private string propertyName;
        [XmlIgnore]
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

        private string description;
        [XmlIgnore]
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

        private int propertyType_ID;
        [XmlIgnore]
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

        private string value;
        [XmlIgnore]
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

        private string aspect;
        [XmlIgnore]
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

        private string attribute1;
        [XmlIgnore]
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

        private string attribute2;
        [XmlIgnore]
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

        private string attribute3;
        [XmlIgnore]
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

        private TD.ObservableItemCollection<PropertyModel> childProperties;
        public TD.ObservableItemCollection<PropertyModel> ChildProperties
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

        private bool isExpanded;
        [XmlElement("IsExpanded")]
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }
            set
            {
                if (value != this.isExpanded)
                {
                    this.isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isNew;
        [XmlIgnore]
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

        private bool isChanged;
        [XmlIgnore]
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

        private bool isDeleted;
        [XmlIgnore]
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
