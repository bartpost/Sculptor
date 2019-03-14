using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Telerik.Windows.Controls;
using TD = Telerik.Windows.Data;

namespace Sculptor.DataModels
{
    [Serializable()]
    public class RequirementModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Constructor
        public RequirementModel()
        {

        }
        #endregion

        # region Properties
        private Guid id;
        [XmlElement("ID")]
        public Guid ID
        {
            get { return this.id; }
            set
            {
                this.id = value;
            }
        }

        private Nullable<Guid> parent_id;
        [XmlIgnore]
        public Nullable<Guid> Parent_ID
        {
            get { return this.parent_id; }
            set
            {
                this.parent_id = value;
            }
        }

        private int project_ID;
        [XmlIgnore]
        public int Project_ID
        {
            get { return this.project_ID; }
            set
            {
                this.project_ID = value;
            }
        }

        private string articleNo;
        [XmlIgnore]
        public string ArticleNo
        {
            get { return this.articleNo; }
            set
            {
                if (value != this.articleNo)
                {
                    this.articleNo = value;
                    OnPropertyChanged();
                }
            }
        }

        private string articleHeader;
        [XmlIgnore]
        public string ArticleHeader
        {
            get { return this.articleHeader; }
            set
            {
                if (value != this.articleHeader)
                {
                    this.articleHeader = value;
                    OnPropertyChanged();
                }
            }
        }

        private string content;
        [XmlIgnore]
        public string Content
        {
            get { return this.content; }
            set
            {
                if (value != this.content)
                {
                    this.content = value;
                    OnPropertyChanged();
                }
            }
        }

        private int requirementType_ID;
        [XmlIgnore]
        public int RequirementType_ID
        {
            get { return this.requirementType_ID; }
            set
            {
                if (value != this.requirementType_ID)
                {
                    this.requirementType_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime created;
        [XmlIgnore]
        public DateTime Created
        {
            get{ return this.created; }
            set
            {
                if (value != this.created)
                {
                    this.created = value;
                    OnPropertyChanged();
                }
            }
        }

        private string createdBy;
        [XmlIgnore]
        public string CreatedBy
        {
            get { return this.createdBy; }
            set
            {
                if (value != this.createdBy)
                {
                    this.createdBy = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime lastChanged;
        [XmlIgnore]
        public DateTime LastChanged
        {
            get { return this.lastChanged; }
            set
            {
                if (value != this.lastChanged)
                {
                    this.lastChanged = value;
                    OnPropertyChanged();
                }
            }
        }

        private string lastChangedBy;
        [XmlIgnore]
        public string LastChangedBy
        {
            get { return this.lastChangedBy; }
            set
            {
                if (value != this.lastChangedBy)
                {
                    this.lastChangedBy = value;
                    OnPropertyChanged();
                }
            }
        }

        private string version;
        [XmlIgnore]
        public string Version
        {
            get { return this.version; }
            set
            {
                if (value != this.version)
                {
                    this.version = value;
                    OnPropertyChanged();
                }
            }
        }

        private TD.ObservableItemCollection<RequirementModel> childRequirements;
        public TD.ObservableItemCollection<RequirementModel> ChildRequirements
        {
            get { return this.childRequirements; }
            set
            {
                this.childRequirements = value;
                OnPropertyChanged();
            }
        }

        private bool isNew;
        [XmlIgnore]
        public bool IsNew
        {
            get { return this.isNew; }
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
            get { return this.isChanged; }
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
            get { return this.isDeleted; }
            set
            {
                if (value != this.isDeleted)
                {
                    this.isDeleted = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isExpanded;
        [XmlElement("IsExpanded")]
        public bool IsExpanded
        {
            get { return this.isExpanded; }
            set
            {
                if (value != this.isExpanded)
                {
                    this.isExpanded = value;
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
            if (!isNew && !IsDeleted)
            {
                isChanged = true;
            }
        }
        #endregion
    }


}
