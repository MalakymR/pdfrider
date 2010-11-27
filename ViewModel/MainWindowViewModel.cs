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
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using System.Configuration;

namespace PDFRider
{
    public class MainWindowViewModel : WindowViewModel
    {
        //Original Uri of the document. It is compared with the current Uri to check if the document
        //may have been changed
        string _originalUri = "";

        PDFDocument _doc = null;
        
        string _homeUri = "";

        // Stores application settings
        PDFRiderConfig _config = null;

        
        public MainWindowViewModel()
            : base()
        {
            string homeUriPath = App.HOME_URI;
            if (System.IO.File.Exists(homeUriPath))
                this._homeUri = homeUriPath;

            //Initialize the properties
            this.WindowTitle = App.TITLE;
            this.Uri = this._homeUri;
            this.Files = new List<string>();
            
            this.StartupInitializations();
        }


        #region Properties

        /// <summary>
        /// Title of the main window. Displays the current file name.
        /// </summary>
        string _windowTitle;
        public string WindowTitle
        {
            get
            {
                return this._windowTitle;
            }
            set
            {
                this._windowTitle = value;
                RaisePropertyChanged("WindowTitle");
            }
        }

        /// <summary>
        /// Get the application version as string
        /// </summary>
        public string Version
        {
            get
            {
                return "ver. " + App.VERSION;
            }
        }

        /// <summary>
        /// Indicates that the document may be changed
        /// </summary>
        public bool IsDocumentChanged 
        {
            get
            {
                return this._doc.IsChanged;
            }
            set
            {
                this._doc.IsChanged = value;
                RaisePropertyChanged("IsDocumentChanged");
            }
        }

        /// <summary>
        /// Get or set the Uri of the (PDF) document to work with
        /// </summary>
        public string Uri
        {
            get
            {
                if (this._doc != null)
                    return this._doc.FullName;
                else
                    return "";
            }
            set
            {   
                this._doc = new PDFDocument(value);

                if (this._doc.FullName != this._homeUri)
                {
                    this.WindowTitle = System.IO.Path.GetFileName(this._doc.FileName) + " - " + App.TITLE;
                    if (this._doc.HasInfo)
                    {
                        this.ShowInfoBar = false;
                    }
                    else
                    {
                        this.Information = App.Current.FindResource("loc_invalidDocument").ToString(); 
                        // "Impossibile recuperare informazioni sul documento.\nIl documento potrebbe non essere valido oppure protetto da password.";
                        this.ShowInfoBar = true;
                    }
                }

                RaisePropertyChanged("Uri");
            }
        }

        /// <summary>
        /// Indicates if the info bar is visible
        /// </summary>
        bool _showInfoBar = false;
        public bool ShowInfoBar
        {
            get
            {
                return this._showInfoBar;
            }
            set
            {
                this._showInfoBar = value;
                RaisePropertyChanged("ShowInfoBar");
            }
        }

        /// <summary>
        /// Indicates whether to show the Ad
        /// </summary>
        bool _showAd = false;
        public bool ShowAd
        {
            get
            {
                return this._showAd;
            }
            set
            {
                this._showAd = value;
                RaisePropertyChanged("ShowAd");
            }
        }

        public List<string> Files { get; private set; }

        #endregion


        #region Commands

        #region CmdHome

        RelayCommand _cmdHome;
        public ICommand CmdHome
        {
            get
            {
                if (this._cmdHome == null)
                {
                    this._cmdHome = new RelayCommand(() => this.DoCmdHome());
                }
                return this._cmdHome;
            }
        }
        private void DoCmdHome()
        {
            this.Uri = this._homeUri;
        }

        #endregion

        #region CmdOpenDocument

        RelayCommand _cmdOpenDocument;
        public ICommand CmdOpenDocument
        {
            get
            {
                if (this._cmdOpenDocument == null)
                {
                    this._cmdOpenDocument = new RelayCommand(() => this.DoCmdOpenDocument());
                }
                return this._cmdOpenDocument;
            }
        }
        private void DoCmdOpenDocument()
        {
            // TODO: Ask for saving before open a new document

            GenericMessageAction<TMsgOpenFile, TMsgOpenFile> message = new GenericMessageAction<TMsgOpenFile, TMsgOpenFile>(
                new TMsgOpenFile()
                {
                    NewFile = true
                }, 
                this.MsgLoadFile_Handler);

            Messenger.Default.Send<GenericMessageAction<TMsgOpenFile, TMsgOpenFile>, MainWindow>(message);
        }

