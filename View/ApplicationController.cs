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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// Controls the application workflow.
    /// Opens windows with their view models.
    /// </summary>
    public class ApplicationController
    {
        /// <summary>
        /// A controller specific for dialog message and common dialog windows
        /// </summary>
        private DialogController _dialogController;


        public ApplicationController()
        {
            this._dialogController = new DialogController();


            Messenger.Default.Register<GenericMessageAction<MsgShowExtractPages, MsgOpenFile>>(this, MsgShowExtractPages_Handler);
            Messenger.Default.Register<GenericMessageAction<MsgShowDeletePages, MsgOpenFile>>(this, MsgShowDeletePages_Handler);
            Messenger.Default.Register<GenericMessageAction<MsgShowInsertPages, MsgOpenFile>>(this, MsgShowInsertPages_Handler);
            Messenger.Default.Register<GenericMessageAction<MsgShowRotatePages, MsgOpenFile>>(this, MsgShowRotatePages_Handler);
            Messenger.Default.Register<GenericMessageAction<MsgShowMergeDocuments, MsgOpenFile>>(this, MsgShowMergeDocuments_Handler);
            Messenger.Default.Register<MsgShowBurst>(this, MsgShowBurst_Handler);
            Messenger.Default.Register<GenericMessageAction<MsgShowSecurity, MsgOpenFile>>(this, MsgShowSecurity_Handler);

            Messenger.Default.Register<GenericMessageAction<MsgShowConfirmPassword, bool>>(this, MsgShowConfirmPassword_Handler);
            Messenger.Default.Register<GenericMessageAction<MsgShowEnterPassword, string>>(this, MsgShowEnterPassword_Handler);

            Messenger.Default.Register<MsgShowAbout>(this, MsgShowAbout_Handler);
        }


        /// <summary>
        /// Returns the current active window
        /// </summary>
        /// <returns></returns>
        Window GetActiveWindow()
        {
            Window window = null;
            foreach (Window w in App.Current.Windows)
            {
                if (w.IsActive)
                {
                    window = w;
                    break;
                }
            }
            return window;
        }


        /// <summary>
        /// Opens a new window
        /// </summary>
        /// <param name="window">The window to open</param>
        /// <param name="modal">Indicates whether to open the window as modal or not</param>
        /// <returns></returns>
        bool? ShowWindow(Window window, bool modal)
        {
            bool? ret = null;

            if (modal)
            {
                window.Owner = GetActiveWindow();
                ret = window.ShowDialog();
            }
            else
            {
                window.Show();
            }
            return ret;
        }


        internal void OpenMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

            mainWindow.DataContext = mainWindowViewModel;
            ShowWindow(mainWindow, false);
        }


        // --- EXTRACT PAGES
        void MsgShowExtractPages_Handler(GenericMessageAction<MsgShowExtractPages, MsgOpenFile> msg)
        {
            WndExtractPages wndExtractPages = new WndExtractPages();
            WndExtractPagesViewModel wndExtractPagesViewModel = new WndExtractPagesViewModel(msg.Data.Document);

            wndExtractPages.DataContext = wndExtractPagesViewModel;
            ShowWindow(wndExtractPages, true);
            
            string tempFile = wndExtractPagesViewModel.ReturnValue as string;

            msg.Execute(new MsgOpenFile(tempFile, false, true));
        }


        // --- DELETE PAGES
        void MsgShowDeletePages_Handler(GenericMessageAction<MsgShowDeletePages, MsgOpenFile> msg)
        {
            WndDeletePages wndDeletePages = new WndDeletePages();
            WndDeletePagesViewModel wndDeletePagesViewModel = new WndDeletePagesViewModel(msg.Data.Document);

            wndDeletePages.DataContext = wndDeletePagesViewModel;
            ShowWindow(wndDeletePages, true);

            string tempFile = wndDeletePagesViewModel.ReturnValue as string;

            msg.Execute(new MsgOpenFile(tempFile));
        }


        // --- INSERT PAGES
        void MsgShowInsertPages_Handler(GenericMessageAction<MsgShowInsertPages, MsgOpenFile> msg)
        {
            WndInsertPages wndInsertPages = new WndInsertPages();
            WndInsertPagesViewModel wndInsertPagesViewModel = new WndInsertPagesViewModel(msg.Data.Document, msg.Data.FileToMerge);

            wndInsertPages.DataContext = wndInsertPagesViewModel;
            ShowWindow(wndInsertPages, true);

            string tempFile = wndInsertPagesViewModel.ReturnValue as string;

            msg.Execute(new MsgOpenFile(tempFile));
        }


        // --- ROTATE PAGES
        void MsgShowRotatePages_Handler(GenericMessageAction<MsgShowRotatePages, MsgOpenFile> msg)
        {
            WndRotatePages wndRotatePages = new WndRotatePages();
            WndRotatePagesViewModel wndRotatePagesViewModel = new WndRotatePagesViewModel(msg.Data.Document);

            wndRotatePages.DataContext = wndRotatePagesViewModel;
            ShowWindow(wndRotatePages, true);

            string tempFile = wndRotatePagesViewModel.ReturnValue as string;

            msg.Execute(new MsgOpenFile(tempFile));
        }


        // --- MERGE DOCUMENTS
        void MsgShowMergeDocuments_Handler(GenericMessageAction<MsgShowMergeDocuments, MsgOpenFile> msg)
        {
            WndMergeDocuments wndMergeDocuments = new WndMergeDocuments();
            WndMergeDocumentsViewModel wndMergeDocumentsViewModel = new WndMergeDocumentsViewModel(msg.Data.FilesToMerge);

            wndMergeDocuments.DataContext = wndMergeDocumentsViewModel;
            ShowWindow(wndMergeDocuments, true);

            string tempFile = wndMergeDocumentsViewModel.ReturnValue as string;

            msg.Execute(new MsgOpenFile(tempFile));
        }


        // --- BURST
        void MsgShowBurst_Handler(MsgShowBurst msg)
        {
            WndBurst wndBurst = new WndBurst();
            WndBurstViewModel wndBurstViewModel = new WndBurstViewModel(msg.Document);

            wndBurst.DataContext = wndBurstViewModel;
            ShowWindow(wndBurst, true);
        }


        // --- SECURITY
        void MsgShowSecurity_Handler(GenericMessageAction<MsgShowSecurity, MsgOpenFile> msg)
        {
            WndSecurity wndSecurity = new WndSecurity();
            WndSecurityViewModel wndSecurityViewModel = new WndSecurityViewModel(msg.Data.Document);

            wndSecurity.DataContext = wndSecurityViewModel;
            ShowWindow(wndSecurity, true);

            string tempFile = wndSecurityViewModel.ReturnValue as string;

            msg.Execute(new MsgOpenFile(tempFile));
        }

        private void MsgShowConfirmPassword_Handler(GenericMessageAction<MsgShowConfirmPassword, bool> msg)
        {
            WndConfirmPassword wndConfirmPassword = new WndConfirmPassword();
            WndConfirmPasswordViewModel wndConfirmPasswordViewModel = new WndConfirmPasswordViewModel(msg.Data.PasswordType, msg.Data.OriginalPassword);

            wndConfirmPassword.DataContext = wndConfirmPasswordViewModel;
            ShowWindow(wndConfirmPassword, true);

            bool ret = wndConfirmPasswordViewModel.ReturnValue == null ? false : (bool)wndConfirmPasswordViewModel.ReturnValue;

            msg.Execute(ret);
        }

        private void MsgShowEnterPassword_Handler(GenericMessageAction<MsgShowEnterPassword, string> msg)
        {
            WndEnterPassword wndEnterPassword = new WndEnterPassword();
            WndEnterPasswordViewModel wndEnterPasswordViewModel = new WndEnterPasswordViewModel(msg.Data.Document);

            wndEnterPassword.DataContext = wndEnterPasswordViewModel;
            ShowWindow(wndEnterPassword, true);

            string ret = wndEnterPasswordViewModel.ReturnValue as string;

            msg.Execute(ret);
        }



        // --- ABOUT
        void MsgShowAbout_Handler(MsgShowAbout msg)
        {
            WndAbout wndAbout = new WndAbout();

            ShowWindow(wndAbout, true);
        }
    }


}
