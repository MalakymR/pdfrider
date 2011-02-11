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
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.IO;

namespace PDFRider
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public class WndDeletePagesViewModel : ToolWindowViewModel
    {
        PDFDocument _doc;

        /// <summary>
        /// Initializes a new instance of the WndDeletePagesViewModel class.
        /// </summary>
        public WndDeletePagesViewModel()
        {
            Messenger.Default.Register<TMsgShowDeletePages>(this, MsgShowDeletePages_Handler);
        }


        #region Commands

        #region CmdDelete

        RelayCommand _cmdDelete;
        public ICommand CmdDelete
        {
            get
            {
                if (this._cmdDelete == null)
                {
                    this._cmdDelete = new RelayCommand(() => this.DoCmdDelete(),
                        () => CanDoCmdDelete);
                }
                return this._cmdDelete;
            }
        }

        private void DoCmdDelete()
        {
            string tempFile = System.IO.Path.Combine(App.TEMP_DIR, this._doc.FileName);

            int start = Int32.Parse(this.PageStart) - this._doc.PageLabelStart + 1;
            int end = Int32.Parse(this.PageEnd) - this._doc.PageLabelStart + 1;

            PDFDocument.OperationStates state = this._doc.DeletePages(start, end, ref tempFile, false);

            if (state == PDFDocument.OperationStates.PageRangeOutOfDocument)
            {
                this.Information = App.Current.FindResource("loc_msgOutOfDocument").ToString();
            }
            else if (state == PDFDocument.OperationStates.PageRangeEntireDocument)
            {
                this.Information = App.Current.FindResource("loc_msgEntireDocument").ToString();
            }
            else
            {
                Messenger.Default.Send<TMsgClose>(new TMsgClose(this, tempFile));
            }
        }

        private bool CanDoCmdDelete
        {
            get
            {
                return ((this.IsValidPageStart) && (this.IsValidPageEnd));
            }
        }


        #endregion

        #endregion

        #region Message handlers

        void MsgShowDeletePages_Handler(TMsgShowDeletePages msg)
        {
            //Get the document...
            this._doc = msg.Document;

            //... and do some initializations.
            this.NumberOfPages = (this._doc.NumberOfPages + this._doc.PageLabelStart - 1);
            this.NumberOfPhysicalPages = this._doc.NumberOfPages;

            this.PageStart = this._doc.PageLabelStart.ToString();
            this.PageEnd = this._doc.PageLabelStart.ToString();

        }

        #endregion

    }
}