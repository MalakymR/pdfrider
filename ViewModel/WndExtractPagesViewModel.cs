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
    public class WndExtractPagesViewModel : ToolWindowViewModel
    {
        PDFDocument _doc;

        public WndExtractPagesViewModel()
            : base()
        {
            Messenger.Default.Register<TMsgShowExtractPages>(this, MsgShowExtractPages_Handler);
        }

        
        #region Commands

        #region CmdExtract

        RelayCommand _cmdExtract;
        public ICommand CmdExtract
        {
            get
            {
                if (this._cmdExtract == null)
                {
                    this._cmdExtract = new RelayCommand(() => this.DoCmdExtract(),
                        () => CanDoCmdExtract);
                }
                return this._cmdExtract;
            }
        }

        private void DoCmdExtract()
        {
            string tempFileName = String.Format(App.Current.FindResource("loc_tempPagesFrom").ToString(),
                    System.IO.Path.GetFileNameWithoutExtension(this._doc.FileName)) +
                    System.IO.Path.GetExtension(this._doc.FileName);
            string tempFile = System.IO.Path.Combine(App.TEMP_DIR, tempFileName);

            int start = Int32.Parse(this.PageStart) - this._doc.PageLabelStart + 1;
            int end = Int32.Parse(this.PageEnd) - this._doc.PageLabelStart + 1;

            PDFDocument.OperationStates state = this._doc.ExtractPages(start, end, ref tempFile);

            if (state == PDFDocument.OperationStates.PageRangeOutOfDocument)
            {
                this.Information = App.Current.FindResource("loc_msgOutOfDocument").ToString();
            }
            else
            {
                Messenger.Default.Send<TMsgClose>(new TMsgClose(this, tempFile));
            }
        }

        private bool CanDoCmdExtract
        {
            get
            {
                return ((this.IsValidPageStart) && (this.IsValidPageEnd));
            }
        }

        #endregion

        #endregion


        #region Message handlers

        void MsgShowExtractPages_Handler(TMsgShowExtractPages msg)
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
