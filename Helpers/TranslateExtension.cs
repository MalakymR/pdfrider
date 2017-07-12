using System;

namespace PDFRider
{
    /* This markup extension is used in place of "DynamicResource" when you want to
     * reference a resource of type LocItem in a string property of a control
     * (e.g. Text property of TextBlock control).
     * 
     * For some reason (maybe a bug) Visual Studio 2010 Designer throw an exception
     * if you use DynamicResource, saying that the type of the referenced item is not
     * the same of the property type (of course LocItem returns a string!).
     * 
     * Using this extension the designer works fine. 
     * This extension returns an "object", so I wonder why this works 
     * while the DynamicResource method doesn't... 
     */

    public class TranslateExtension : System.Windows.Markup.MarkupExtension
    {
        public TranslateExtension(string key)
            : base()
        {
            this.Key = key;
        }

        /// <summary>
        /// The key of the translated resource
        /// </summary>
        public string Key { get; set; }

                
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            System.Windows.FrameworkElement e = new System.Windows.FrameworkElement();

            return e.TryFindResource(this.Key);
        }
    }
}
