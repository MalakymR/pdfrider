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
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace PDFRider
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public class WndInsertPagesViewModel : ToolWindowViewModel
    {
        PDFDocument _doc;

        PDFActions.InsertPositions _insertPosition;
        string _fileToMerge;
        string _fileNameToMerge;

        
        /// <summary>
        /// Initializes a new instance of the WndInsertPagesViewModel class.
        /// </summary>
        public WndInsertPagesViewModel(PDFDocument document, string fileToMerge)
        {
            //Get the document...
            this._doc = document;

            //... and do some initializations.
            this.NumberOfPages = (this._doc.NumberOfPages + this._doc.PageLabelStart - 1);
            this.NumberOfPhysicalPages = this._doc.NumberOfPages;

            this.PageStart = this._doc.PageLabelStart.ToString();
            this.PageEnd = this.NumberOfPhysicalPages.ToString();

            this.FileToMerge = fileToMerge;
            this.FileNameToMerge = System.IO.Path.GetFileName(this._fileToMerge);

            this.InsertPosition = PDFActions.InsertPositions.End;
        }

        #region Properties

        /// <summary>
        /// Position to start insert from.
        /// </summary>
        public PDFActions.InsertPositions InsertPosition
        {
            get
            {
                return this._insertPosition;
            }
            set
            {
                this._insertPosition = value;
                RaisePropertyChanged("InsertPosition");
            }

        }

        /// <summary>
        /// Path of the file to merge with this document.
        /// </summary>
        public string FileToMerge
        {
            get
            {
                return this._fileToMerge;
            }
            set
            {
                this._fileToMerge = value;
                RaisePropertyChanged("FileToMerge");
            }
        }


        /// <summary>
        /// Name of the file to merge with this document.
        /// </summary>
        public string FileNameToMerge
        {
            get
            {
                return this._fileNameToMerge;
            }
            set
            {
                this._fileNameToMerge = value;
                RaisePropertyChanged("FileNameToMerge");
            }

        }

        #endregion


        #region Commands

        #region CmdInsert

        RelayCommand _cmdInsert;
        public ICommand CmdInsert
        {
            get
            {
                if (this._cmdInsert == null)
                {
                    this._cmdInsert = new RelayCommand(() => this.DoCmdInsert(),
                        () => CanDoCmdInsert);
                }
                return this._cmdInsert;
            }
        }

        private void DoCmdInsert()
        {
            string tempFile = System.IO.Path.Combine(App.TEMP_DIR, this._doc.FileName);

            int start = Int32.Parse(this.PageStart) - this._doc.PageLabelStart + 1;

            PDFActions pdfActions = new PDFActions();
            PDFActions.OperationStates state = pdfActions.InsertPages(this._doc, start, this._insertPosition, 
                this._fileToMerge, ref tempFile);

            if (state == PDFActions.OperationStates.PageRangeOutOfDocument)
            {
                this.Information = App.Current.FindResource("loc_msgOutOfDocument").ToString();
            }
            else
            {
                this.Close(tempFile);
            }
        }

        private bool CanDoCmdInsert
        {
            get
            {
                return this.IsValidPageStart;
            }
        }

        #endregion

        #endregion


    }
}
