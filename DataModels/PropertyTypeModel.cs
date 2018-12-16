using System.ComponentModel;
using Telerik.Windows.Controls;

namespace Sculptor
{
    #region PropertyTypeModel
    public class PropertyTypeModel : ViewModelBase, INotifyPropertyChanged
    {
        int id;
        string propertyType;
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

        public string PropertyType
        {
            get
            {
                return this.propertyType;
            }
            set
            {
                if (value != this.propertyType)
                {
                    this.propertyType = value;
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
