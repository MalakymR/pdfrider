using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace PDFRider
{
    #region ClosingCommandBehavior

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

    #endregion



    #region SelectedCommandBehavior

    /// <summary>
    /// Defines a command for Selector.SelectionChanged event
    /// </summary>
    public class SelectedCommandBehavior
    {
        public static DependencyProperty ItemSelectedProperty = DependencyProperty.RegisterAttached(
            "ItemSelected", typeof(ICommand), typeof(SelectedCommandBehavior),
            new UIPropertyMetadata(ItemSelectedPropertyChanged));

        public static void SetItemSelected(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ItemSelectedProperty, value);
        }

        private static void ItemSelectedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Selector selector = o as Selector;

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                selector.SelectionChanged += Selector_SelectionChanged;
            }
            if ((e.NewValue == null) && (e.OldValue != null))
            {
                selector.SelectionChanged -= Selector_SelectionChanged;
            }
        }

        private static void Selector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Selector element = sender as Selector;
            ICommand command = (ICommand)element.GetValue(SelectedCommandBehavior.ItemSelectedProperty);
            command.Execute(element.SelectedItem);
        }
    }

    #endregion
}
