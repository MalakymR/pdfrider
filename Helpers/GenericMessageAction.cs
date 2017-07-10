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
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// Passes a generic message to a recipient and provides a built-in callback with a genric parameter.
    /// </summary>
    /// <typeparam name="TMessageData">Generic message data to pass to a recipient.</typeparam>
    /// <typeparam name="TCallbackParameter">Generic callback parameter.</typeparam>
    /// <remarks>This class combines GenericMessage of T and NotificationMessageAction of T classes
    /// of MVVM Light Toolkit to provide generic data with callback functionality.</remarks>
    public class GenericMessageAction<TMessageData, TCallbackParameter> : NotificationMessageAction<TCallbackParameter>
    {
        public GenericMessageAction(TMessageData data, Action<TCallbackParameter> callback)
            : base("", callback)
        {
            this.Data = data;
        }

        public GenericMessageAction(Action<TCallbackParameter> callback)
            : base("", callback)
        {
        }

        public TMessageData Data { get; set; }
    }
}