        #endregion

        #region CmdSaveDocument

        RelayCommand _cmdSaveDocument;
        public ICommand CmdSaveDocument
        {
            get
            {
                if (this._cmdSaveDocument == null)
                {
                    this._cmdSaveDocument = new RelayCommand(() => this.DoCmdSaveDocument(),
                        () => this.CanDoCmdSaveDocument );
                }
                return this._cmdSaveDocument;
            }
        }
        private void DoCmdSaveDocument()
        {

            GenericMessageAction<TMsgSaveFile, TMsgOpenFile> message = new GenericMessageAction<TMsgSaveFile, TMsgOpenFile>(
                new TMsgSaveFile(), x =>
                {
                    if (x.NewFile)
                    {
                        // Always overwrite. Don't check here if the file already exists!
                        System.IO.File.Copy(this.Uri, x.FileName, true);

                        this.MsgLoadFile_Handler(x);
                    }
                });

            Messenger.Default.Send<GenericMessageAction<TMsgSaveFile, TMsgOpenFile>, MainWindow>(message);
        }
        private bool CanDoCmdSaveDocument
        {
            get
            {
                return this.IsDocumentChanged;
            }
        }

        #endregion

        #region CmdClosing

        RelayCommand<System.ComponentModel.CancelEventArgs> _cmdClosing;
        public ICommand CmdClosing
        {
            get
            {
                if (this._cmdClosing == null)
                {
                    this._cmdClosing = new RelayCommand<System.ComponentModel.CancelEventArgs>(
                        x => this.DoCmdClosing(x));
                }
                return this._cmdClosing;
            }
        }
        private void DoCmdClosing(System.ComponentModel.CancelEventArgs e)
        {
            GenericMessageAction<TMsgClosing, TMsgClosing> message = new GenericMessageAction<TMsgClosing, TMsgClosing>(
                new TMsgClosing()
                {
                    AskForSave = this.IsDocumentChanged
                }, 
                x =>
                    {
                        // x.Data contains the file name chosen to save the file
                        // or null if no name was provided.
                        if ((!x.Cancel)&&(x.Data != null))
                        {
                            // Always overwrite. Don't check here if the file already exists!
                            System.IO.File.Copy(this.Uri, (string)x.Data, true);
                        }
                        e.Cancel = x.Cancel;
                    }
            );

            Messenger.Default.Send<GenericMessageAction<TMsgClosing, TMsgClosing>, MainWindow>(message);
        }

        #endregion

        #region CmdExtractPages

        RelayCommand _cmdExtractPages;
        public ICommand CmdExtractPages
        {
            get
            {
                if (this._cmdExtractPages == null)
                {
                    this._cmdExtractPages = new RelayCommand(() => this.DoCmdExtractPages(),
                        () => this.CanDoCmdExtractPages);
                }
                return this._cmdExtractPages;
            }
        }
        private void DoCmdExtractPages()
        {
            GenericMessageAction<TMsgShowExtractPages, TMsgOpenFile> message = new GenericMessageAction<TMsgShowExtractPages, TMsgOpenFile>(
                new TMsgShowExtractPages(this._doc), this.MsgLoadFile_Handler);

            Messenger.Default.Send<GenericMessageAction<TMsgShowExtractPages, TMsgOpenFile>, MainWindow>(message);
        }
        private bool CanDoCmdExtractPages
        {
            get
            {
                return ((this._doc != null) && (this._doc.HasInfo));
            }
        }

        #endregion

        #region CmdDeletePages

