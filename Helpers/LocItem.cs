using System;

namespace PDFRider
{
    //[System.Windows.Markup.DictionaryKeyProperty("Key")] this doesn't work so use "classic" x:Key property
    [System.Windows.Markup.MarkupExtensionReturnType(typeof(string))]
    [System.Windows.Markup.ContentProperty("LocalizedValue")]
    public class LocItem : System.Windows.Markup.MarkupExtension
    {
        public LocItem()
            : base()
        {
        }

        /// <summary>
        /// The key to identify this object.
        /// </summary>
        /// <remarks>This is actually unused, because DictionaryKeyProperty attribute
        /// doesn't seem to work...</remarks>
        public string Key { get; set; }

        /// <summary>
        /// Stores the original value (usually in English) set by the application developer(s)
        /// </summary>
        public string OriginalValue { get; set; }

        /// <summary>
        /// Stores the localized value
        /// </summary>
        public string LocalizedValue { get; set; }

        /// <summary>
        /// Indicates if the current object is deprecated (but it is still active for compatibility)
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// Stores comments set by developer(s)
        /// </summary>
        public string Comment { get; set; }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.LocalizedValue ?? "";
        }
    }
}
