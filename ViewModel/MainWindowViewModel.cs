using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class MainWindowViewModel : ViewModelBase
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

            
        }

        #region Properties

        /// <summary>
        /// Get the application version as string
        /// </summary>
        public string Version
        {
            get
            {
                return "ver. " + App.ResourceAssembly.GetName().Version.ToString();
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
            Messenger.Default.Send<TMsgOpenFile>(new TMsgOpenFile()
            {
                FileType = "pdf"
            });
        }

        #endregion

        #region CmdClose

        RelayCommand _cmdClose;
        public ICommand CmdClose
        {
            get
            {
                if (this._cmdClose == null)
                {
                    this._cmdClose = new RelayCommand(() => this.DoCmdClose());
                }
                return this._cmdClose;
            }
        }
        private void DoCmdClose()
        {
            Messenger.Default.Send<TMsgClose>(new TMsgClose(this));
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
            Messenger.Default.Send<TMsgClosing>(new TMsgClosing(this)
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
            if ((msg.NewWindow) &&
                (msg.FileName != null) && (msg.FileName != ""))
            {
                PDFDocument.OpenWithPdfRider(msg.FileName, "true");
            }
            else
            {
                if ((msg.FileName != null) && (msg.FileName != ""))
                {
                    this.Uri = msg.FileName;
                }

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

        #endregion

    }
}