        RelayCommand _cmdDeletePages;
        public ICommand CmdDeletePages
        {
            get
            {
                if (this._cmdDeletePages == null)
                {
                    this._cmdDeletePages = new RelayCommand(() => this.DoCmdDeletePages(),
                        () => this.CanDoCmdDeletePages);
                }
                return this._cmdDeletePages;
            }
        }
        private void DoCmdDeletePages()
        {
            GenericMessageAction<TMsgShowDeletePages, TMsgOpenFile> message = new GenericMessageAction<TMsgShowDeletePages, TMsgOpenFile>(
                new TMsgShowDeletePages(this._doc), this.MsgLoadFile_Handler);

            Messenger.Default.Send<GenericMessageAction<TMsgShowDeletePages, TMsgOpenFile>, MainWindow>(message);
        }
        private bool CanDoCmdDeletePages
        {
            get
            {
                return ((this._doc != null) && (this._doc.HasInfo));
            }
        }

        #endregion

        #region CmdInsertPages

        RelayCommand _cmdInsertPages;
        public ICommand CmdInsertPages
        {
            get
            {
                if (this._cmdInsertPages == null)
                {
                    this._cmdInsertPages = new RelayCommand(() => this.DoCmdInsertPages(),
                        () => this.CanDoCmdInsertPages);
                }
                return this._cmdInsertPages;
            }
        }
        private void DoCmdInsertPages()
        {
            GenericMessageAction<TMsgShowInsertPages, TMsgOpenFile> message = new GenericMessageAction<TMsgShowInsertPages, TMsgOpenFile>(
                new TMsgShowInsertPages(this._doc), this.MsgLoadFile_Handler);

            Messenger.Default.Send<GenericMessageAction<TMsgShowInsertPages, TMsgOpenFile>, MainWindow>(message);

        }
        private bool CanDoCmdInsertPages
        {
            get
            {
                return ((this._doc != null) && (this._doc.HasInfo));
            }
        }

        #endregion

        #region CmdRotatePages

        RelayCommand _cmdRotatePages;
        public ICommand CmdRotatePages
        {
            get
            {
                if (this._cmdRotatePages == null)
                {
                    this._cmdRotatePages = new RelayCommand(() => this.DoCmdRotatePages(),
                        () => this.CanDoCmdRotatePages);
                }
                return this._cmdRotatePages;
            }
        }
        private void DoCmdRotatePages()
        {
            GenericMessageAction<TMsgShowRotatePages, TMsgOpenFile> message = new GenericMessageAction<TMsgShowRotatePages, TMsgOpenFile>(
                new TMsgShowRotatePages(this._doc), this.MsgLoadFile_Handler);

            Messenger.Default.Send<GenericMessageAction<TMsgShowRotatePages, TMsgOpenFile>, MainWindow>(message);

        }
        private bool CanDoCmdRotatePages
        {
            get
            {
                return ((this._doc != null) && (this._doc.HasInfo));
            }
        }

        #endregion

        #region CmdMergeDocuments

        RelayCommand _cmdMergeDocuments;
        public ICommand CmdMergeDocuments
        {
            get
            {
                if (this._cmdMergeDocuments == null)
                {
                    this._cmdMergeDocuments = new RelayCommand(() => this.DoCmdMergeDocuments());
                }
                return this._cmdMergeDocuments;
            }
        }
        private void DoCmdMergeDocuments()
        {
            TMsgShowMergeDocuments msg = new TMsgShowMergeDocuments();

            if ((this._doc != null) && (this._doc.HasInfo))
            {
                msg.FilesToMerge.Add(this._doc.FullName);
            }

            GenericMessageAction<TMsgShowMergeDocuments, TMsgOpenFile> message = new GenericMessageAction<TMsgShowMergeDocuments, TMsgOpenFile>(
                msg, this.MsgLoadFile_Handler);

            Messenger.Default.Send<GenericMessageAction<TMsgShowMergeDocuments, TMsgOpenFile>, MainWindow>(message);

        }
        

        #endregion

        #region CmdBurst

        RelayCommand _cmdBurst;
        public ICommand CmdBurst
        {
            get
            {
                if (this._cmdBurst == null)
                {
                    this._cmdBurst = new RelayCommand(() => this.DoCmdBurst(),
                        () => this.CanDoCmdBurst);
                }
                return this._cmdBurst;
            }
        }
        private void DoCmdBurst()
        {
            Messenger.Default.Send<TMsgShowBurst>(new TMsgShowBurst(this._doc));
        }
        private bool CanDoCmdBurst
        {
            get
            {
                return ((this._doc != null) && (this._doc.HasInfo));
            }
        }

