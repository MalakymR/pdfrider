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
using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// A base class for windows that use MVVM
    /// </summary>
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
            this.OnMsgClose(msg);
        }

        #endregion

        protected virtual void OnMsgClose(TMsgClose msg)
        {
            // Close the window only if the message comes from the DataContext of this window
            if (msg.SenderViewModel == this.DataContext)
            {
                this._returnValue = msg.DialogReturnValue;
                this.Close();
            }
        }

    }
}
