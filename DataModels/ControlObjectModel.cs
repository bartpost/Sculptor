using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Telerik.Windows.Controls;

namespace Sculptor.DataModels
{
    public class ControlObjectModel : ViewModelBase
    {
        #region Constructor
        public ControlObjectModel()
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
            }
        }

        private string objectname;
        [XmlIgnore]
        public string ObjectName
        {
            get
            {
                return this.objectname;
            }
            set
            {
                if (value != this.objectname)
                {
                    this.objectname = value;
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

        private int controlgeartype_ID;
        [XmlIgnore]
        public int ControlObjectType_ID
        {
            get
            {
                return this.controlgeartype_ID;
            }
            set
            {
                if (value != this.controlgeartype_ID)
                {
                    this.controlgeartype_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private Telerik.Windows.Data.ObservableItemCollection<ControlObjectModel> childcontrolgear;
        public Telerik.Windows.Data.ObservableItemCollection<ControlObjectModel> ChildControlObjects
        {
            get
            {
                return this.childcontrolgear;
            }
            set
            {
                this.childcontrolgear = value;
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

        #region Commands

        #endregion

        #region events
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (!isNew && !IsDeleted) isChanged = true;
        }
        #endregion

        #region Methods
        #endregion
    }
}
