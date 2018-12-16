using System.ComponentModel;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Sculptor
{
    #region ObjectTypeModel
    public class ObjectTypeModel : ViewModelBase, INotifyPropertyChanged
    {
        int id;
        string objectType;
        string description;
        byte[] image;

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

        public string ObjectType
        {
            get
            {
                return this.objectType;
            }
            set
            {
                if (value != this.objectType)
                {
                    this.objectType = value;
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
                }
            }
        }
    }
    #endregion
}
