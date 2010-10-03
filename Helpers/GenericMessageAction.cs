using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
