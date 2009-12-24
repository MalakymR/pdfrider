using System;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// Logica di interazione per WndInsertPages.xaml
    /// </summary>
    public partial class WndInsertPages : Window
    {
        //Value returned by the ShowDialog method.
        object _returnValue = null;

        public WndInsertPages()
        {
            InitializeComponent();

            Messenger.Default.Register<TMsgClose>(this, MsgClose_Handler);
            Messenger.Default.Register<TMsgInformation>(this, MsgInformation_Handler);


        }

        #region Messages handlers

        void MsgClose_Handler(TMsgClose msg)
        {
            if (msg.SenderViewModel == this.DataContext)
            {
                this._returnValue = msg.DialogReturnValue;
                this.Close();
            }
        }

        void MsgInformation_Handler(TMsgInformation msg)
        {
            tbkInfo.Text = msg.Text;
        }

        #endregion

        public new string ShowDialog()
        {
            base.ShowDialog();

            return this._returnValue == null ? "" : this._returnValue.ToString();
        }

    }
}
