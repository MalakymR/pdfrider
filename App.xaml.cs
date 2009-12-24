using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Linq;
using System.Windows;
using System.IO;

namespace PDFRider
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        const string PROCESS_NAME = "PDFRider";
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

            // Questo blocco deve essere eliminato nella versione definitiva

            System.Globalization.CultureInfo enCulture = new System.Globalization.CultureInfo("en-US");
            //enCulture = new System.Globalization.CultureInfo("it-IT");
            //System.Threading.Thread.CurrentThread.CurrentCulture = enCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = enCulture;

            #endregion

            if (!Directory.Exists(App.LOC_DIR))
                Directory.CreateDirectory(App.LOC_DIR);

            if (!Directory.Exists(App.TEMP_DIR))
                Directory.CreateDirectory(App.TEMP_DIR);
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
                catch { } //No action, the default StringTable.xaml is used
            }
        }

        //Deletes the temporary files.
        private void DeleteTempFiles()
        {
            foreach (string file in Directory.GetFiles(App.TEMP_DIR))
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        //Does some startup initializations and handles the command line arguments
        protected override void OnStartup(StartupEventArgs e)
        {
            SetLocalizedStrings();

            base.OnStartup(e);

            this._mainWindow = new MainWindow();

            //Gets the view model to initialize with command line arguments
            MainWindowViewModel mainWindowViewModel = this._mainWindow.DataContext as MainWindowViewModel;

            if (e.Args != null)
            {
                //Arguments args = new Arguments(e.Args);
                //System.Windows.MessageBox.Show(args.Count.ToString());
                //for (int i = 0; i < args.Count; i++)
                //{
                //    System.Windows.MessageBox.Show(args[""][0]);
                //}
                try
                {
                    //First argument is the path of the PDF file
                    if (e.Args.Length > 0)
                    {
                        mainWindowViewModel.Uri = e.Args[0];
                    }

                    //Second argument tells if the opened document may has not been saved
                    //(e.g. if the doc is opened after a page extraction ...)
                    if (e.Args.Length > 1)
                    {
                        mainWindowViewModel.IsDocumentChanged = bool.Parse(e.Args[1]);
                    }
                }
                catch
                {
                    // Just a temp message, so it isn't localized.
                    //  -- This isn't the final expected behaviour ;) --
                    System.Windows.MessageBox.Show("By now, you can open one file only. Sorry...");
                    this.Shutdown();
                    return;
                }
            }

            //Deletes the temporary files only if the temporary directory is not in use or
            //if there isn't another PDF Rider process running.
            if ((System.Diagnostics.Process.GetProcessesByName(App.PROCESS_NAME).Length == 0) &&
                (!mainWindowViewModel.Uri.StartsWith(App.TEMP_DIR)))
            {
                this.DeleteTempFiles();
            }

            this._mainWindow.Show();

        }

    }
}
