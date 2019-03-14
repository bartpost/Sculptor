using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;
using TD = Telerik.Windows.Data;

namespace Sculptor
{
    public class TemplateAssociationModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Constructor
        public TemplateAssociationModel()
        {
        }
        #endregion

        #region Properties

        private Guid id;
        public Guid ID
        {
            get { return this.id; }
            set
            {
                this.id = value;
            }
        }

        private int project_id;
        public int Project_ID
        {
            get { return this.project_id; }
            set
            {
                this.project_id = value;
            }
        }

        private Guid template_id;
        public Guid Template_ID
        {
            get { return this.template_id; }
            set
            {
                if (template_id != value)
                {
                    template_id = value;
                    OnPropertyChanged();
                }
            }
        }

        private Guid association_id;
        public Guid Association_ID
        {
            get { return this.association_id; }
            set
            {
                if (association_id != value)
                {
                    association_id = value;
                    OnPropertyChanged();
                }
            }
        }

        private Nullable<Guid> associationParent_id;
        public Nullable<Guid> AssociationParent_ID
        {
            get { return this.associationParent_id; }
            set
            {
                if (associationParent_id != value)
                {
                    associationParent_id = value;
                    OnPropertyChanged();
                }
            }
        }

        private TD.ObservableItemCollection<TemplateAssociationModel> childAssociations;
        public TD.ObservableItemCollection<TemplateAssociationModel> ChildAssociations
        {
            get { return this.childAssociations; }
            set
            {
                this.childAssociations = value;
                OnPropertyChanged();
            }
        }

        // Name can be the name of the class, the property or the class property
        private string name;
        public string Name
        {
            get{ return this.name; }
            set
            {
                this.name = value;
                OnPropertyChanged();
            }
        }

        private string description;
        public string Description
        {
            get { return this.description; }
            set
            {
                this.description = value;
                OnPropertyChanged();
            }
        }

        private String value;
        public string Value
        {
            get { return this.value; }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnPropertyChanged();
                }
            }
        }

        private string associationType;
        public string AssociationType
        {
            get { return this.associationType; }
            set
            {
                this.associationType = value;
                OnPropertyChanged();
            }
        }

        private bool isNew;
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

        private bool isExpanded;
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

        private bool isDeleted;
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

        private int associationType_ID;
        public int AssociationType_ID
        {
            get { return this.associationType_ID; }
            set
            {
                this.associationType_ID = value;
                OnPropertyChanged();
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
