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

        public WndBurstViewModel()
        {
            Messenger.Default.Register<TMsgShowBurst>(this, MsgShowBurst_Handler);
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
            GenericMessageAction<TMsgSelectFolder, string> message = new GenericMessageAction<TMsgSelectFolder, string>(
                new TMsgSelectFolder()
                {
                    Description = App.Current.FindResource("loc_burstFolderBrowserDialogDescription").ToString()
                }, 
                x => this.WorkingDirectory = x);

            Messenger.Default.Send<GenericMessageAction<TMsgSelectFolder, string>, MainWindow>(message);
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
            PDFDocument.OperationStates state = this._doc.Burst(this.WorkingDirectory, this.Prefix);

            DialogMessage message = new DialogMessage(
                App.Current.FindResource("loc_burstCompletedDialogInformation").ToString(), DoCmdBurstCallback)
            {
                Button = System.Windows.MessageBoxButton.OK,
                Icon = System.Windows.MessageBoxImage.Information,
                Caption = App.NAME
            };

            Messenger.Default.Send<DialogMessage, WndBurst>(message);
            //Messenger.Default.Send<TMsgClose>(new TMsgClose(this, this.WorkingDirectory));
            
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



        #region Message handlers

        void MsgShowBurst_Handler(TMsgShowBurst msg)
        {
            //Get the document...
            this._doc = msg.Document;

            //... and do some initializations.
            this.WorkingDirectory = System.IO.Path.GetDirectoryName(this._doc.FullName);
            this.Prefix = "pg_";
        }


        void DoCmdBurstCallback(System.Windows.MessageBoxResult result)
        {
            if (this.ShowFiles)
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();

                p.StartInfo.FileName = this.WorkingDirectory;
                
                p.Start();
                
                p.Close();
            }

            Messenger.Default.Send<TMsgClose>(new TMsgClose(this, this.WorkingDirectory));
        }

        #endregion
    }
}
