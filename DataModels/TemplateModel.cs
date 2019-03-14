using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using TD = Telerik.Windows.Data;

namespace Sculptor
{
    [Serializable]
    public class TemplateModel : ViewModelBase, INotifyPropertyChanged
    {

        #region Properties
        public TemplateModel()
        {
        }

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

        private string templateName;
        [XmlIgnore]
        public string TemplateName
        {
            get
            {
                return this.templateName;
            }
            set
            {
                if (value != this.templateName)
                {
                    this.templateName = value;
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

        private int templatetype_ID;
        [XmlIgnore]
        public int TemplateType_ID
        {
            get
            {
                return this.templatetype_ID;
            }
            set
            {
                if (value != this.templatetype_ID)
                {
                    this.templatetype_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private TD.ObservableItemCollection<TemplateModel> childTemplates;
        public TD.ObservableItemCollection<TemplateModel> ChildTemplates
        {
            get
            {
                return this.childTemplates;
            }
            set
            {
                this.childTemplates = value;
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
