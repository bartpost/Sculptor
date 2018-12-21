using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;

namespace Sculptor
{
    public class ObjectAssociationModel : ViewModelBase, INotifyPropertyChanged
    {
        private int project_id;
        private Guid object_id;
        private Guid association_id;
        private Nullable<Guid> associationParent_id;
        private string name;
        private string description;
        private String value;
        private bool isNew;
        private bool isChanged;
        private bool isDeleted;
        private string associationType;
        private int associationType_ID;
        private ObservableCollectionWithItemChanged<ObjectAssociationModel> childAssociations;


        public ObjectAssociationModel()
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

        public Guid Association_ID
        {
            get
            {
                return this.association_id;
            }
            set
            {
                if (association_id != value)
                {
                    association_id = value;
                    OnPropertyChanged();
                }
            }
        }

        public Nullable<Guid> AssociationParent_ID
        {
            get
            {
                return this.associationParent_id;
            }
            set
            {
                if (associationParent_id != value)
                {
                    associationParent_id = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollectionWithItemChanged<ObjectAssociationModel> ChildAssociations
        {
            get
            {
                return this.childAssociations;
            }
            set
            {
                this.childAssociations = value;
                OnPropertyChanged();
            }
        }

        // Name can be the name of the class, the property or the class property
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                OnPropertyChanged();
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
                this.description = value;
                OnPropertyChanged();
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

        public string AssociationType
        {
            get
            {
                return this.associationType;
            }
            set
            {
                this.associationType = value;
                OnPropertyChanged();
            }
        }

        public int AssociationType_ID
        {
            get
            {
                return this.associationType_ID;
            }
            set
            {
                this.associationType_ID = value;
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
