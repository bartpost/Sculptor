
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sculptor.Behaviors
{
    public static class WindowPlacementBehavior
    {
        public static NewWindowToShowParameters GetWindowPlacementBehavior(DependencyObject obj)
        {
            return (NewWindowToShowParameters)obj.GetValue(WindowPlacementBehaviorProperty);
        }

        public static void SetWindowPlacementBehavior(DependencyObject obj, NewWindowToShowParameters value)
        {
            obj.SetValue(WindowPlacementBehaviorProperty, value);
        }

        // Using a DependencyProperty as the backing store for WindowPlacement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowPlacementBehaviorProperty =
            DependencyProperty.RegisterAttached("WindowPlacementBehavior", typeof(NewWindowToShowParameters), typeof(WindowPlacementBehavior), new PropertyMetadata(null, new PropertyChangedCallback(WindowPlacementChanged)));

        private static void WindowPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button ctrl = d as Button;
            ctrl.Click += ((s, args) =>
            {
                DependencyObject parent = VisualTreeHelper.GetParent(ctrl);
                while ((parent as Window) == null)
                    parent = VisualTreeHelper.GetParent(parent);

                Window rootWindow = (Window)parent;

                NewWindowToShowParameters newWindowParams = ((NewWindowToShowParameters)e.NewValue);
                Window newWin = newWindowParams.NewWindow;
                Border b = (Border)rootWindow.FindName(newWindowParams.TargetVisualName);

                newWin.Width = b.ActualWidth;
                newWin.Height = b.ActualHeight;

                Point pt = b.PointToScreen(new Point(0, 0));
                PresentationSource source = PresentationSource.FromVisual(rootWindow);
                System.Windows.Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(pt);

                newWin.Top = targetPoints.Y;
                newWin.Left = targetPoints.X;
                newWin.Show();
            });
        }
    }

    public class NewWindowToShowParameters
    {
        public Window NewWindow { get; set; }
        public string TargetVisualName { get; set; }
    }
}
