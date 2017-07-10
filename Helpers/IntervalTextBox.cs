/*
 *    Copyright 2009-2011 Francesco Tonucci
 * 
 * This file is part of PDFRider.
 * 
 * PDFRider is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * PDFRider is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with PDFRider; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 * 
 * 
 * Project page: http://pdfrider.codeplex.com
*/

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
