using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public partial class WndSecurity : BaseWindow
    {
        public WndSecurity()
        {
            InitializeComponent();

            this.DataContext = new WndSecurityViewModel();

            Messenger.Default.Register<GenericMessageAction<TMsgConfirmPassword, bool>>(this, MsgShowConfirmPassword_Handler);
            Messenger.Default.Register<GenericMessageAction<TMsgEnterPassword, string>>(this, MsgShowEnterPassword_Handler);
        }

        private void MsgShowConfirmPassword_Handler(GenericMessageAction<TMsgConfirmPassword, bool> msg)
        {
            WndConfirmPassword wndConfirmPassword = new WndConfirmPassword();
            Messenger.Default.Send<TMsgConfirmPassword, WndConfirmPasswordViewModel>(msg.Data);

            wndConfirmPassword.Owner = this;
            wndConfirmPassword.ShowDialog();
            bool ret = wndConfirmPassword.ReturnValue == null ? false : (bool)wndConfirmPassword.ReturnValue;

            msg.Execute(ret);
        }

        private void MsgShowEnterPassword_Handler(GenericMessageAction<TMsgEnterPassword, string> msg)
        {
            WndEnterPassword wndEnterPassword = new WndEnterPassword();
            Messenger.Default.Send<TMsgEnterPassword, WndEnterPasswordViewModel>(msg.Data);

            wndEnterPassword.Owner = this;
            wndEnterPassword.ShowDialog();
            string ret = wndEnterPassword.ReturnValue as string;

            msg.Execute(ret);
        }
    }
}
