using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Telerik.Windows.Controls.GridView;

namespace Sculptor
{
    /// <summary>
    /// Exposes attached behaviors that can be
    /// applied to GridViewRow objects.
    /// </summary>
    public class GridViewRowBringInFocusBehavior : Behavior<GridViewRow>
    {
        //public static bool GetIsBroughtIntoViewWhenSelected(GridViewRow gridViewRow)
        //{
        //    return (bool)gridViewRow.GetValue(IsBroughtIntoViewWhenSelectedProperty);
        //}

        //public static void SetIsBroughtIntoViewWhenSelected(GridViewRow gridViewRow, bool value)
        //{
        //    gridViewRow.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
        //}

        //public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
        //    DependencyProperty.RegisterAttached(
        //    "IsBroughtIntoViewWhenSelected",
        //    typeof(bool),
        //    typeof(GridViewRowBehavior),
        //    new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

        //static void OnIsBroughtIntoViewWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        //{
        //    GridViewRow row = depObj as GridViewRow;
        //    if (row == null)
        //        return;

        //    if (e.NewValue is bool == false)
        //        return;

        //    if ((bool)e.NewValue)
        //        row.Selected += OnGridViewRowSelected;
        //    else
        //        row.Selected -= OnGridViewRowSelected;
        //}

        protected override void OnAttached()
        {
            AssociatedObject.Selected += BringRowIntoFocusIfSelected;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Selected -= BringRowIntoFocusIfSelected;
            base.OnDetaching();
        }

        private void BringRowIntoFocusIfSelected(object sender, RoutedEventArgs routedEventArgs)
        {
            // Only react to the Selected event raised by the GridViewRow
            //    // whose IsSelected property was modified.  Ignore all ancestors
            //    // who are merely reporting that a descendant's Selected fired.

            if (!Object.ReferenceEquals(sender, routedEventArgs.OriginalSource))
                return;

            GridViewRow row = routedEventArgs.OriginalSource as GridViewRow;
            if (row != null) row.BringIntoView();
        }
    }

    public class ShowMessageOnLostFocusBehavior : Behavior<GridViewRow>
    {
        protected override void OnAttached()
        {
            AssociatedObject.LostFocus += AssociatedObjectOnLostFocus;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.LostFocus -= AssociatedObjectOnLostFocus;
            base.OnDetaching();
        }

        private void AssociatedObjectOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            MessageBox.Show("Hello");
        }
    }
}