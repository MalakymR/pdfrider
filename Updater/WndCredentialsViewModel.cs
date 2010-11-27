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
using System.Security;
using System.IO;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider.Updater
{
    public class WndCredentialsViewModel : WindowViewModel
    {
        public WndCredentialsViewModel()
        {
            Messenger.Default.Register<TMsgAskForCredentials>(this, MsgAskForCredentials_Handler);
        }

        //SecureString _originalPassword;

        string _credentialsMessage = "";
        public string CredentialsMessage
        {
            get
            {
                return this._credentialsMessage;
            }
            set
            {
                this._credentialsMessage = value;
                RaisePropertyChanged("CredentialsMessage");
            }
        }


        string _username = "";
        public string Username
        {
            get
            {
                return this._username;
            }
            set
            {
                this._username = value;
                RaisePropertyChanged("Username");
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


        bool _saveCredentials = false;
        public bool SaveCredentials
        {
            get
            {
                return this._saveCredentials;
            }
            set
            {
                this._saveCredentials = value;
                RaisePropertyChanged("SaveCredentials");
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
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(this.Password);
            string password = System.Runtime.InteropServices.Marshal.PtrToStringAuto(ptr);

            if (this.SaveCredentials == true)
            {
                if (File.Exists(Updater.CREDENTIAL_FILE))
                    File.Delete(Updater.CREDENTIAL_FILE);

                XmlDocument credentialsDoc = new XmlDocument();
                credentialsDoc.AppendChild(credentialsDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

                XmlElement documentElement = credentialsDoc.CreateElement("Update");

                XmlElement xe = credentialsDoc.CreateElement("username");
                xe.InnerXml = this.Username;
                documentElement.AppendChild(xe);

                xe = credentialsDoc.CreateElement("password");
                xe.InnerXml = CryptoDLL.FTCrypt.Encrypt(password);
                documentElement.AppendChild(xe);

                credentialsDoc.AppendChild(documentElement);

                credentialsDoc.Save(Updater.CREDENTIAL_FILE);
            }

            Messenger.Default.Send<TMsgClose>(
                new TMsgClose(this, new System.Net.NetworkCredential(this.Username, password)));
        }


        #endregion

        private void MsgAskForCredentials_Handler(TMsgAskForCredentials msg)
        {
            this.CredentialsMessage = String.Format(App.Current.FindResource("loc_wndCredentialsMessage").ToString(),
                msg.Proxy.Address.ToString());
        }
    }
}
