using System;
using System.Linq;
using System.Windows.Input;
using System.Security;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class WndConfirmPasswordViewModel : WindowViewModel
    {
        public WndConfirmPasswordViewModel()
        {
            Messenger.Default.Register<TMsgConfirmPassword>(this, MsgShowConfirmPassword_Handler);
        }

        SecureString _originalPassword;

        string _confirmPasswordMessage = "";
        public string ConfirmPasswordMessage
        {
            get
            {
                return this._confirmPasswordMessage;
            }
            set
            {
                this._confirmPasswordMessage = value;
                RaisePropertyChanged("ConfirmPasswordMessage");
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
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(this._originalPassword);
            string p1 = System.Runtime.InteropServices.Marshal.PtrToStringAuto(ptr);
            ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(this.Password);
            string p2 = System.Runtime.InteropServices.Marshal.PtrToStringAuto(ptr);

            if (p1 == p2)
            {
                Messenger.Default.Send<TMsgClose>(new TMsgClose(this, true));
            }

            p1 = p2 = null;
        }

        //private bool CanDoCmdConfirm
        //{
        //    get
        //    {
        //        return (this._password == this._originalPassword);
        //    }
        //}


        #endregion

        private void MsgShowConfirmPassword_Handler(TMsgConfirmPassword msg)
        {
            switch (msg.PasswordType)
            {
                case TMsgConfirmPassword.PasswordTypes.Open:
                    this.ConfirmPasswordMessage = App.Current.FindResource("loc_confirmOpenPasswordMessage").ToString();
                    break;
                case TMsgConfirmPassword.PasswordTypes.Edit:
                    this.ConfirmPasswordMessage = App.Current.FindResource("loc_confirmEditPasswordMessage").ToString();
                    break;
            }

            this._originalPassword = msg.OriginalPassword;
        }
    }
}
