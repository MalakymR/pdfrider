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

namespace PDFRider
{
    public partial class MainWindow : BaseWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        #region DragDrop events

        /* **********************************************************************************************
         * The WebBrowser control has its own drag-drop implementations. It does not raise a drop event
         * that I can implement. So when the cursor enter the main window with a drag operation, I hide 
         * the WebBrowser, letting the window reacts to the drop event.
         * When the drag-drop operation is finished, I show the WebBrowser again.
         * ********************************************************************************************** */

        // If I am dragging something to the window, hide the browser
        private void BaseWindow_DragEnter(object sender, DragEventArgs e)
        {
            this.browser.Visibility = Visibility.Hidden;
        }

        // If I am not dragging make the browser visible again
        private void BaseWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.browser.Visibility == Visibility.Hidden)
                this.browser.Visibility = Visibility.Visible;
        }

        #endregion

    }
}
