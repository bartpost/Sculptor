using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace Sculptor
{
    public class AspectModel : ViewModelBase, INotifyPropertyChanged
    {
        private Guid id;
        private int project_ID;
        private string aspectName;
        private string description;
        private bool hardIO;
        private bool extIO;
        private bool plcTag;
        private bool scadaTag;
        private bool alarmTag;
        private bool trendTag;
        private string note;
        private bool isChanged;
        private bool isNew;
        private bool isDeleted;

        #region Contructor
        public AspectModel()
        {
        }
        #endregion

        #region Properties
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

        public string AspectName
        {
            get
            {
                return this.aspectName;
            }
            set
            {
                if (value != this.aspectName)
                {
                    this.aspectName = value;
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

        public bool HardIO
        {
            get
            {
                return this.hardIO;
            }
            set
            {
                if (value != this.hardIO)
                {
                    this.hardIO = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ExtIO
        {
            get
            {
                return this.extIO;
            }
            set
            {
                if (value != this.extIO)
                {
                    this.extIO = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool PLCTag
        {
            get
            {
                return this.plcTag;
            }
            set
            {
                if (value != this.plcTag)
                {
                    this.plcTag = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool SCADATag
        {
            get
            {
                return this.scadaTag;
            }
            set
            {
                if (value != this.scadaTag)
                {
                    this.scadaTag = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AlarmTag
        {
            get
            {
                return this.alarmTag;
            }
            set
            {
                if (value != this.alarmTag)
                {
                    this.alarmTag = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool TrendTag
        {
            get
            {
                return this.trendTag;
            }
            set
            {
                if (value != this.trendTag)
                {
                    this.trendTag = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Note
        {
            get
            {
                return this.note;
            }
            set
            {
                if (value != this.note)
                {
                    this.note = value;
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
