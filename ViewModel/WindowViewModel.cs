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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// A base class for the view models of windows
    /// </summary>
    public abstract class WindowViewModel : ViewModelBase
    {
        public WindowViewModel()
        {
            this.ReturnValue = null;
        }

        #region Properties

        public object ReturnValue { get; set; }

        public string Information
        {
            get
            {
                return this._information;
            }
            set
            {
                this._information = value;
                RaisePropertyChanged("Information");
            }
        }
        string _information = "";
        

        #endregion


        #region Commands

        #region CmdClose

        RelayCommand _cmdClose;
        public ICommand CmdClose
        {
            get
            {
                if (this._cmdClose == null)
                {
                    this._cmdClose = new RelayCommand(() => this.DoCmdClose());
                }
                return this._cmdClose;
            }
        }
        private void DoCmdClose()
        {
            this.OnCmdClose();
        }

        #endregion

        #endregion

        protected virtual void OnCmdClose()
        {
            Messenger.Default.Send<MsgClose>(new MsgClose(this));
        }

        /// <summary>
        /// Provides an easy way to close the view model and his relative window
        /// </summary>
        protected void Close()
        {
            this.OnCmdClose();
        }

        /// <summary>
        /// Provides an easy way to close the view model and his relative window
        /// </summary>
        /// <param name="return_value">Value returned by the dialog window</param>
        protected void Close(object returnValue)
        {
            this.ReturnValue = returnValue;
            this.OnCmdClose();
        }
    }
}
