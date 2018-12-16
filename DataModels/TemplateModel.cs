using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;

namespace Sculptor
{
    public class TemplateModel : ViewModelBase, INotifyPropertyChanged
    {
        private Guid id;
        private Nullable<Guid> parent_id;
        private int project_id;
        private string templateName;
        private string description;
        private int templatetype_ID;
        private bool isNew;
        private bool isChanged;
        private bool isDeleted;
        private ObservableCollectionWithItemChanged<TemplateModel> childTemplates;

        public TemplateModel()
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

        public ObservableCollectionWithItemChanged<TemplateModel> ChildTemplates
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
