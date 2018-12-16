using Sculptor.EDBEntityDataModel;
using System;
using System.Linq;
using Telerik.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;

namespace Sculptor
{
    public class MainViewModel
    {
        private ObservableCollectionWithItemChanged<ProjectModel> projects = new ObservableCollectionWithItemChanged<ProjectModel>();
        private ObservableCollectionWithItemChanged<ProjectModel> selectedItems;
        private bool projectSelected;
        private bool isBackStageOpen=true;
        private UserControl userControlFrame;
        private UserControl objectViewControl;
        private bool isBusy;
        private string mainTitle = "No project selected ....";

        #region Properties

        public ObservableCollectionWithItemChanged<ProjectModel> Projects
        {
            get
            {
                return projects;
            }
            set
            {
                projects = value;
                //OnPropertyChanged();
            }
        }
        public ObservableCollectionWithItemChanged<ProjectModel> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                {
                    selectedItems = new ObservableCollectionWithItemChanged<ProjectModel>();
                }
                return selectedItems;
            }
        }

        public DelegateCommand SelectProjectCommand { get; set; }
        public DelegateCommand CloseProjectCommand { get; set; }
        public DelegateCommand OpenObjectViewCommand { get; set; }

        public bool ProjectSelected {
            get
            {
                return projectSelected;
            }

            set
            {
                projectSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsBackStageOpen
        {
            get
            {
                return isBackStageOpen;
            }

            set
            {
                isBackStageOpen = value;
                OnPropertyChanged();
            }
        }

        public string MainTitle {
            get
            {
                return mainTitle;
            }
            set
            {
                if (mainTitle != value)
                {
                    mainTitle = value;
                    OnPropertyChanged();
                }
            }
        }


        // Using a DependencyProperty as the backing store for MainTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyTitleProperty =
            DependencyProperty.Register("MainTitle", typeof(string), typeof(MainWindow), new UIPropertyMetadata(null));

        public UserControl UserControlFrame
        {
            get { return userControlFrame; }
            set
            {
                userControlFrame = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
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
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        public MainViewModel()
        {
            // Load the Project List
            Load();
            SelectProjectCommand = new DelegateCommand(p => SelectProject(p), p => CanSelectProject(p));
            CloseProjectCommand = new DelegateCommand(p => CloseProject(p), p => CanCloseProject(p));
            OpenObjectViewCommand = new DelegateCommand(p => OpenObjectView(p), p => CanOpenObjectView(p));
        }

        /// <summary>
        /// Loads the records from the DbSet into the ViewModel. This function designed for recursive use
        /// </summary>
        /// <returns>Observable collection of Projects</returns>
        private void Load()
        {
            using (EDBEntities eDB = new EDBEntities())
            {
                try
                {
                    foreach (tblProject Rec in (from o in eDB.tblProjects select o))
                    {
                        ProjectModel projectItem = new ProjectModel
                        {
                            ID = Rec.ID,
                            ProjectName = Rec.ProjectName,
                            ContractNo = Rec.ContractNo,
                            CustomerName = Rec.CustomerName,
                            Logo = Rec.Logo,
                            LastOpened = Rec.LastOpened,
                            LastOpenedBy = Rec.LastOpenedBy
                        };

                        Projects.Add(projectItem);
                    }
                }
                catch(Exception ex)
                {
                    RadWindow.Alert(ex.Message);
                }
            }
        }
        
        /// <summary>
        /// Saves all changes to the ViewModel
        /// </summary>
        private void Save()
        {
            EDBEntities eDB = new EDBEntities();
            eDB.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CanSelectProject(object p)
        {
            return true;
        }

        private ProjectModel GetProject(int id)
        {
            foreach (var projectItem in Projects)
            {
                // return the item if found on this level
                if (projectItem.ID == id) return projectItem;
            }
            return null;
        }

        /// <summary>
        /// Select Project Command through command interface
        /// </summary>
        private void SelectProject(Object p)
        {
            ProjectModel selectedProject = GetProject((int)p);
            Globals.Project_ID = (int)p;
            Globals.ProjectSelected = true;
            ProjectSelected = true;
            Globals.ProjectName = selectedProject.ProjectName;
            selectedProject.LastOpened = DateTime.Now;

            // Store the current DateTime in the selected project record so it can be used to sort the project list
            EDBEntities eDB = new EDBEntities();
            tblProject Rec = eDB.tblProjects.Where(o => o.ID == selectedProject.ID).FirstOrDefault();
            Rec.LastOpened = selectedProject.LastOpened;
            Rec.LastOpenedBy = Environment.UserName;
            eDB.SaveChanges();

            IsBackStageOpen = false;
            MainTitle = Globals.ProjectName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CanCloseProject(object p)
        {
            return true;
        }

        private void CloseProject(Object p)
        {
            ProjectModel selectedProject = null;
            Globals.Project_ID = 0;
            Globals.ProjectSelected = false;
            ProjectSelected = false;
            Globals.ProjectName = "";

            if (ObjectViewModelLocator.IsLoaded()) (ObjectViewModelLocator.GetObjectVM()).Close();
            //if (ClassViewModelLocator.IsLoaded()) (ClassViewModelLocator.GetClassVM()).Close();
            //if (PropertyViewModelLocator.IsLoaded()) (PropertyViewModelLocator.GetPropertyVM()).Close();
            //if (propertyUserControl != null)
            //    (PropertyViewModelLocator.GetProperties()).Close();

        }

        private bool CanOpenObjectView(object p)
        {
            return true;
        }

        private void OpenObjectView(object p)
        {
            //if (objectViewControl == null) objectViewControl = new PropertyView();
            //MainFrame.NavigationService.Navigate(objectViewControl);
            if (objectViewControl == null) objectViewControl = new ObjectView();
            UserControlFrame = objectViewControl;

        }
        #endregion
    }


}

