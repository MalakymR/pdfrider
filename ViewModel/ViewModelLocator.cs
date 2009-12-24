/*
 * In App.xaml:
 * <Application.Resources>
 *     <vm:ViewModelLocator xmlns:vm="clr-namespace:PDFRider"
 *                                  x:Key="Locator" />
 * </Application.Resources>
 * 
 * In the View:
 * DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
 * 
 * OR (WPF only):
 * 
 * xmlns:vm="clr-namespace:PDFRider"
 * DataContext="{Binding Source={x:Static vm:ViewModelLocator.ViewModelNameStatic}}"
*/

namespace PDFRider
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
    /// to this locator.
    /// </para>
    /// <para>
    /// In Silverlight and WPF, place the ViewModelLocator in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:ViewModelLocator xmlns:vm="clr-namespace:PDFRider"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// You can also use Blend to do all this with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// <para>
    /// In <strong>*WPF only*</strong> (and if databinding in Blend is not relevant), you can delete
    /// the ViewModelName property and bind to the ViewModelNameStatic property instead:
    /// </para>
    /// <code>
    /// xmlns:vm="clr-namespace:PDFRider"
    /// DataContext="{Binding Source={x:Static vm:ViewModelLocator.ViewModelNameStatic}}"
    /// </code>
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ////if (ViewModelBase.IsInDesignMode)
            ////{
            ////    // Create design time view models
            ////}
            ////else
            ////{
            ////    // Create run time view models
            ////}
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return new MainWindowViewModel();
            }
        }

        public WndExtractPagesViewModel WndExtractPagesViewModel
        {
            get
            {
                return new WndExtractPagesViewModel();
            }
        }

        public WndDeletePagesViewModel WndDeletePagesViewModel
        {
            get
            {
                return new WndDeletePagesViewModel();
            }
        }

        public WndInsertPagesViewModel WndInsertPagesViewModel
        {
            get
            {
                return new WndInsertPagesViewModel();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Dispose()
        {
            // Call ClearViewModelName() for each ViewModel.
        }
    }
}