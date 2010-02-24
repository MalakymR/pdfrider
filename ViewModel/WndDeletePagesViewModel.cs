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
    public class WndDeletePagesViewModel : WindowViewModel
    {
        PDFDocument _doc;

        string _pageStart = "";
        string _pageEnd = "";
        bool _isValidPageStart;
        bool _isValidPageEnd;

        System.Text.RegularExpressions.Regex _intervalRegex = new System.Text.RegularExpressions.Regex(
                @"^[0-9]+$"); // Accepts only numbers

        /// <summary>
        /// Initializes a new instance of the WndDeletePagesViewModel class.
        /// </summary>
        public WndDeletePagesViewModel()
        {
            Messenger.Default.Register<TMsgShowDeletePages>(this, MsgShowDeletePages_Handler);
        }

        #region Properties

        /// <summary>
        /// The total number of pages of the document.
        /// </summary>
        public int NumberOfPages { get; private set; }
        public int NumberOfPhysicalPages { get; private set; }

        /// <summary>
        /// Number of page to start extracting from.
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

                if ((this._intervalRegex.IsMatch(this._pageStart)) &&
                    (Int32.Parse(this._pageStart) > 0) &&
                    (Int32.Parse(this._pageStart) <= this.NumberOfPages))
                {
                    this.IsValidPageStart = true;
                }
                else
                {
                    this.IsValidPageStart = false;
                }

                RaisePropertyChanged("PageStart");

                if ((this.IsValidPageStart) && (this.IsValidPageEnd)) this.Information = "";
                else this.Information = App.Current.FindResource("loc_correctErrors").ToString();
            }

        }

        /// <summary>
        /// Number of page to end extracting to.
        /// </summary>
        public string PageEnd
        {
            get
            {
                return this._pageEnd;
            }
            set
            {
                this._pageEnd = value;

                if ((this._intervalRegex.IsMatch(this._pageEnd)) &&
                    (Int32.Parse(this._pageEnd) > 0) &&
                    (Int32.Parse(this._pageEnd) <= this.NumberOfPages))
                {
                    this.IsValidPageEnd = true;
                }
                else
                {
                    this.IsValidPageEnd = false;
                }

                RaisePropertyChanged("PageEnd");

                if ((this.IsValidPageStart) && (this.IsValidPageEnd)) this.Information = "";
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
        /// Indicates if the value for PageEnd is valid
        /// </summary>
        public bool IsValidPageEnd
        {
            get
            {
                return this._isValidPageEnd;
            }
            set
            {
                this._isValidPageEnd = value;
                RaisePropertyChanged("IsValidPageEnd");
            }
        }

        #endregion

        #region Commands

        #region CmdDelete

        RelayCommand _cmdDelete;
        public ICommand CmdDelete
        {
            get
            {
                if (this._cmdDelete == null)
                {
                    this._cmdDelete = new RelayCommand(() => this.DoCmdDelete(),
                        () => CanDoCmdDelete);
                }
                return this._cmdDelete;
            }
        }

        private void DoCmdDelete()
        {
            string tempFileName = App.Current.FindResource("loc_tempPagesFrom").ToString() +
                this._doc.FileName;
            string tempFile = System.IO.Path.Combine(App.TEMP_DIR, tempFileName);

            int start = Int32.Parse(this.PageStart) - this._doc.PageLabelStart + 1;
            int end = Int32.Parse(this.PageEnd) - this._doc.PageLabelStart + 1;

            PDFDocument.OperationStates state = this._doc.DeletePages(start, end, tempFile, false);

            if (state == PDFDocument.OperationStates.PageRangeOutOfDocument)
            {
                this.Information = App.Current.FindResource("loc_msgOutOfDocument").ToString();
            }
            else if (state == PDFDocument.OperationStates.PageRangeEntireDocument)
            {
                this.Information = App.Current.FindResource("loc_msgEntireDocument").ToString();
            }
            else
            {
                Messenger.Default.Send<TMsgClose>(new TMsgClose(this, tempFile));
            }
        }

        private bool CanDoCmdDelete
        {
            get
            {
                return ((this.IsValidPageStart) && (this.IsValidPageEnd));
            }
        }


        #endregion

        #endregion

        #region Message handlers

        void MsgShowDeletePages_Handler(TMsgShowDeletePages msg)
        {
            //Get the document...
            this._doc = msg.Document;

            //... and do some initializations.
            this.NumberOfPages = (this._doc.NumberOfPages + this._doc.PageLabelStart - 1);
            this.NumberOfPhysicalPages = this._doc.NumberOfPages;

            this.PageStart = this._doc.PageLabelStart.ToString();
            this.PageEnd = this._doc.PageLabelStart.ToString();
        }

        #endregion

    }
}