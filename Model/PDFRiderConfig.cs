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
using System.Xml;

namespace PDFRider
{
    /// <summary>
    /// Reads, stores and saves values from the configuration file.
    /// </summary>
    public class PDFRiderConfig
    {
        internal const string CONFIG_FILE_NAME = "pdfrider.config";
        internal static string CONFIG_FILE = System.IO.Path.Combine(
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), App.NAME),
            CONFIG_FILE_NAME);

        XmlDocument _doc = null;

        /// <summary>
        /// Creates a new istance of PDFRiderConfig, loading values from the configuration file.
        /// </summary>
        public PDFRiderConfig()
        {
            this._doc = new XmlDocument();
            this._doc.Load(CONFIG_FILE);

            this.ShowAd = bool.Parse(this._doc.DocumentElement.SelectSingleNode("./ShowAd").InnerXml);
        }


        public bool ShowAd { get; set; }


        /// <summary>
        /// Saves the values to the file.
        /// </summary>
        public void Save()
        {
            this._doc.DocumentElement.SelectSingleNode("./ShowAd").InnerXml = this.ShowAd.ToString();

            this._doc.Save(CONFIG_FILE);
        }
    }
}
