using System.ComponentModel;
using Telerik.Windows.Controls;

namespace Sculptor
{
    public class RequirementTypeModel : ViewModelBase, INotifyPropertyChanged
    {
        int id;
        string requirementType;

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

        public string RequirementType
        {
            get
            {
                return this.requirementType;
            }
            set
            {
                if (value != this.requirementType)
                {
                    this.requirementType = value;
                }
            }
        }
    }
}
