﻿/*
 *    Copyright 2009, 2010 Francesco Tonucci
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
                uie.Focus();
            }
        }
    }

}
