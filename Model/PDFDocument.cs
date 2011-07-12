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
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace PDFRider
{
    /// <summary>
    /// Contains properties and methods for working with the PDF document.
    /// </summary>
    public class PDFDocument
    {

        public PDFDocument(string fileName)
        {
            this.FullName = fileName;

            this.LoadInfo(fileName);
        }

        /// <summary>
        /// Get the informations from the pdf file
        /// </summary>
        /// <param name="pdfFilePath">Path of the pdf file to get information from</param>
        private void LoadInfo(string pdfFilePath)
        {
            PDFFileInfo info = PDFActions.GetPDFFileInfo(pdfFilePath);
            this.NumberOfPages = info.NumberOfPages;
            this.PageLabelStart = info.PageLabelStart;
            this.PageRanges = new List<string>();

            this.HasInfo = true;
            if (this.NumberOfPages == 0)
            {
                this.HasInfo = false;
            }
        }
        
        #region Properties

        /// <summary>
        /// Tells if it was possible to get informations about the pdf file
        /// using the dump_data function. If this value is FALSE, no operation can be
        /// done on the file.
        /// </summary>
        public bool HasInfo { get; private set; }

        /// <summary>
        /// Full path of the pdf file.
        /// </summary>
        public string FullName { get; private set; }
        
        
        /// <summary>
        /// Name of the pdf file.
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileName(this.FullName);
            }
        }

        /// <summary>
        /// Total number of pages of the document.
        /// </summary>
        public int NumberOfPages { get; private set; }

        /// <summary>
        /// There can be 20 pages in a document, but the page counting could start from 50.
        /// In this case, PageLabelStart will be 50.
        /// You need to know this value because if a user selects, for example, 54 as start page
        /// you need to subtract this value from it in order to obtain the actual page to pass to pdftk.
        /// </summary>
        public int PageLabelStart { get; private set; }

        /// <summary>
        /// Selected page ranges (e.g. "2-4, 7, 8")
        /// </summary>
        public List<string> PageRanges { get; private set; }

        /// <summary>
        /// Indicates whether this document id changed
        /// </summary>
        public bool IsChanged { get; set; }


        #endregion


        #region public methods

        /// <summary>
        /// Open the current PDF document with the default application.
        /// </summary>
        public void Open()
        {
            Process p = new Process();

            p.StartInfo.FileName = this.FullName;

            p.Start();
        }

        /// <summary>
        /// Open a PDF document with the default application.
        /// </summary>
        /// <param name="fileName">Path of the file to open</param>
        public static void Open(string fileName)
        {
            Process p = new Process();

            p.StartInfo.FileName = fileName;

            p.Start();
        }

        /// <summary>
        /// Open a PDF document with PDFRider.
        /// </summary>
        /// <param name="fileName">Path of the file to open</param>
        public static void OpenWithPdfRider(string fileName, string args)
        {
            Process p = new Process();

            p.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "PDFRider.exe");
            p.StartInfo.Arguments = "\"" + fileName + "\" " + args;

            p.Start();
        }

        /// <summary>
        /// Checks the extension of a file to see if it is a pdf.
        /// </summary>
        /// <param name="path">Path of the file to check</param>
        /// <returns></returns>
        public static bool PDFCheck(string path)
        {
            if (Path.GetExtension(path).ToUpperInvariant() == ".pdf".ToUpperInvariant())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }


    
}
