using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public partial class App : Application
    {
        #region Command Line Options
        
        // Tells that the document may has been changed
        public const string CLO_DOCUMENT_CHANGED = "/C";

        #endregion

        public const string PROCESS_NAME = "PDFRider";
        const string LOC_DIR_NAME = "Locales";
        const string BASE_LOC_FILE_NAME = "LocTable-";
        const string LOC_FILE_NAME_EXT = ".xaml";
        const string TEMP_DIR_NAME = "Temp";

        public static string NAME = App.ResourceAssembly.GetName().Name;
        public static string TITLE = "PDF Rider";
        public static string VERSION = String.Format("{0}.{1}",
            App.ResourceAssembly.GetName().Version.Major.ToString(),
            App.ResourceAssembly.GetName().Version.Minor.ToString());
        public static string FULL_VERSION = App.ResourceAssembly.GetName().Version.ToString();

        //Locale directory (e.g. {app}\Languages\en-US\ )
        public static string LOC_DIR = Path.Combine(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, App.LOC_DIR_NAME),
            System.Threading.Thread.CurrentThread.CurrentCulture.ToString());

        //Temporary directory ( {app}\Temp\ )
        public static string TEMP_DIR = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            TEMP_DIR_NAME);
        
        private MainWindow _mainWindow;

        public App()
        {
            #region Test localization

            // -- Remove or comment this block in the final version --

            //System.Globalization.CultureInfo enCulture = new System.Globalization.CultureInfo("en-US");
            //enCulture = new System.Globalization.CultureInfo("it-IT");
            //System.Threading.Thread.CurrentThread.CurrentCulture = enCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = enCulture;

            #endregion

            // Verifies the directory structure
            if (!Directory.Exists(App.LOC_DIR))
                Directory.CreateDirectory(App.LOC_DIR);
            
            if (!Directory.Exists(App.TEMP_DIR))
                Directory.CreateDirectory(App.TEMP_DIR);
        }


        // Shows the main window.
        // Command line arguments are handled via Environment.GetCommandLineArgs() in MainWindowViewModel
        protected override void OnStartup(StartupEventArgs e)
        {
            // This must be called after the initialization
            SetLocalizedStrings();

            this._mainWindow = new MainWindow();
            this._mainWindow.Show();
        }


        //Gets the localized strings file and adds it to the the application resources
        private void SetLocalizedStrings()
        {
            ResourceDictionary dictionary = new ResourceDictionary();
            string cultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            Uri languageUri = new Uri(
                Path.Combine(App.LOC_DIR, App.BASE_LOC_FILE_NAME + cultureName + App.LOC_FILE_NAME_EXT));

            //If the localized StringTable doesn't exist, the default (compiled)
            //StringTable will be used.
            if (File.Exists(languageUri.LocalPath))
            {
                try
                {
                    dictionary.Source = languageUri;

                    this.Resources.MergedDictionaries.Add(dictionary);
                }
                catch { } //No action, the default LocTable.xaml is used
            }
        }

    }
}