        #endregion

        #region CmdSecurity

        RelayCommand _cmdSecurity;
        public ICommand CmdSecurity
        {
            get
            {
                if (this._cmdSecurity == null)
                {
                    this._cmdSecurity = new RelayCommand(() => this.DoCmdSecurity(),
                        () => this.CanDoCmdSecurity);
                }
                return this._cmdSecurity;
            }
        }
        private void DoCmdSecurity()
        {
            GenericMessageAction<TMsgShowSecurity, TMsgOpenFile> message = new GenericMessageAction<TMsgShowSecurity, TMsgOpenFile>(
                new TMsgShowSecurity(this._doc), this.MsgLoadFile_Handler);

            Messenger.Default.Send<GenericMessageAction<TMsgShowSecurity, TMsgOpenFile>, MainWindow>(message);

        }
        private bool CanDoCmdSecurity
        {
            get
            {
                return (this._doc != null);
            }
        }

        #endregion

        #region CmdCheckForUpdates

        RelayCommand _cmdCheckForUpdates;
        public ICommand CmdCheckForUpdates
        {
            get
            {
                if (this._cmdCheckForUpdates == null)
                {
                    this._cmdCheckForUpdates = new RelayCommand(() => this.DoCmdCheckForUpdates());
                }
                return this._cmdCheckForUpdates;
            }
        }
        private void DoCmdCheckForUpdates()
        {
            Updater.VersionInfo info = Updater.Updater.CheckForUpdates(App.VERSION);

            if (info != null)
            {
                if (info.NewVersionAvailable == true)
                {
                    Messenger.Default.Send<TMsgShowNewVersion>(new TMsgShowNewVersion(info));
                }
                else
                {
                    DialogMessage message = new DialogMessage(App.Current.FindResource("loc_programUpToDate").ToString(), null)
                    {
                        Button = System.Windows.MessageBoxButton.OK,
                        Icon = System.Windows.MessageBoxImage.Information,
                        Caption = App.NAME
                    };

                    Messenger.Default.Send<DialogMessage, MainWindow>(message);
                }
            }
            
        }

        #endregion

        #region CmdAbout

        RelayCommand _cmdAbout;
        public ICommand CmdAbout
        {
            get
            {
                if (this._cmdAbout == null)
                {
                    this._cmdAbout = new RelayCommand(() => this.DoCmdAbout());
                }
                return this._cmdAbout;
            }
        }
        private void DoCmdAbout()
        {
            Messenger.Default.Send<TMsgShowAbout>(new TMsgShowAbout());
        }

        #endregion

        #region CmdHideInfoBar

        RelayCommand _cmdHideInfoBar;
        public ICommand CmdHideInfoBar
        {
            get
            {
                if (this._cmdHideInfoBar == null)
                {
                    this._cmdHideInfoBar = new RelayCommand(() => this.DoCmdHideInfoBar());
                }
                return this._cmdHideInfoBar;
            }
        }
        private void DoCmdHideInfoBar()
        {
            this.ShowInfoBar = false;
        }

        #endregion

        #region CmdOpenAd

