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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFRider.Updater
{
    /// <summary>
    /// Stores useful informations for update checking
    /// </summary>
    public class VersionInfo
    {
        public VersionInfo()
        {
            this.CurrentVersion = App.VERSION;
            this.NewVersion = App.VERSION;
            this.NewVersionAvailable = false;
        }

        /// <summary>
        /// Version of the installed application
        /// </summary>
        public string CurrentVersion { get; set; }

        /// <summary>
        /// Latest version available
        /// </summary>
        public string NewVersion { get; set; }

        /// <summary>
        /// Indicates whether a new version is available
        /// </summary>
        public bool NewVersionAvailable { get; set; }
    }
}
