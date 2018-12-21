using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Sculptor
{
    public class TypeModel : ViewModelBase, INotifyPropertyChanged
    {
        int id;
        int project_ID;
        string type;
        string description;
        byte[] image;
        string typeGroup;
        int showOrder;
        bool isChanged;
        bool isNew;
        bool isDeleted;

        #region Properties
        public int ID
        {
            get
            {
                return this.id;
            }
            set
            {
                if (value != this.id)
                {
                    this.id = value;
                }
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
                if (value != this.project_ID)
                {
                    this.project_ID = value;
                }
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                if (value != this.type)
                {
                    this.type = value;
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

        public byte[] Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (value != this.image)
                {
                    this.image = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TypeGroup
        {
            get
            {
                return this.typeGroup;
            }
            set
            {
                if (value != this.typeGroup)
                {
                    this.typeGroup = value;
                    OnPropertyChanged();
                }
            }
        }


        public int ShowOrder
        {
            get
            {
                return this.showOrder;
            }
            set
            {
                if (value != this.showOrder)
                {
                    this.showOrder = value;
                    OnPropertyChanged();
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

        #region events
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (!isNew && !IsDeleted) isChanged = true;
        }
        #endregion
    }
}
