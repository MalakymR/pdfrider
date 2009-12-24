using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PDFRider
{
    /// <summary>
    /// Useful stuff for WebBrowser control
    /// </summary>
    public static class WebBrowserUtility
    {
        // The Source property of WebBrowser control is not bindable, so I made an attached property
        // (named BindableSource) that can be binded to a data source and sets the control Source property 
        // accordingly.
        #region BindableSourceProperty

        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                string uri = e.NewValue as string;
                browser.Source = ((uri != null)&&(uri != "")) ? new Uri(uri) : null;
            }
        }

        #endregion

    }

}
