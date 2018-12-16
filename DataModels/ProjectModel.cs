using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;
using System.Runtime.Serialization.Configuration;
using System;

namespace Sculptor
{
    #region ProjectModel

    public class ProjectModel : ViewModelBase, INotifyPropertyChanged
    {
        private int id;
        private string projectname;
        private string customerName;
        private string contractNo;
        private byte[] logo;
        private DateTime? lastOpened;
        private string lastOpenedBy;

        public ProjectModel()
        {
        }

        public int ID
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

        public string ProjectName
        {
            get
            {
                return this.projectname;
            }
            set
            {
                if (value != this.projectname)
                {
                    this.projectname = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CustomerName
        {
            get
            {
                return this.customerName;
            }
            set
            {
                if (value != this.customerName)
                {
                    this.customerName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ContractNo
        {
            get
            {
                return this.contractNo;
            }
            set
            {
                if (value != this.contractNo)
                {
                    this.contractNo = value;
                    OnPropertyChanged();
                }
            }
        }

        public byte[] Logo
        {
            get
            {
                return this.logo;
            }
            set
            {
                if (value != this.logo)
                {
                    this.logo = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? LastOpened
        {
            get
            {
                return this.lastOpened;
            }
            set
            {
                if (value != this.lastOpened)
                {
                    this.lastOpened = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastOpenedBy
        {
            get
            {
                return this.lastOpenedBy;
            }
            set
            {
                if (value != this.lastOpenedBy)
                {
                    this.lastOpenedBy = value;
                    OnPropertyChanged();
                }
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    #endregion
}
