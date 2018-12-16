using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace Sculptor
{
    #region Properties
    public class AttributeModel : ViewModelBase, INotifyPropertyChanged
    {
        private Guid id;
        private string attribute;
        private int project_ID;
        private string description;
        private bool isChanged;
        private bool isNew;
        private bool isDeleted;

        public AttributeModel()
        {
        }

        public Guid ID
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

        public string Attribute
        {
            get
            {
                return this.attribute;
            }
            set
            {
                if (value != this.attribute)
                {
                    this.attribute = value;
                    OnPropertyChanged();
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
