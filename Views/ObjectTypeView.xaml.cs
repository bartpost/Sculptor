using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sculptor.Views
{
    /// <summary>
    /// Interaction logic for AddObjectTypeDialog.xaml
    /// </summary>
    public partial class ObjectTypeView : Window
    {
        public ObjectTypeView()
        {
            InitializeComponent();
        }

        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = ObjectTypeViewModelLocator.GetObjectTypeVM();
        }
    }
}
