using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class MainWindowViewModel : WindowViewModel
    {
        //Original Uri of the document. It is compared with the current Uri to check if the document
        //may have been changed
        string _originalUri = "";

        PDFDocument _doc = null;
        
        bool _isDocumentChanged = false;

        string _homeUri = "";

        public MainWindowViewModel()
            : base()
        {
            //Register some messages
            Messenger.Default.Register<TMsgLoadFile>(this, MsgLoadFile_Handler);

            string homeUriPath = System.IO.Path.Combine(App.LOC_DIR, "home.html");
            if (System.IO.File.Exists(homeUriPath))
                this._homeUri = homeUriPath;

            //Initialize the properties
            this.Uri = this._homeUri;
            this.Files = new List<string>();

            this.StartupInitializations();
        }

        #region Properties

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
                return this._isDocumentChanged;
            }
            set
            {
                this._isDocumentChanged = value;
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
                RaisePropertyChanged("Uri");
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
            Messenger.Default.Send<TMsgOpenFile, MainWindow>(new TMsgOpenFile());
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
            Messenger.Default.Send<TMsgClosing, MainWindow>(new TMsgClosing()
            {
                AskForSave = this.IsDocumentChanged,
                EventArgs = e
            });

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
            Messenger.Default.Send<TMsgShowExtractPages>(new TMsgShowExtractPages(this._doc));
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
            Messenger.Default.Send<TMsgShowDeletePages>(new TMsgShowDeletePages(this._doc));
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
            Messenger.Default.Send<TMsgShowInsertPages>(new TMsgShowInsertPages(this._doc));
        }
        private bool CanDoCmdInsertPages
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
            
            if ((this._doc != null)&&(this._doc.HasInfo))
            {
                msg.FilesToMerge.Add(this._doc.FullName);
            }

            Messenger.Default.Send<TMsgShowMergeDocuments>(msg);
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

        #endregion


        #region Message handlers

        void MsgLoadFile_Handler(TMsgLoadFile msg)
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
                    //if ((msg.FileName != null) && (msg.FileName != ""))
                    //{
                        this.Uri = msg.FileName;
                    //}

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
            // Handles command line arguments
            string[] args = Environment.GetCommandLineArgs();
            
            if (args.Length > 1)
            {
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
                                this.IsDocumentChanged = true;
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
                }
            }

            //Deletes the temporary files only if the temporary directory is not in use or
            //if there isn't another PDF Rider process running.
            if ((System.Diagnostics.Process.GetProcessesByName(App.PROCESS_NAME).Length == 0) &&
                (!this.Uri.StartsWith(App.TEMP_DIR)))
            {
                this.DeleteTempFiles();
            }

            // Shows the window for merging documents, if more than one file is passed in Arguments list
            if (this.Files.Count > 1)
            {
                Messenger.Default.Send<TMsgShowMergeDocuments>(
                    new TMsgShowMergeDocuments(this.Files));
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
