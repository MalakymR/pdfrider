using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class WndExtractPagesViewModel : ViewModelBase
    {
        PDFDocument _doc;

        string _numberOfPages = "";
        string _pageStart = "";
        string _pageEnd = "";

        public WndExtractPagesViewModel()
            : base()
        {
            Messenger.Default.Register<TMsgShowExtractPages>(this, MsgShowExtractPages_Handler);
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
                RaisePropertyChanged("PageStart");
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
                RaisePropertyChanged("PageEnd");
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

        #region CmdExtract

        RelayCommand _cmdExtract;
        public ICommand CmdExtract
        {
            get
            {
                if (this._cmdExtract == null)
                {
                    this._cmdExtract = new RelayCommand(() => this.DoCmdExtract());
                }
                return this._cmdExtract;
            }
        }

        private void DoCmdExtract()
        {
            string tempFileName = App.Current.FindResource("loc_defaultTempFileName").ToString() + 
                this._doc.FileName;
            string tempFile = System.IO.Path.Combine(App.TEMP_DIR, tempFileName);

            int start = Int32.Parse(this.PageStart) - this._doc.PageLabelStart + 1;
            int end = Int32.Parse(this.PageEnd) - this._doc.PageLabelStart + 1;

            SelectionStates state = this._doc.ExtractPages(start, end, tempFile);

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

        void MsgShowExtractPages_Handler(TMsgShowExtractPages msg)
        {
            //Get the document...
            this._doc = msg.Document;

            //... and do some initializations.
            this.NumberOfPages = (this._doc.NumberOfPages + this._doc.PageLabelStart - 1).ToString() +
                " (" + this._doc.NumberOfPages.ToString() + ")";

            this.PageStart = this._doc.PageLabelStart.ToString();
            this.PageEnd = this._doc.PageLabelStart.ToString();
        }

        #endregion

    }
}
