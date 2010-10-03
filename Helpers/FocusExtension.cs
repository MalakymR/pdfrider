using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PDFRider
{
    /// <summary>
    /// Register an attached property wich can be used to set focus on an UI Element.
    /// </summary>
    /// <remarks>Setting the property to True will call UIElement.Focus() method.</remarks>
    public static class FocusExtension
    {
        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }


        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }


        public static readonly DependencyProperty IsFocusedProperty =
                DependencyProperty.RegisterAttached(
                 "IsFocused", typeof(bool), typeof(FocusExtension),
                 new UIPropertyMetadata(false, IsFocusedPropertyChanged));


        private static void IsFocusedPropertyChanged(DependencyObject d,
                DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;
            if ((bool)e.NewValue)
            {
                uie.Focus(); // Don't care about false values.
            }
        }
    }

}
