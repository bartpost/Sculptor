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
    /// Interaction logic for TypeSelectionPopup.xaml
    /// </summary>
    public partial class TypeSelectionPopup : Window
    {
        public TypeSelectionPopup()
        {
            InitializeComponent();
        }


        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = TypeViewModelLocator.GetTypeVM();
        }
    }
}
