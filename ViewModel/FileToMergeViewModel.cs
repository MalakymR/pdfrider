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

        static System.Text.RegularExpressions.Regex _intervalRegex = new System.Text.RegularExpressions.Regex(
                @"^[0-9]+((-?,?[0-9]+)*)$"); /* Checks for a valid pages interval:
                                              *     1. Start with a number 
                                              *     2. Can have 0 or more combination
                                              *        of "-" or "," characters, followed by a number
                                              */
                                               

        string _fileName;
        string _uri;
        PagesIntervalEnum _pagesInterval;
        string _interval;
        bool _isValidInterval;

        
        public FileToMergeViewModel(string path)
        {
            this.PdfDocument = new PDFDocument(path);
            
            this.FullName = path;
            this.PagesInterval = PagesIntervalEnum.AllPages;
        }

        public PDFDocument PdfDocument { get; private set; }

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
                return this.PdfDocument.FullName;
            }
            set
            {
                this.PdfDocument = new PDFDocument(value);
                RaisePropertyChanged("FullName");

                this.FileName = System.IO.Path.GetFileName(value);

                // I use uri instead of path so I can specify parameters like below
                Uri uri = new Uri(value);
                this.Uri = uri.AbsoluteUri + "#toolbar=0"; // Maybe this works only with Adobe Reader ?
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
                    this.PdfDocument.PageRanges.Clear();
                    if (this._interval != App.Current.FindResource("loc_allPages").ToString())
                    {
                        this.PdfDocument.PageRanges.AddRange(this._interval.Split(','));
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
            if (_intervalRegex.IsMatch(this._interval))
            {
                // Start with true...
                this.IsValidInterval = true;

                // ...then check if the values are inside the document length
                string[] ranges = this._interval.Split(',');
                string[] subranges;
                foreach (string range in ranges)
                {
                    subranges = range.Split('-');
                    foreach (string subrange in subranges)
                    {
                        if ((short.Parse(subrange.ToString()) <= 0) ||
                            (short.Parse(subrange.ToString()) > this.PdfDocument.NumberOfPages))
                        {
                            this.IsValidInterval = false;
                            break;
                        }
                    }
                }
                
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