using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;

namespace PDFRider
{
    /// <summary>
    /// A view model for the files to merge
    /// </summary>
    public class FileToMergeViewModel : ViewModelBase
    {
        public enum PagesIntervalEnum
        {
            AllPages,
            Interval
        }

        System.Text.RegularExpressions.Regex _intervalRegex = new System.Text.RegularExpressions.Regex(
                @"^[0-9]+((-?,?[0-9]+)*)$"); // Checks for a valid pages interval

        string _fileName;
        string _uri;
        PagesIntervalEnum _pagesInterval;
        string _interval;
        bool _isValidInterval;

        public FileToMergeViewModel(string path)
        {
            this.PdfFile = new PDFFile(path);

            this.FullName = path;
            this.PagesInterval = PagesIntervalEnum.AllPages;
        }

        public PDFFile PdfFile { get; private set; }

        public string FileName
        {
            get
            {
                return this._fileName;
            }
            private set
            {
                this._fileName = value;
                RaisePropertyChanged("FileName");
            }
        }

        public string FullName
        {
            get
            {
                return this.PdfFile.FullName;
            }
            set
            {
                this.PdfFile.FullName = value;
                RaisePropertyChanged("FullName");

                this.FileName = System.IO.Path.GetFileName(value);

                // I use uri instead of path so I can specify parameters like below
                Uri uri = new Uri(value);
                this.Uri = uri.AbsoluteUri + "#toolbar=0";
            }
        }

        public string Uri
        {
            get
            {
                return this._uri;
            }
            private set
            {
                this._uri = value;
                RaisePropertyChanged("Uri");
            }
        }

        public PagesIntervalEnum PagesInterval
        {
            get
            {
                return this._pagesInterval;
            }
            set
            {
                this._pagesInterval = value;
                RaisePropertyChanged("PagesInterval");

                if (this._pagesInterval == PagesIntervalEnum.AllPages)
                {
                    this.Interval = App.Current.FindResource("loc_allPages").ToString();
                }
                else
                {
                    this.Interval = "";
                }
            }
        }

        public string Interval
        {
            get
            {
                return this._interval;
            }
            set
            {
                this._interval = value;
                RaisePropertyChanged("Interval");

                this.CheckInterval();

                // If I had specified a valid interval, I add the page ranges to the underlying PDFFile
                if (this.IsValidInterval)
                {
                    this.PdfFile.PageRanges.Clear();
                    if (this._interval != App.Current.FindResource("loc_allPages").ToString())
                    {
                        this.PdfFile.PageRanges.AddRange(this._interval.Split(','));
                    }
                }

            }
        }

        public bool IsValidInterval
        {
            get
            {
                return this._isValidInterval;
            }
            set
            {
                this._isValidInterval = value;
                RaisePropertyChanged("IsValidInterval");
            }
        }

        private void CheckInterval()
        {
            if (this._intervalRegex.IsMatch(this._interval))
            {
                this.IsValidInterval = true;
            }
            else
            {
                if (this._interval == App.Current.FindResource("loc_allPages").ToString())
                {
                    this.IsValidInterval = true;
                }
                else
                {
                    this.IsValidInterval = false;
                }
            }
        }
    }
}
