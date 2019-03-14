using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace Sculptor
{
    public class HardIOModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Contructor
        public HardIOModel()
        {
        }
        #endregion

        #region Properties
        private string objectName;
        public string ObjectName
        {
            get
            {
                return this.objectName;
            }
            set
            {
                if (value != this.objectName)
                {
                    this.objectName = value;
                }
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
                if (value != this.description)
                {
                    this.description = value;
                }
            }
        }

        private string propertyName;
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
            set
            {
                if (value != this.propertyName)
                {
                    this.propertyName = value;
                }
            }
        }

        #endregion

        #region Events
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
