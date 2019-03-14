using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Sculptor
{
    public class TypeModel : ViewModelBase, INotifyPropertyChanged
    {

        #region Properties
        int id;
        public int ID
        {
            get { return this.id; }
            set
            {
                if (value != this.id)
                {
                    this.id = value;
                }
            }
        }

        int project_ID;
        public int Project_ID
        {
            get { return this.project_ID; }
            set
            {
                if (value != this.project_ID)
                {
                    this.project_ID = value;
                }
            }
        }

        string type;
        public string Type
        {
            get { return this.type; }
            set
            {
                if (value != this.type)
                {
                    this.type = value;
                    OnPropertyChanged();
                }
            }
        }

        string description;
        public string Description
        {
            get { return this.description; }
            set
            {
                if (value != this.description)
                {
                    this.description = value;
                    OnPropertyChanged();
                }
            }
        }

        byte[] image;
        public byte[] Image
        {
            get { return this.image; }
            set
            {
                if (value != this.image)
                {
                    this.image = value;
                    OnPropertyChanged();
                }
            }
        }

        string typeGroup;
        public string TypeGroup
        {
            get { return this.typeGroup; }
            set
            {
                if (value != this.typeGroup)
                {
                    this.typeGroup = value;
                    OnPropertyChanged();
                }
            }
        }

        int showOrder;
        public int ShowOrder
        {
            get { return this.showOrder; }
            set
            {
                if (value != this.showOrder)
                {
                    this.showOrder = value;
                    OnPropertyChanged();
                }
            }
        }
        // ToDo: If item is changed before the item is saved, an exception will be raised
        bool isChanged;
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

        bool isNew;
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

        bool isDeleted;
        public bool IsDeleted
        {
            get { return this.isDeleted; }
            set
            {
                if (value != this.isDeleted)
                {
                    this.isDeleted = value;
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
