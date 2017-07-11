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
using System.Security;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class WndSecurityViewModel : WindowViewModel
    {
        PDFDocument _doc;

        public WndSecurityViewModel(PDFDocument document)
        {
            //Get the document...
            this._doc = document;

            //... and do some initializations.
            this.IsUnlocked = this._doc.HasInfo;
        }

        #region Properties

        /* If the document is crypted (locked), you must remove the old password
         * before setting a new one */
        bool _isUnlocked = true;
        public bool IsUnlocked
        {
            get
            {
                return this._isUnlocked;
            }
            set
            {
                this._isUnlocked = value;
                RaisePropertyChanged("IsUnlocked");
            }
        }

        bool _setOpenPassword = false;
        public bool SetOpenPassword
        {
            get
            {
                return this._setOpenPassword;
            }
            set
            {
                this._setOpenPassword = value;
                RaisePropertyChanged("SetOpenPassword");
            }
        }

        SecureString _openPassword = new SecureString();
        public SecureString OpenPassword
        {
            get
            {
                return this._openPassword;
            }
            set
            {
                this._openPassword = value;
                RaisePropertyChanged("OpenPassword");
            }
        }

        bool _setEditPassword = false;
        public bool SetEditPassword
        {
            get
            {
                return this._setEditPassword;
            }
            set
            {
                this._setEditPassword = value;
                RaisePropertyChanged("SetEditPassword");
            }
        }

        SecureString _editPassword = new SecureString();
        public SecureString EditPassword
        {
            get
            {
                return this._editPassword;
            }
            set
            {
                this._editPassword = value;
                RaisePropertyChanged("EditPassword");
            }
        }

        bool _allowPrinting = false;
        public bool AllowPrinting
        {
            get
            {
                return this._allowPrinting;
            }
            set
            {
                this._allowPrinting = value;
                RaisePropertyChanged("AllowPrinting");
            }
        }

        bool _allowDegradatedPrinting = false;
        public bool AllowDegradatedPrinting
        {
            get
            {
                return this._allowDegradatedPrinting;
            }
            set
            {
                this._allowDegradatedPrinting = value;
                RaisePropertyChanged("AllowDegradatedPrinting");
            }
        }

        bool _allowModifyContents = false;
        public bool AllowModifyContents
        {
            get
            {
                return this._allowModifyContents;
            }
            set
            {
                this._allowModifyContents = value;
                if (value == true)
                {
                    this.AllowAssembly = true;
                }
                RaisePropertyChanged("AllowModifyContents");
            }
        }

        bool _allowAssembly = false;
        public bool AllowAssembly
        {
            get
            {
                return this._allowAssembly;
            }
            set
            {
                this._allowAssembly = value;
                RaisePropertyChanged("AllowAssembly");
            }
        }

        bool _allowCopyContents = false;
        public bool AllowCopyContents
        {
            get
            {
                return this._allowCopyContents;
            }
            set
            {
                this._allowCopyContents = value;
                if (value == true)
                {
                    this.AllowScreenReaders = true;
                }
                RaisePropertyChanged("AllowCopyContents");
            }
        }

        bool _allowScreenReaders = false;
        public bool AllowScreenReaders
        {
            get
            {
                return this._allowScreenReaders;
            }
            set
            {
                this._allowScreenReaders = value;
                RaisePropertyChanged("AllowScreenReaders");
            }
        }

        bool _allowModifyAnnotations = false;
        public bool AllowModifyAnnotations
        {
            get
            {
                return this._allowModifyAnnotations;
            }
            set
            {
                this._allowModifyAnnotations = value;
                if (value == true)
                {
                    this.AllowFillIn = true;
                }
                RaisePropertyChanged("AllowModifyAnnotations");
            }
        }

        bool _allowFillIn = false;
        public bool AllowFillIn
        {
            get
            {
                return this._allowFillIn;
            }
            set
            {
                this._allowFillIn = value;
                RaisePropertyChanged("AllowFillIn");
            }
        }

        bool _allowAll = false;
        public bool AllowAll
        {
            get
            {
                return this._allowAll;
            }
            set
            {
                this._allowAll = value;
                if (value == true)
                {
                    this.AllowPrinting = true;
                    this.AllowDegradatedPrinting = true;
                    this.AllowModifyContents = true;
                    this.AllowAssembly = true;
                    this.AllowCopyContents = true;
                    this.AllowScreenReaders = true;
                    this.AllowModifyAnnotations = true;
                    this.AllowFillIn = true;
                }
                RaisePropertyChanged("AllowAll");
            }
        }

        #endregion


        #region Commands

        #region CmdSetPasswords

        RelayCommand _cmdSetPasswords;
        public ICommand CmdSetPasswords
        {
            get
            {
                if (this._cmdSetPasswords == null)
                {
                    this._cmdSetPasswords = new RelayCommand(() => this.DoCmdSetPasswords(),
                        () => CanDoCmdSetPasswords());
                }
                return this._cmdSetPasswords;
            }
        }

        private void DoCmdSetPasswords()
        {
            SecureString confirmedOpenPassword = null;
            SecureString confirmedEditPassword = null;

            if (this.SetOpenPassword == true)
            {
                GenericMessageAction<MsgShowConfirmPassword, bool> message = new GenericMessageAction<MsgShowConfirmPassword,bool>(
                    new MsgShowConfirmPassword(this._openPassword, PasswordTypes.Open),
                    x => confirmedOpenPassword = (x == true) ? this._openPassword : null);

                Messenger.Default.Send(message);
            }

            if (this.SetEditPassword == true)
            {
                GenericMessageAction<MsgShowConfirmPassword, bool> message = new GenericMessageAction<MsgShowConfirmPassword, bool>(
                    new MsgShowConfirmPassword(this._editPassword, PasswordTypes.Edit),
                    x => confirmedEditPassword = (x == true) ? this._editPassword : null);

                Messenger.Default.Send(message);
            }

            if ((confirmedOpenPassword != null) || (confirmedEditPassword != null))
            {
                string tempFileName = String.Format(App.Current.FindResource("loc_tempEncrypted").ToString(),
                    System.IO.Path.GetFileNameWithoutExtension(this._doc.FileName)) +
                    System.IO.Path.GetExtension(this._doc.FileName);
                string tempFile = System.IO.Path.Combine(App.TEMP_DIR, tempFileName);

                PDFActions pdfActions = new PDFActions();
                PDFActions.OperationStates state = pdfActions.Encrypt(this._doc, confirmedOpenPassword, confirmedEditPassword,
                    this.AllowPrinting, this.AllowDegradatedPrinting, this.AllowModifyContents,
                    this.AllowAssembly, this.AllowCopyContents, this.AllowScreenReaders, this.AllowModifyAnnotations,
                    this.AllowFillIn, this.AllowAll, ref tempFile);

                this.Close(tempFile);
            }
        }

        private bool CanDoCmdSetPasswords()
        {
            return (this.SetOpenPassword || this.SetEditPassword);
        }

        #endregion

        #region CmdRemovePasswords

        RelayCommand _cmdRemovePasswords;
        public ICommand CmdRemovePasswords
        {
            get
            {
                if (this._cmdRemovePasswords == null)
                {
                    this._cmdRemovePasswords = new RelayCommand(() => this.DoCmdRemovePasswords());
                }
                return this._cmdRemovePasswords;
            }
        }

        private void DoCmdRemovePasswords()
        {
            System.Security.SecureString password = new SecureString();


            GenericMessageAction<MsgShowEnterPassword, System.Security.SecureString> message = new GenericMessageAction<MsgShowEnterPassword, System.Security.SecureString>(
                new MsgShowEnterPassword(this._doc),
                x => password = x);

            Messenger.Default.Send(message);
            

            if (password != null)
            {
                string tempFileName = String.Format(App.Current.FindResource("loc_tempDecrypted").ToString(),
                    System.IO.Path.GetFileNameWithoutExtension(this._doc.FileName)) +
                    System.IO.Path.GetExtension(this._doc.FileName);
                string tempFile = System.IO.Path.Combine(App.TEMP_DIR, tempFileName);

                PDFActions pdfActions = new PDFActions();
                PDFActions.OperationStates state = pdfActions.Decrypt(this._doc, password, ref tempFile);

                this.Close(tempFile);
            }
        }


        #endregion

        #endregion

    }
}
