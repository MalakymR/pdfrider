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
using System.Security;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class WndEnterPasswordViewModel : WindowViewModel
    {
        public WndEnterPasswordViewModel()
        {
            Messenger.Default.Register<TMsgEnterPassword>(this, MsgShowEnterPassword_Handler);
        }

        PDFDocument _doc;

        //string _originalPassowrd;

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
            PDFDocument.OperationStates state = this._doc.TryDecrypt(this._password);

            if (state == PDFDocument.OperationStates.WrongPassword)
            {
                this.Information = App.Current.FindResource("loc_enterPasswordInformation").ToString();
            }
            else
            {
                Messenger.Default.Send<TMsgClose>(new TMsgClose(this, this._password));
            }
        }


        #endregion

        private void MsgShowEnterPassword_Handler(TMsgEnterPassword msg)
        {
            this._doc = msg.Document;

            //this._originalPassowrd = msg.OriginalPassword;
        }
    }
}
