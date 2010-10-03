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
