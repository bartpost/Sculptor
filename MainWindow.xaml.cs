using Sculptor.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Sculptor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObjectView objectViewControl;
        private TemplateView templateViewControl;
        private PropertyView propertyViewControl;
        private RequirementView requirementViewControl;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModelLocator.GetMainVM();
        }

        // TODO: Haven't found a way to control this through the view model. 
        private void EditObjectTree_Click(object sender, RoutedEventArgs e)
        {
            if (objectViewControl == null) objectViewControl = new ObjectView();
            MainFrame.NavigationService.Navigate(objectViewControl);
        }

        private void EditTemplateTree_Click(object sender, RoutedEventArgs e)
        {
            if (templateViewControl == null) templateViewControl = new TemplateView();
            MainFrame.NavigationService.Navigate(templateViewControl);
        }

        private void EditPropertyTree_Click(object sender, RoutedEventArgs e)
        {
            if (propertyViewControl == null) propertyViewControl = new PropertyView();
            MainFrame.NavigationService.Navigate(propertyViewControl);
        }

        private void EditRequirementTree_Click(object sender, RoutedEventArgs e)
        {
            if (requirementViewControl == null) requirementViewControl = new RequirementView();
            MainFrame.NavigationService.Navigate(requirementViewControl);
        }

        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
           // SculptorRibbonView.IsBackstageOpen = false;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }

        private void ClearHistory()
        {
            if (!MainFrame.CanGoBack && !MainFrame.CanGoForward)
            {
                return;
            }

            var entry = MainFrame.RemoveBackEntry();
            while (entry != null)
            {
                entry = MainFrame.RemoveBackEntry();
            }

            //this.MainFrame.Navigate(new PageFunction<string>() { RemoveFromJournal = true });
        }

    }

}
