using System.Windows;
using System.Windows.Controls;

namespace WpfApp.AttachedProperty
{
    public class ScrollExtensions
    {
        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoScroll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), 
                typeof(ScrollExtensions), new PropertyMetadata(false, OnAutoScrollChnaged));

        private static void OnAutoScrollChnaged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer viewer && (bool)e.NewValue)
                viewer.SizeChanged += (s, _) => viewer.ScrollToEnd();
        }
    }
}
