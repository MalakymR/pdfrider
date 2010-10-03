using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Security;

namespace PDFRider
{
    /* PasswordBox.Password isn't implemented as a DependencyProperty for security reasons (the password
     * would be stored in memory as plain text).
     * I don't know why PasswordBox.SecurePassword isn't a DependencyProperty too, since it stores the
     * password in a SecureString object. 
     * This is an extension that implements a SecureString password as a DependencyProperty. */
    public class PasswordBoxExtension
    {
        public static readonly DependencyProperty BindableSecurePasswordProperty = DependencyProperty.RegisterAttached(
            "BindableSecurePassword", typeof(SecureString), typeof(PasswordBoxExtension),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                BindableSecurePasswordPropertyChanged));

        public static SecureString GetBindableSecurePassword(DependencyObject obj)
        {
            return (SecureString)obj.GetValue(BindableSecurePasswordProperty);
        }


        public static void SetBindableSecurePassword(DependencyObject obj, SecureString value)
        {
            obj.SetValue(BindableSecurePasswordProperty, value);
        }

        private static void BindableSecurePasswordPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pbx = o as PasswordBox;

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                pbx.PasswordChanged += Password_Changed;
            }
            if ((e.NewValue == null) && (e.OldValue != null))
            {
                pbx.PasswordChanged -= Password_Changed;
            }
        }

        private static void Password_Changed(object sender, EventArgs e)
        {
            PasswordBox pbx = sender as PasswordBox;

            SetBindableSecurePassword(pbx, pbx.SecurePassword);
        }

    }
}