        RelayCommand _cmdOpenAd;
        public ICommand CmdOpenAd
        {
            get
            {
                if (this._cmdOpenAd == null)
                {
                    this._cmdOpenAd = new RelayCommand(() => this.DoCmdOpenAd());
                }
                return this._cmdOpenAd;
            }
        }
        private void DoCmdOpenAd()
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo(App.AD_LINK));
        }

        #endregion

        #endregion


        #region Message handlers

        void MsgLoadFile_Handler(TMsgOpenFile msg)
        {
            if ((msg.FileName != null) && (msg.FileName != "") &&
                (System.IO.File.Exists(msg.FileName)))
            {
                if (msg.NewWindow)
                {
                    PDFDocument.OpenWithPdfRider(msg.FileName, App.CLO_DOCUMENT_CHANGED);
                }
                else
                {
                    this.Uri = msg.FileName;

                    if (msg.NewFile)
                    {
                        this._originalUri = msg.FileName;
                    }
                    
                    if (this.Uri == this._originalUri)
                    {
                        this.IsDocumentChanged = false;
                    }
                    else
                    {
                        this.IsDocumentChanged = true;
                    }
                }

            }
        }

        #endregion


        #region Private (support) methods

        void StartupInitializations()
        {
            if ((!System.IO.File.Exists(PDFRiderConfig.CONFIG_FILE)) ||
                (!System.IO.File.Exists(Updater.Updater.CONFIG_FILE)))
            {
                DialogMessage message = new DialogMessage(App.Current.FindResource("loc_missingConfigFiles").ToString(), null)
                    {
                        Button = System.Windows.MessageBoxButton.OK,
                        Icon = System.Windows.MessageBoxImage.Error,
                        Caption = App.NAME
                    };

                Messenger.Default.Send<DialogMessage, MainWindow>(message);

                return;
            }

            // Handles command line arguments
            string[] args = Environment.GetCommandLineArgs();

            
            if (args.Length > 1)
            {
                // Define some variables to store informations passed via command line arguments
                bool documentChanged = false;

                // The first argument returned by Environment.GetCommandLineArgs() is the
                // program executable, so I bypass it.
                for (int i = 1; i < args.Length; i++)
                {
                    string a = args[i];
                    
                    // Handles simple command line options (e.g. "/C")
                    if (a.StartsWith("/")) // process the options
                    {
                        
                        switch (a)
                        {
                            case App.CLO_DOCUMENT_CHANGED:
                                documentChanged = true;
                                break;
                        }
                    }
                    else // get the file names
                        this.Files.Add(a);
                }

                if (this.Files.Count == 1)
                {
                    // Only one file: open it in the main window
                    this.Uri = this.Files[0];

                    // Set initial informations
                    this.IsDocumentChanged = documentChanged;
                }
            }

            // Load application configuration
            try
            {
                this._config = new PDFRiderConfig();
            }
            catch { }

            if (this._config != null)
            {
                // Show Ad?
                this.ShowAd = this._config.ShowAd;
            }
            
            // Operation to do only if the temporary directory is not in use or
            // if there isn't another PDF Rider process running.
            if ((System.Diagnostics.Process.GetProcessesByName(App.PROCESS_NAME).Length == 1) &&
                (!this.Uri.StartsWith(App.TEMP_DIR)))
                // Use the following lines for debug
                //if ((System.Diagnostics.Process.GetProcessesByName("PDFRider.vshost.exe").Length <= 1) &&
                //    (!this.Uri.StartsWith(App.TEMP_DIR)))
            {
                // Deletes the temporary files
                this.DeleteTempFiles();

                // Checks for update
                Updater.UpdaterConfig updaterConfig = Updater.Updater.LoadConfig();
                if ((updaterConfig != null) && (updaterConfig.CheckAtStartup))
                {
                    Updater.VersionInfo info = Updater.Updater.CheckForUpdates(App.VERSION);

                    if (info != null)
                    {
                        if (info.NewVersionAvailable == true)
                        {
                            Messenger.Default.Send<TMsgShowNewVersion>(new TMsgShowNewVersion(info));
                        }
                    }
                }

            }

            
            // Shows the window for merging documents, if more than one file is passed in Arguments list
            if (this.Files.Count > 1)
            {
                GenericMessageAction<TMsgShowMergeDocuments, TMsgOpenFile> message = new GenericMessageAction<TMsgShowMergeDocuments, TMsgOpenFile>(
                new TMsgShowMergeDocuments(this.Files), this.MsgLoadFile_Handler);

                Messenger.Default.Send<GenericMessageAction<TMsgShowMergeDocuments, TMsgOpenFile>, MainWindow>(message);
            }
        }

        //Deletes the temporary files.
        void DeleteTempFiles()
        {
            foreach (string file in System.IO.Directory.GetFiles(App.TEMP_DIR))
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch { }
            }
        }


        #endregion

    }
}
