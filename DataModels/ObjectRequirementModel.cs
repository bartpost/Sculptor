using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Telerik.Windows.Controls;

namespace Sculptor.DataModels
{
    public class ObjectRequirementModel : ViewModelBase, INotifyPropertyChanged
    {
        private int project_id;
        private Guid object_id;
        private Guid requirement_id;
        private Nullable<Guid> requirementParent_id;
        private int requirementType_ID;
        private string articleNo;
        private string articleHeader;
        private string content;
        private string version;
        private bool? preFATOk;
        private bool? fATOk;
        private bool? sATOk;
        private string fATBy;
        private string sATBy;
        private DateTime? fATDate;
        private DateTime? sATDate;
        private string requirementType;
        private bool isNew;
        private bool isChanged;
        private bool isDeleted;
        private ObservableCollection<ObjectRequirementModel> childRequirements;

        public ObjectRequirementModel()
        {
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
            }
        }

        public Guid Object_ID
        {
            get
            {
                return this.object_id;
            }
            set
            {
                if (object_id != value)
                {
                    object_id = value;
                    OnPropertyChanged();
                }
            }
        }

        public Guid Requirement_ID
        {
            get
            {
                return this.requirement_id;
            }
            set
            {
                if (requirement_id != value)
                {
                    requirement_id = value;
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
                if (requirementType_ID != value)
                {
                    requirementType_ID = value;
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
                if (version != value)
                {
                    version = value;
                    OnPropertyChanged();
                }
            }
        }

        public Nullable<Guid> RequirementParent_ID
        {
            get
            {
                return this.requirementParent_id;
            }
            set
            {
                if (requirementParent_id != value)
                {
                    requirementParent_id = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ObjectRequirementModel> ChildRequirements
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

        public string ArticleNo
        {
            get
            {
                return this.articleNo;
            }
            set
            {
                this.articleNo = value;
                OnPropertyChanged();
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
                this.articleHeader = value;
                OnPropertyChanged();
            }
        }

        public bool? PreFATOk
        {
            get
            {
                if (preFATOk == null)
                    preFATOk = false;
                return this.preFATOk;
            }
            set
            {
                this.preFATOk = value;
                OnPropertyChanged();
            }
        }

        public bool? FATOk
        {
            get
            {
                if (fATOk == null)
                    fATOk = false;
                return this.fATOk;
            }
            set
            {
                if (fATOk != value)
                {
                    this.fATOk = value;
                    if (fATOk == true)
                    {
                        FATBy = WindowsIdentity.GetCurrent().Name;
                        FATDate = DateTime.Now;
                    }
                    else
                    {
                        FATBy = "";
                        FATDate = null;
                    }
                }
                OnPropertyChanged();
            }
        }

        public bool? SATOk
        {
            get
            {
                if (sATOk == null)
                    sATOk = false;
                return this.sATOk;
            }
            set
            {
                if (sATOk != value)
                {
                    this.sATOk = value;
                    if (sATOk == true)
                    {
                        SATBy = WindowsIdentity.GetCurrent().Name;
                        SATDate = DateTime.Now;
                    }
                }
                OnPropertyChanged();
            }
        }

        public DateTime? FATDate
        {
            get
            {
                return this.fATDate;
            }
            set
            {
                this.fATDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? SATDate
        {
            get
            {
                return this.sATDate;
            }
            set
            {
                this.sATDate = value;
                OnPropertyChanged();
            }
        }

        public string FATBy
        {
            get
            {
                return this.fATBy;
            }
            set
            {
                this.fATBy = value;
                OnPropertyChanged();
            }
        }

        public string SATBy
        {
            get
            {
                return this.sATBy;
            }
            set
            {
                this.sATBy = value;
                OnPropertyChanged();
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
                this.content = value;
                OnPropertyChanged();
            }
        }

        public string RequirementType
        {
            get
            {
                return this.requirementType;
            }
            set
            {
                this.requirementType = value;
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
            if (!isNew && !IsDeleted) isChanged = true;
        }
    }
}
