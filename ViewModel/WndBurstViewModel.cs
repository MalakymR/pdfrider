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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class WndBurstViewModel : ToolWindowViewModel
    {
        PDFDocument _doc;

        public WndBurstViewModel(PDFDocument document)
        {
            //Get the document...
            this._doc = document;

            //... and do some initializations.
            this.WorkingDirectory = System.IO.Path.GetDirectoryName(this._doc.FullName);
            this.Prefix = "pg_";
        }


        #region Properties

        /// <summary>
        /// Directory where the file will be burst.
        /// If not specified, the directory of the input file will be used.
        /// </summary>
        string _workingDirectory;
        public string WorkingDirectory
        {
            get
            {
                return this._workingDirectory;
            }
            set
            {
                this._workingDirectory = value;
                RaisePropertyChanged("WorkingDirectory");
            }
        }

        /// <summary>
        /// Prefix used to name the burst files.
        /// </summary>
        string _prefix;
        public string Prefix
        {
            get
            {
                return this._prefix;
            }
            set
            {
                this._prefix = value;
                RaisePropertyChanged("Prefix");
            }
        }

        /// <summary>
        /// Show burst files on exit.
        /// </summary>
        bool _showFiles;
        public bool ShowFiles
        {
            get
            {
                return this._showFiles;
            }
            set
            {
                this._showFiles = value;
                RaisePropertyChanged("ShowFiles");
            }
        }

        #endregion


        #region Commands

        #region CmdBrowse

        RelayCommand _cmdBrowse;
        public ICommand CmdBrowse
        {
            get
            {
                if (this._cmdBrowse == null)
                {
                    this._cmdBrowse = new RelayCommand(() => this.DoCmdBrowse());
                }
                return this._cmdBrowse;
            }
        }

        private void DoCmdBrowse()
        {
            GenericMessageAction<MsgSelectFolder, string> message = new GenericMessageAction<MsgSelectFolder, string>(
                new MsgSelectFolder()
                {
                    Description = App.Current.FindResource("loc_burstFolderBrowserDialogDescription").ToString()
                }, 
                x => this.WorkingDirectory = x);

            Messenger.Default.Send(message);
        }

        #endregion


        #region CmdBurst

        RelayCommand _cmdBurst;
        public ICommand CmdBurst
        {
            get
            {
                if (this._cmdBurst == null)
                {
                    this._cmdBurst = new RelayCommand(() => this.DoCmdBurst(),
                        () => CanDoCmdBurst);
                }
                return this._cmdBurst;
            }
        }

        private void DoCmdBurst()
        {
            PDFActions pdfActions = new PDFActions();
            pdfActions.Burst(this._doc, this.WorkingDirectory, this.Prefix);

            DialogMessage message = new DialogMessage(this,
                App.Current.FindResource("loc_burstCompletedDialogInformation").ToString(),
                x =>
                {
                    if (this.ShowFiles)
                    {
                        System.Diagnostics.Process p = new System.Diagnostics.Process();

                        p.StartInfo.FileName = this.WorkingDirectory;

                        p.Start();

                        p.Close();
                    }

                    this.Close(this.WorkingDirectory);
                })
            {
                Button = System.Windows.MessageBoxButton.OK,
                Icon = System.Windows.MessageBoxImage.Information,
                Caption = App.NAME
            };

            Messenger.Default.Send(message);
            
        }

        private bool CanDoCmdBurst
        {
            get
            {
                return System.IO.Directory.Exists(this.WorkingDirectory);
            }
        }


        #endregion


        #endregion

    }
}
