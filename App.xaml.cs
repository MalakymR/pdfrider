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
        const string LOC_FILE_NAME = "LocTable-{0}.xaml";
        const string TEMP_DIR_NAME = "Temp";
        
        public static string NAME = App.ResourceAssembly.GetName().Name;
        public static string TITLE = "PDF Rider"; // I could use reflection here...
        public static string VERSION = String.Format("{0}.{1}.{2}",
            App.ResourceAssembly.GetName().Version.Major.ToString(),
            App.ResourceAssembly.GetName().Version.Minor.ToString(),
            App.ResourceAssembly.GetName().Version.Build.ToString());
        public static string FULL_VERSION = App.ResourceAssembly.GetName().Version.ToString();
        public static string WEBSITE = "http://pdfrider.codeplex.com";
        public static string AD_LINK = "http://www.babylon.com/welcome/index.html?affID=16195";

        //Locale directory (e.g. {app}\Languages\en-US\ )
        //The locale specific part is added in the constructor, after the localization test part.
        public static string LOC_DIR = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, App.LOC_DIR_NAME);

        //Temporary directory ( {app}\Temp\ )
        public static string TEMP_DIR = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            TEMP_DIR_NAME);

        public static string HOME_URI = ""; //Path.Combine(App.LOC_DIR, "");
        
        private MainWindow _mainWindow;

        public App()
        {
            #region Test localization

            // -- Remove or comment this block in the final version --

            //System.Globalization.CultureInfo enCulture = new System.Globalization.CultureInfo("es-ES");
            ////enCulture = new System.Globalization.CultureInfo("fr-FR");
            //System.Threading.Thread.CurrentThread.CurrentCulture = enCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = enCulture;

            #endregion


            App.LOC_DIR = Path.Combine(App.LOC_DIR, System.Threading.Thread.CurrentThread.CurrentCulture.ToString());


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
                Path.Combine(App.LOC_DIR, String.Format(App.LOC_FILE_NAME, cultureName)));

            //If the localized StringTable doesn't exist, the default (compiled)
            //LocTable will be used.
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
