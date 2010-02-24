using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class BaseWindow : Window
    {
        //Value returned by the ShowDialog method.
        object _returnValue = null;

        public BaseWindow()
        {
            // Standard message to close the window
            Messenger.Default.Register<TMsgClose>(this, MsgClose_Handler);
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unregister the messages of the window and its view model.
            // This is very important, because the messages live outside their recipient.
            // If you register a message in the constructor of a window and you open the window twice
            // without having unregistered its message, that message will be registered (and handled) twice!
            Messenger.Default.Unregister(this);
            Messenger.Default.Unregister(this.DataContext);
            
            base.OnClosed(e);

        }

        public object ReturnValue
        {
            get
            {
                return this._returnValue;
            }
        }

        #region Messages handlers

        void MsgClose_Handler(TMsgClose msg)
        {
            // Close the window only if the message comes from the DataContext of this window
            if (msg.SenderViewModel == this.DataContext)
            {
                this._returnValue = msg.DialogReturnValue;
                this.Close();
            }
        }

        #endregion

    }
}
