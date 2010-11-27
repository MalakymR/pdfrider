/*
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
using System.Linq;
using System.Text;

namespace PDFRider
{
    public static class DialogController
    {
        
        /// <summary>
        /// Shows a OpenFileDialog window with standard settings for this application.
        /// </summary>
        /// <param name="multiselect">Indicates whether OpenFileDialog should allow multiple file selection</param>
        /// <returns>A structure containing information returned by a CommonDialog window.</returns>
        public static FileDialogResult ShowOpenFileDialog(bool multiselect)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = "pdf";
            ofd.Multiselect = multiselect;
            ofd.Title = App.Current.FindResource("loc_openFileDialogTitle").ToString();
            ofd.Filter = App.Current.FindResource("loc_openFileDialogFilter").ToString();

            FileDialogResult result = new FileDialogResult();
            result.CommonDialogReturn = (bool)ofd.ShowDialog();
            result.FileName = ofd.FileName;
            result.FileNames = ofd.FileNames;

            return result;
        }

        public static FileDialogResult ShowOpenFileDialog()
        {
            return ShowOpenFileDialog(false);
        }

        /// <summary>
        /// Shows a SaveFileDialog window with standard settings for this application.
        /// </summary>
        /// <returns>A structure containing information returned by a CommonDialog window.</returns>
        public static FileDialogResult ShowSaveFileDialog()
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Title = App.Current.FindResource("loc_saveFileDialogTitle").ToString();
            sfd.DefaultExt = "pdf";
            sfd.AddExtension = true;
            sfd.Filter = App.Current.FindResource("loc_saveFileDialogFilter").ToString();

            FileDialogResult result = new FileDialogResult();
            result.CommonDialogReturn = (bool)sfd.ShowDialog();
            result.FileName = sfd.FileName;
            
            return result;
        }

        
    }


    public struct FileDialogResult
    {
        public string[] FileNames { get; set; }
        public string FileName { get; set; }
        public bool CommonDialogReturn { get; set; }
    }
}
