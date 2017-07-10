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
    public partial class WndAbout : Window
    {
        public WndAbout()
        {
            InitializeComponent();

            this.AppName = App.TITLE;
            this.AppVersion = App.VERSION;

            // A ViewModel is very useless for this window!
            this.DataContext = this;
        }

        public string AppName { get; private set; }
        public string AppVersion { get; private set; }

        // To open link in wpf window applications you must handle this event like this
        private void linkCodePlex_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo(App.WEBSITE));

            e.Handled = true;
        }
    }
}
