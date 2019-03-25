using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using TD = Telerik.Windows.Data;

namespace Sculptor
{
    public class ControlObjectAssociationModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Properties
        public ControlObjectAssociationModel()
        {
        }

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
            get
            {
                return this.project_id;
            }
            set
            {
                this.project_id = value;
            }
        }

        private Guid controlObject_id;
        public Guid ControlObject_ID
        {
            get
            {
                return this.controlObject_id;
            }
            set
            {
                if (controlObject_id != value)
                {
                    controlObject_id = value;
                }
            }
        }

        private Guid controlProperty_id;
        public Guid ControlProperty_ID
        {
            get
            {
                return this.controlProperty_id;
            }
            set
            {
                if (controlProperty_id != value)
                {
                    controlProperty_id = value;
                }
            }
        }

        private Guid? association_id;
        public Guid? Association_ID
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
                }
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                //OnPropertyChanged();
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
                //OnPropertyChanged();
            }
        }

        private String value;
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

        private string associationType;
        public string AssociationType
        {
            get
            {
                return this.associationType;
            }
            set
            {
                this.associationType = value;
                //OnPropertyChanged();
            }
        }

        private bool isNew;
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
