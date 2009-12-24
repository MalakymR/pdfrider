using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public class WndInsertPagesViewModel : ViewModelBase
    {
        PDFDocument _doc;

        string _numberOfPages = "";
        string _pageStart = "0";
        InsertPositions _insertPosition = InsertPositions.End;
        string _fileToMerge = "";
        string _fileNameToMerge = "";

        /// <summary>
        /// Initializes a new instance of the WndInsertPagesViewModel class.
        /// </summary>
        public WndInsertPagesViewModel()
        {
            Messenger.Default.Register<TMsgShowInsertPages>(this, MsgShowInsertPages_Handler);
        }

        #region Properties

        /// <summary>
        /// The total number of pages of the document.
        /// </summary>
        public string NumberOfPages
        {
            get
            {
                return this._numberOfPages;
            }
            set
            {
                this._numberOfPages = value;
                RaisePropertyChanged("NumberOfPages");
            }

        }

        /// <summary>
        /// Number of the page to start insert from.
        /// </summary>
        public string PageStart
        {
            get
            {
                return this._pageStart;
            }
            set
            {
                this._pageStart = value;
                RaisePropertyChanged("PageStart");
            }

        }

        /// <summary>
        /// Position to start insert from.
        /// </summary>
        public InsertPositions InsertPosition
        {
            get
            {
                return this._insertPosition;
            }
            set
            {
                this._insertPosition = value;
                RaisePropertyChanged("InsertPosition");
            }

        }

        /// <summary>
        /// Path of the file to merge with this document.
        /// </summary>
        public string FileToMerge
        {
            get
            {
                return this._fileToMerge;
            }
            set
            {
                this._fileToMerge = value;
                RaisePropertyChanged("FileToMerge");
            }
        }


        /// <summary>
        /// Name of the file to merge with this document.
        /// </summary>
        public string FileNameToMerge
        {
            get
            {
                return this._fileNameToMerge;
            }
            set
            {
                this._fileNameToMerge = value;
                RaisePropertyChanged("FileNameToMerge");
            }

        }

        #endregion


        #region Commands

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
            Messenger.Default.Send<TMsgClose>(new TMsgClose(this, ""));
        }

        #endregion

        #region CmdInsert

        RelayCommand _cmdInsert;
        public ICommand CmdInsert
        {
            get
            {
                if (this._cmdInsert == null)
                {
                    this._cmdInsert = new RelayCommand(() => this.DoCmdInsert());
                }
                return this._cmdInsert;
            }
        }

        private void DoCmdInsert()
        {
            string tempFileName = App.Current.FindResource("loc_defaultTempFileName").ToString() +
                this._doc.FileName;
            string tempFile = System.IO.Path.Combine(App.TEMP_DIR, tempFileName);

            int start = Int32.Parse(this.PageStart) - this._doc.PageLabelStart + 1;
            //int end = Int32.Parse(this.PageEnd) - this._doc.PageLabelStart + 1;

            SelectionStates state = this._doc.InsertPages(start, this._insertPosition, 
                this._fileToMerge, tempFile);

            if (state == SelectionStates.OutOfDocument)
            {
                Messenger.Default.Send<TMsgInformation>(new TMsgInformation(
                    App.Current.FindResource("loc_msgOutOfDocument").ToString()));
            }
            else
            {
                Messenger.Default.Send<TMsgClose>(new TMsgClose(this, tempFile));
            }
        }


        #endregion

        #endregion

        #region Message handlers

        void MsgShowInsertPages_Handler(TMsgShowInsertPages msg)
        {
            //Get the document...
            this._doc = msg.Document;

            //... and do some initializations.
            this.NumberOfPages = (this._doc.NumberOfPages + this._doc.PageLabelStart - 1).ToString();

            this.PageStart = this._doc.PageLabelStart.ToString();

            this.FileToMerge = msg.FileToMerge;
            this.FileNameToMerge = System.IO.Path.GetFileName(this._fileToMerge);
        }

        #endregion

    }
}
