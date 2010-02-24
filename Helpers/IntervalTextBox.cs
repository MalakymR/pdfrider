using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PDFRider
{
    /// <summary>
    /// A specific TextBox for page interval
    /// </summary>
    public class IntervalTextBox : TextBox
    {

        #region Properties

        #region IsValidInterval

        public static DependencyProperty IsValidIntervalProperty = DependencyProperty.RegisterAttached(
            "IsValidInterval", typeof(bool), typeof(IntervalTextBox),
            new FrameworkPropertyMetadata(true));

        public bool IsValidInterval
        {
            get { return (bool)GetValue(IntervalTextBox.IsValidIntervalProperty); }
            set { SetValue(IntervalTextBox.IsValidIntervalProperty, value); }
        }

        #endregion

        #region SelectAllText

        public static DependencyProperty SelectAllTextProperty = DependencyProperty.RegisterAttached(
            "SelectAllText", typeof(bool), typeof(IntervalTextBox),
            new FrameworkPropertyMetadata(false));

        public bool SelectAllText
        {
            get { return (bool)GetValue(IntervalTextBox.SelectAllTextProperty); }
            set { SetValue(IntervalTextBox.SelectAllTextProperty, value); }
        }

        #endregion

        #endregion



        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            if ((bool)GetValue(IntervalTextBox.SelectAllTextProperty))
                this.SelectAll();
        }

        protected override void OnGotMouseCapture(System.Windows.Input.MouseEventArgs e)
        {
            base.OnGotMouseCapture(e);

            if ((bool)GetValue(IntervalTextBox.SelectAllTextProperty))
                this.SelectAll();
        }
    }

}
