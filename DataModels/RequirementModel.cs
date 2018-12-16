using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace Sculptor.DataModels
{
    public class RequirementModel : ViewModelBase, INotifyPropertyChanged
    {
        private Guid id;
        private Nullable<Guid> parent_id;
        private int project_ID;
        private string articleNo;
        private string articleHeader;
        private string content;
        private int requirementType_ID;
        private DateTime created;
        private string createdBy;
        private DateTime lastChanged;
        private string lastChangedBy;
        private string version;
        private ObservableCollectionWithItemChanged<RequirementModel> childRequirements;
        private bool isNew;
        private bool isChanged;
        private bool isDeleted;

        public RequirementModel()
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
            }
        }

        public int Project_ID
        {
            get
            {
                return this.project_ID;
            }
            set
            {
                this.project_ID = value;
            }
        }

        public string ArticleNo
        {
            get
            {
                return this.articleNo;
            }
            set
            {
                if (value != this.articleNo)
                {
                    this.articleNo = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ArticleHeader
        {
            get
            {
                return this.articleHeader;
            }
            set
            {
                if (value != this.articleHeader)
                {
                    this.articleHeader = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Content
        {
            get
            {
                return this.content;
            }
            set
            {
                if (value != this.content)
                {
                    this.content = value;
                    OnPropertyChanged();
                }
            }
        }

        public int RequirementType_ID
        {
            get
            {
                return this.requirementType_ID;
            }
            set
            {
                if (value != this.requirementType_ID)
                {
                    this.requirementType_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime Created
        {
            get
            {
                return this.created;
            }
            set
            {
                if (value != this.created)
                {
                    this.created = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CreatedBy
        {
            get
            {
                return this.createdBy;
            }
            set
            {
                if (value != this.createdBy)
                {
                    this.createdBy = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime LastChanged
        {
            get
            {
                return this.lastChanged;
            }
            set
            {
                if (value != this.lastChanged)
                {
                    this.lastChanged = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastChangedBy
        {
            get
            {
                return this.lastChangedBy;
            }
            set
            {
                if (value != this.lastChangedBy)
                {
                    this.lastChangedBy = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Version
        {
            get
            {
                return this.version;
            }
            set
            {
                if (value != this.version)
                {
                    this.version = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollectionWithItemChanged<RequirementModel> ChildRequirements
        {
            get
            {
                return this.childRequirements;
            }
            set
            {
                this.childRequirements = value;
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

        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (!isNew && !IsDeleted)
            {
                isChanged = true;
            }
        }
    }


}
