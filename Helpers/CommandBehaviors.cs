using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PDFRider
{
    /// <summary>
    /// Defines a command for Window_Closing event
    /// </summary>
    public class ClosingCommandBehavior
    {
        public static DependencyProperty WindowClosingProperty = DependencyProperty.RegisterAttached(
            "WindowClosing", typeof(ICommand), typeof(ClosingCommandBehavior),
            new UIPropertyMetadata(ClosingPropertyChanged));

        public static void SetWindowClosing(DependencyObject obj, ICommand value)
        {
            obj.SetValue(WindowClosingProperty, value);
        }

        private static void ClosingPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Window window = o as Window;

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                window.Closing += Window_Closing;
            }
            if ((e.NewValue == null) && (e.OldValue != null))
            {
                window.Closing -= Window_Closing;
            }
        }

        private static void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UIElement element = (UIElement)sender;
            ICommand command = (ICommand)element.GetValue(ClosingCommandBehavior.WindowClosingProperty);
            command.Execute(e);
        }
    }
}
