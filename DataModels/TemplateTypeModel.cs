using System.ComponentModel;
using Telerik.Windows.Controls;

namespace Sculptor
{
    #region TemplateTypeModel
    public class TemplateTypeModel : ViewModelBase, INotifyPropertyChanged
    {
        int id;
        string templateType;
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

        public string TemplateType
        {
            get
            {
                return this.templateType;
            }
            set
            {
                if (value != this.templateType)
                {
                    this.templateType = value;
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
