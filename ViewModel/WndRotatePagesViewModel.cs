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
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class WndRotatePagesViewModel : ToolWindowViewModel
    {
        
        public struct Rotation
        {
            public PDFActions.Rotations Id;
            public string Description { get; set; }
        }

        PDFDocument _doc;

        public WndRotatePagesViewModel(PDFDocument document)
        {
            //Get the document...
            this._doc = document;

            //... and do some initializations.
            this.NumberOfPages = (this._doc.NumberOfPages + this._doc.PageLabelStart - 1);
            this.NumberOfPhysicalPages = this._doc.NumberOfPages;

            this.PageStart = this._doc.PageLabelStart.ToString();
            this.PageEnd = this._doc.PageLabelStart.ToString();

            this.PageIntervals = LoadPageIntervals();
            this.SelectedPageInterval = this.PageIntervals[0];

            this.Rotations = LoadRotations();
            this.SelectedRotation = this.Rotations[0];
        }

        #region Private methods

        private static ObservableCollection<PageInterval> LoadPageIntervals()
        {
            ObservableCollection<PageInterval> pageIntervals = new ObservableCollection<PageInterval>();

            pageIntervals.Add(new PageInterval()
            {
                Id = PDFActions.PageIntervals.All,
                Description = App.Current.FindResource("loc_intervalAllPages").ToString()
            });
            pageIntervals.Add(new PageInterval()
            {
                Id = PDFActions.PageIntervals.Even,
                Description = App.Current.FindResource("loc_intervalEvenPages").ToString()
            });
            pageIntervals.Add(new PageInterval()
            {
                Id = PDFActions.PageIntervals.Odd,
                Description = App.Current.FindResource("loc_intervalOddPages").ToString()
            });

            return pageIntervals;
        }

        private static ObservableCollection<Rotation> LoadRotations()
        {
            ObservableCollection<Rotation> rotations = new ObservableCollection<Rotation>();

            rotations.Add(new Rotation()
            {
                Id = PDFActions.Rotations.Left,
                Description = App.Current.FindResource("loc_rotation90CCW").ToString()
            });
            rotations.Add(new Rotation()
            {
                Id = PDFActions.Rotations.Right,
                Description = App.Current.FindResource("loc_rotation90CW").ToString()
            });
            rotations.Add(new Rotation()
            {
                Id = PDFActions.Rotations.Down,
                Description = App.Current.FindResource("loc_rotation180").ToString()
            });

            return rotations;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Available rotations: Nord=0, South=180, East=90, West=90, Left=-90, Right=+90, Down=+180
        /// </summary>
        /// <remarks>Only Left, Right and Down rotations are included!</remarks>
        public ObservableCollection<Rotation> Rotations { get; private set; }

        /// <summary>
        /// The current (selected) rotation to use
        /// </summary>
        Rotation _selectedRotation;
        public Rotation SelectedRotation
        {
            get
            {
                return this._selectedRotation;
            }
            set
            {
                this._selectedRotation = value;
                RaisePropertyChanged("SelectedRotation");
            }
        }

        #endregion


        #region Commands

        #region CmdRotate

        RelayCommand _cmdRotate;
        public ICommand CmdRotate
        {
            get
            {
                if (this._cmdRotate == null)
                {
                    this._cmdRotate = new RelayCommand(() => this.DoCmdRotate(),
                        () => CanDoCmdRotate);
                }
                return this._cmdRotate;
            }
        }

        private void DoCmdRotate()
        {
            string tempFile = System.IO.Path.Combine(App.TEMP_DIR, this._doc.FileName);

            int start = Int32.Parse(this.PageStart) - this._doc.PageLabelStart + 1;
            int end = Int32.Parse(this.PageEnd) - this._doc.PageLabelStart + 1;

            PDFActions pdfActions = new PDFActions();

            PDFActions.PageIntervals interval = this.SelectedPageInterval.Id;
            PDFActions.Rotations rotation = this.SelectedRotation.Id;

            PDFActions.OperationStates state = pdfActions.RotatePages(this._doc, start, end, interval, rotation, ref tempFile);

            if (state == PDFActions.OperationStates.PageRangeOutOfDocument)
            {
                this.Information = App.Current.FindResource("loc_msgOutOfDocument").ToString();
            }
            else
            {
                this.Close(tempFile);
            }
        }

        private bool CanDoCmdRotate
        {
            get
            {
                return ((this.IsValidPageStart) && (this.IsValidPageEnd));
            }
        }

        #endregion

        #endregion


    }
}
