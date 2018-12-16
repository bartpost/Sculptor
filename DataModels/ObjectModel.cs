using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Sculptor.DataModels
{
    public class ObjectModel : ViewModelBase, INotifyPropertyChanged, IDataErrorInfo
    {
        private Guid id;
        private Nullable<Guid> parent_id;
        private int project_id;
        private string objectname;
        private string description;
        private int objecttype_ID;
        private bool isExpanded;
        private bool isNew;
        private bool isChanged;
        private bool isDeleted;
        private ObservableCollectionWithItemChanged<ObjectModel> childobjects;
        public ObjectModel()
        {
        }

        #region Properties
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

        public int ObjectType_ID
        {
            get
            {
                return this.objecttype_ID;
            }
            set
            {
                if (value != this.objecttype_ID)
                {
                    this.objecttype_ID = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollectionWithItemChanged<ObjectModel> ChildObjects
        {
            get
            {
                return this.childobjects;
            }
            set
            {
                this.childobjects = value;
                OnPropertyChanged();
            }
        }

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
        #endregion

        #region Validations
        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }

        static readonly string[] ValidatedProperties =
        {
            "ObjectName"
        };

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return GetValidationError(propertyName);
            }
        }

        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperties)
                    if (GetValidationError(property) != null)
                        return false;
                return true;
            }

        }
        string GetValidationError(string propertyName)
        {
            string error = null;

            switch (propertyName)
            {
                case "ObjectName":
                    error = ValidateObjectName();
                    break;
            }
            return error;
        }

        private string ValidateObjectName()
        {
            if (ObjectName == "bartje")
                return "Fault";
            else
                return "";
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
