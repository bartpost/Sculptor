using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace Sculptor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            StyleManager.ApplicationTheme = new Office2016Theme();
            Office2016Palette.Palette.AccentColor = (Color)ColorConverter.ConvertFromString("#FF000000");
            Office2016Palette.Palette.AccentFocusedColor = (Color)ColorConverter.ConvertFromString("#FF88C3FF");
            Office2016Palette.Palette.AccentMouseOverColor = (Color)ColorConverter.ConvertFromString("#FF3E6DB6");
            Office2016Palette.Palette.AccentPressedColor = (Color)ColorConverter.ConvertFromString("#FF19478A");
            Office2016Palette.Palette.AlternativeColor = (Color)ColorConverter.ConvertFromString("#FFF1F1F1");
            Office2016Palette.Palette.BasicColor = (Color)ColorConverter.ConvertFromString("#FFABABAB");
            Office2016Palette.Palette.ComplementaryColor = (Color)ColorConverter.ConvertFromString("#FFE1E1E1");
            Office2016Palette.Palette.IconColor = (Color)ColorConverter.ConvertFromString("#FF444444");
            Office2016Palette.Palette.MainColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF");
            Office2016Palette.Palette.MarkerColor = (Color)ColorConverter.ConvertFromString("#FF444444");
            Office2016Palette.Palette.MarkerInvertedColor = (Color)ColorConverter.ConvertFromString("#FFF9F9F9");
            Office2016Palette.Palette.MouseOverColor = (Color)ColorConverter.ConvertFromString("#FFC5C5C5");
            Office2016Palette.Palette.PressedColor = (Color)ColorConverter.ConvertFromString("#FFAEAEAE");
            Office2016Palette.Palette.PrimaryColor = (Color)ColorConverter.ConvertFromString("#FFE6E6E6");
            Office2016Palette.Palette.SelectedColor = (Color)ColorConverter.ConvertFromString("#FFEBEBEB");
            Office2016Palette.Palette.ValidationColor = (Color)ColorConverter.ConvertFromString("#FFE81123");
            this.InitializeComponent();
        }
    }
}
