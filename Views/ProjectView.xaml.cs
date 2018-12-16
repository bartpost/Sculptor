
using System.Windows;
using System.Windows.Controls;

namespace Sculptor
{
    /// <summary>
    /// Interaction logic for Projects.xaml
    /// </summary>
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
