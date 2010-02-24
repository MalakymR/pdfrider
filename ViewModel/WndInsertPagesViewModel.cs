using System;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// </summary>
    public class WndInsertPagesViewModel : WindowViewModel
    {
        PDFDocument _doc;

        string _pageStart;
        bool _isValidPageStart;
        PDFDocument.InsertPositions _insertPosition;
        string _fileToMerge;
        string _fileNameToMerge;

        System.Text.RegularExpressions.Regex _intervalRegex = new System.Text.RegularExpressions.Regex(
                @"^[0-9]+$"); // Accepts only numbers

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
        public int NumberOfPages { get; private set; }
        public int NumberOfPhysicalPages { get; private set; }


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

                this.InsertPosition = PDFDocument.InsertPositions.Custom;

                if ((this._intervalRegex.IsMatch(this._pageStart)) &&
                    (Int32.Parse(this._pageStart) >= 0) &&
                    (Int32.Parse(this._pageStart) <= this.NumberOfPages))
                {
                    this.IsValidPageStart = true;
                }
                else
                {
                    this.IsValidPageStart = false;
                }

                RaisePropertyChanged("PageStart");

                if (this.IsValidPageStart) this.Information = "";
                else this.Information = App.Current.FindResource("loc_correctErrors").ToString();
            }
        }

        /// <summary>
        /// Indicates if the value for PageStart is valid
        /// </summary>
        public bool IsValidPageStart
        {
            get
            {
                return this._isValidPageStart;
            }
            set
            {
                this._isValidPageStart = value;
                RaisePropertyChanged("IsValidPageStart");
            }
        }


        /// <summary>
        /// Position to start insert from.
        /// </summary>
        public PDFDocument.InsertPositions InsertPosition
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

        #region CmdInsert

        RelayCommand _cmdInsert;
        public ICommand CmdInsert
        {
            get
            {
                if (this._cmdInsert == null)
                {
                    this._cmdInsert = new RelayCommand(() => this.DoCmdInsert(),
                        () => CanDoCmdInsert);
                }
                return this._cmdInsert;
            }
        }

        private void DoCmdInsert()
        {
            string tempFileName = App.Current.FindResource("loc_tempPagesFrom").ToString() +
                this._doc.FileName;
            string tempFile = System.IO.Path.Combine(App.TEMP_DIR, tempFileName);

            int start = Int32.Parse(this.PageStart) - this._doc.PageLabelStart + 1;

            PDFDocument.OperationStates state = this._doc.InsertPages(start, this._insertPosition, 
                this._fileToMerge, tempFile);

            if (state == PDFDocument.OperationStates.PageRangeOutOfDocument)
            {
                this.Information = App.Current.FindResource("loc_msgOutOfDocument").ToString();
            }
            else
            {
                Messenger.Default.Send<TMsgClose>(new TMsgClose(this, tempFile));
            }
        }

        private bool CanDoCmdInsert
        {
            get
            {
                return this.IsValidPageStart;
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
            this.NumberOfPages = (this._doc.NumberOfPages + this._doc.PageLabelStart - 1);
            this.NumberOfPhysicalPages = this._doc.NumberOfPages;

            this.PageStart = this._doc.PageLabelStart.ToString();

            this.FileToMerge = msg.FileToMerge;
            this.FileNameToMerge = System.IO.Path.GetFileName(this._fileToMerge);

            this.InsertPosition = PDFDocument.InsertPositions.End;
        }

        #endregion

    }
}
