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
using System.Windows.Input;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// Base class for tool windows (Extract, Insert, etc.). They have some common properties and behaviors.
    /// </summary>
    public abstract class ToolWindowViewModel : WindowViewModel
    {
        public struct PageInterval
        {
            public PDFDocument.PageIntervals Id;
            public string Description { get; set; }
        }

        static System.Text.RegularExpressions.Regex _intervalRegex = new System.Text.RegularExpressions.Regex(
                @"^[0-9]+$"); // Accepts only numbers

        

        #region Properties

        /// <summary>
        /// The total number of pages of the document. This value depends on "PageLabelStart" attribute of
        /// PDF document.
        /// </summary>
        public int NumberOfPages { get; protected set; }

        /// <summary>
        /// The total number of physical pages of the document.
        /// </summary>
        public int NumberOfPhysicalPages { get; protected set; }

        /// <summary>
        /// Number of page to start elaboration from.
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

                if ((_intervalRegex.IsMatch(this._pageStart)) &&
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
        string _pageStart = "";


        /// <summary>
        /// Number of page to end elaboration to.
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

                if ((_intervalRegex.IsMatch(this._pageEnd)) &&
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
        string _pageEnd = "";


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
        bool _isValidPageStart;


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
        bool _isValidPageEnd;


        /// <summary>
        /// Available page intervals: All pages, Even pages, Odd pages
        /// </summary>
        public ObservableCollection<PageInterval> PageIntervals { get; protected set; }

        /// <summary>
        /// The current (selected) page interval to use in page rotation
        /// </summary>
        public PageInterval SelectedPageInterval
        {
            get
            {
                return this._selectedPageInterval;
            }
            set
            {
                this._selectedPageInterval = value;
                RaisePropertyChanged("SelectedPageInterval");
            }
        }
        PageInterval _selectedPageInterval;


        #endregion
    }

}
