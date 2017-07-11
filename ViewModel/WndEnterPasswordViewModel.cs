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

using System.Windows.Input;
using System.Security;
using GalaSoft.MvvmLight.Command;

namespace PDFRider
{
    public class WndEnterPasswordViewModel : WindowViewModel
    {
        PDFDocument _doc;

        public WndEnterPasswordViewModel(PDFDocument document)
        {
            this._doc = document;
        }

        
        string _enterPasswordMessage = "";
        public string EnterPasswordMessage
        {
            get
            {
                return this._enterPasswordMessage;
            }
            set
            {
                this._enterPasswordMessage = value;
                RaisePropertyChanged("EnterPasswordMessage");
            }
        }


        SecureString _password = new SecureString();
        public SecureString Password
        {
            get
            {
                return this._password;
            }
            set
            {
                this._password = value;
                RaisePropertyChanged("Password");
            }
        }

        #region CmdConfirm

        RelayCommand _cmdConfirm;
        public ICommand CmdConfirm
        {
            get
            {
                if (this._cmdConfirm == null)
                {
                    this._cmdConfirm = new RelayCommand(() => this.DoCmdConfirm());
                }
                return this._cmdConfirm;
            }
        }

        private void DoCmdConfirm()
        {
            PDFActions pdfActions = new PDFActions();
            PDFActions.OperationStates state = pdfActions.TryDecrypt(this._doc, this._password);

            if (state == PDFActions.OperationStates.WrongPassword)
            {
                this.Information = App.Current.FindResource("loc_enterPasswordInformation").ToString();
            }
            else
            {
                this.Close(this._password);
            }
        }


        #endregion

     
    }
}
