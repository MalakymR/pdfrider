using System;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class WndExtractPagesViewModel : WindowViewModel
    {
        PDFDocument _doc;

        string _pageStart;
        string _pageEnd;
        bool _isValidPageStart;
        bool _isValidPageEnd;

        System.Text.RegularExpressions.Regex _intervalRegex = new System.Text.RegularExpressions.Regex(
                @"^[0-9]+$"); // Accepts only numbers

        public WndExtractPagesViewModel()
            : base()
        {
            Messenger.Default.Register<TMsgShowExtractPages>(this, MsgShowExtractPages_Handler);

            this._pageStart = "1";
            this._pageEnd = "1";
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

        #region CmdExtract

        RelayCommand _cmdExtract;
        public ICommand CmdExtract
        {
            get
            {
                if (this._cmdExtract == null)
                {
                    this._cmdExtract = new RelayCommand(() => this.DoCmdExtract(),
                        () => CanDoCmdExtract);
                }
                return this._cmdExtract;
            }
        }

        private void DoCmdExtract()
        {
            string tempFileName = App.Current.FindResource("loc_tempPagesFrom").ToString() + 
                this._doc.FileName;
            string tempFile = System.IO.Path.Combine(App.TEMP_DIR, tempFileName);

            int start = Int32.Parse(this._pageStart) - this._doc.PageLabelStart + 1;
            int end = Int32.Parse(this._pageEnd) - this._doc.PageLabelStart + 1;

            PDFDocument.OperationStates state = this._doc.ExtractPages(start, end, tempFile);

            if (state == PDFDocument.OperationStates.PageRangeOutOfDocument)
            {
                this.Information = App.Current.FindResource("loc_msgOutOfDocument").ToString();
            }
            else
            {
                Messenger.Default.Send<TMsgClose>(new TMsgClose(this, tempFile));
            }
        }

        private bool CanDoCmdExtract
        {
            get
            {
                return ((this.IsValidPageStart) && (this.IsValidPageEnd));
            }
        }

        #endregion

        #endregion


        #region Message handlers

        void MsgShowExtractPages_Handler(TMsgShowExtractPages msg)
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
