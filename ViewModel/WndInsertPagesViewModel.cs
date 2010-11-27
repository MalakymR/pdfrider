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
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public class WndInsertPagesViewModel : ToolWindowViewModel
    {
        PDFDocument _doc;

        PDFDocument.InsertPositions _insertPosition;
        string _fileToMerge;
        string _fileNameToMerge;

        
        /// <summary>
        /// Initializes a new instance of the WndInsertPagesViewModel class.
        /// </summary>
        public WndInsertPagesViewModel()
        {
            Messenger.Default.Register<TMsgShowInsertPages>(this, MsgShowInsertPages_Handler);
        }

        #region Properties

        /// <summary>
        /// Position to start insert from.
        /// </summary>
        public PDFDocument.InsertPositions InsertPosition
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

            PDFDocument.OperationStates state = this._doc.InsertPages(start, this._insertPosition, 
                this._fileToMerge, ref tempFile);

            if (state == PDFDocument.OperationStates.PageRangeOutOfDocument)
            {
                this.Information = App.Current.FindResource("loc_msgOutOfDocument").ToString();
            }
            else
            {
                Messenger.Default.Send<TMsgClose>(new TMsgClose(this, tempFile));
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


        #region Message handlers

        void MsgShowInsertPages_Handler(TMsgShowInsertPages msg)
        {
            //Get the document...
            this._doc = msg.Document;

            //... and do some initializations.
            this.NumberOfPages = (this._doc.NumberOfPages + this._doc.PageLabelStart - 1);
            this.NumberOfPhysicalPages = this._doc.NumberOfPages;

            this.PageStart = this._doc.PageLabelStart.ToString();
            this.PageEnd = this.NumberOfPhysicalPages.ToString();

            this.FileToMerge = msg.FileToMerge;
            this.FileNameToMerge = System.IO.Path.GetFileName(this._fileToMerge);

            this.InsertPosition = PDFDocument.InsertPositions.End;
        }

        #endregion

    }
}
