using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public partial class MainWindow : BaseWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<GenericMessageAction<TMsgClosing, TMsgClosing>>(this, MsgClosing_Handler);
            Messenger.Default.Register<GenericMessageAction<TMsgOpenFile, TMsgOpenFile>>(this, MsgOpenFile_Handler);
            Messenger.Default.Register<GenericMessageAction<TMsgSaveFile, TMsgOpenFile>>(this, MsgSaveFile_Handler);

            Messenger.Default.Register<GenericMessageAction<TMsgSelectFolder, string>>(this, MsgOpenFolder_Handler);
            
            Messenger.Default.Register<GenericMessageAction<TMsgShowExtractPages, TMsgOpenFile>>(this, MsgShowExtractPages_Handler);
            Messenger.Default.Register<GenericMessageAction<TMsgShowDeletePages, TMsgOpenFile>>(this, MsgShowDeletePages_Handler);
            Messenger.Default.Register<GenericMessageAction<TMsgShowInsertPages, TMsgOpenFile>>(this, MsgShowInsertPages_Handler);
            Messenger.Default.Register<GenericMessageAction<TMsgShowRotatePages, TMsgOpenFile>>(this, MsgShowRotatePages_Handler);
            Messenger.Default.Register<GenericMessageAction<TMsgShowMergeDocuments, TMsgOpenFile>>(this, MsgShowMergeDocuments_Handler);            
            Messenger.Default.Register<TMsgShowBurst>(this, MsgShowBurst_Handler);
            Messenger.Default.Register<GenericMessageAction<TMsgShowSecurity, TMsgOpenFile>>(this, MsgShowSecurity_Handler);

            Messenger.Default.Register<TMsgShowAbout>(this, MsgShowAbout_Handler);

            // Instantiate the data context (view model) AFTER registration of the messages.
            this.DataContext = new MainWindowViewModel();

            
        }

        #region Messages handlers

        // --- WINDOW CLOSING
        void MsgClosing_Handler(GenericMessageAction<TMsgClosing, TMsgClosing> msg)
        {
            TMsgClosing data = msg.Data;

            if (data.AskForSave)
            {
                string text = App.Current.FindResource("loc_msgSaveChanges").ToString();
                MessageBoxResult result = MessageBox.Show(text, App.TITLE, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    //msg.Data.EventArgs.Cancel = true;
                    //msg.Data.Cancel = true;
                    data.Cancel = true;
                }
                else if (result == MessageBoxResult.Yes)
                {

                    //Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                    //sfd.Title = this.FindResource("loc_saveFileDialogTitle").ToString();
                    //sfd.DefaultExt = "pdf";
                    //sfd.AddExtension = true;
                    //sfd.Filter = App.Current.FindResource("loc_saveFileDialogFilter").ToString();

                    //if ((bool)sfd.ShowDialog())

                    FileDialogResult r = DialogController.ShowSaveFileDialog();
                    data.Data = r.FileName;
                    //msg.Data.EventArgs.Cancel = !r.CommonDialogReturn;
                    data.Cancel = !r.CommonDialogReturn;
                    //msg.Execute(msg.Data);

                    //if (r.CommonDialogReturn)
                    //{
                    //    MainWindowViewModel mwvm = this.DataContext as MainWindowViewModel;

                    //    System.IO.File.Copy(mwvm.Uri, r.FileName);
                    //}
                    //else
                    //{
                    //    msg.Data.EventArgs.Cancel = true;
                    //}
                }

                //msg.Data = data;
                msg.Execute(data);

            }
        }


        // --- OPEN FILE
        void MsgOpenFile_Handler(GenericMessageAction<TMsgOpenFile, TMsgOpenFile> msg)
        {
            TMsgOpenFile data = msg.Data;

            //Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            //ofd.DefaultExt = "pdf";
            //ofd.Multiselect = data.Multiselect;
            //ofd.Title = App.Current.FindResource("loc_openFileDialogTitle").ToString();
            //ofd.Filter = App.Current.FindResource("loc_openFileDialogFilter").ToString();

            FileDialogResult r = DialogController.ShowOpenFileDialog(data.Multiselect);
            data.FileName = r.FileName;
            data.FileNames = r.FileNames;
            data.NewFile = r.CommonDialogReturn;
            msg.Execute(data); //new TMsgOpenFile(r.FileName, r.CommonDialogReturn));

            //if ((bool)ofd.ShowDialog())
            //{
            //    if (ofd.Multiselect)
            //    {
            //        data.FileNames.AddRange(ofd.FileNames);
            //    }
            //    else
            //    {
            //        data.FileName = ofd.FileName;
            //    }
            //    //msg.Data = data;
            //    msg.Execute(data);
            //}
        }


        // --- SAVE FILE
        void MsgSaveFile_Handler(GenericMessageAction<TMsgSaveFile, TMsgOpenFile> msg)
        {
            
            //Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            //sfd.Title = this.FindResource("loc_saveFileDialogTitle").ToString();
            //sfd.DefaultExt = "pdf";
            //sfd.AddExtension = true;
            //sfd.Filter = App.Current.FindResource("loc_saveFileDialogFilter").ToString();

            //if ((bool)sfd.ShowDialog())
            FileDialogResult r = DialogController.ShowSaveFileDialog();
            msg.Execute(new TMsgOpenFile(r.FileName, r.CommonDialogReturn));

            //if (r.CommonDialogReturn)
            //{
            //    MainWindowViewModel mwvm = this.DataContext as MainWindowViewModel;

            //    System.IO.File.Copy(mwvm.Uri, r.FileName);

            //    msg.Execute(new TMsgOpenFile(r.FileName, true));
            //}
            //else
            //{
            //    msg.Execute(new TMsgOpenFile());
            //}
                
        }



        // --- OPEN FOLDER
        void MsgOpenFolder_Handler(GenericMessageAction<TMsgSelectFolder, string> msg) //TMsgSelectFolder msg)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            fbd.Description = msg.Data.Description;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                msg.Execute(fbd.SelectedPath);
            }
        }


        // --- EXTRACT PAGES
        void MsgShowExtractPages_Handler(GenericMessageAction<TMsgShowExtractPages, TMsgOpenFile> msg)
        {
            WndExtractPages wndExtractPages = new WndExtractPages();
            Messenger.Default.Send<TMsgShowExtractPages, WndExtractPagesViewModel>(msg.Data);

            wndExtractPages.Owner = this;
            wndExtractPages.ShowDialog();
            string tempFile = wndExtractPages.ReturnValue as string;

            msg.Execute(new TMsgOpenFile(tempFile, false, true));
        }


        // --- DELETE PAGES
        void MsgShowDeletePages_Handler(GenericMessageAction<TMsgShowDeletePages, TMsgOpenFile> msg)
        {
            WndDeletePages wndDeletePages = new WndDeletePages();
            Messenger.Default.Send<TMsgShowDeletePages, WndDeletePagesViewModel>(msg.Data);

            wndDeletePages.Owner = this;
            wndDeletePages.ShowDialog();
            string tempFile = wndDeletePages.ReturnValue as string;

            msg.Execute(new TMsgOpenFile(tempFile));
        }


        // --- INSERT PAGES
        void MsgShowInsertPages_Handler(GenericMessageAction<TMsgShowInsertPages, TMsgOpenFile> msg)
        {
            //Select the file to merge with the current document.
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = "pdf";
            ofd.Title = App.Current.FindResource("loc_openFileDialogTitle").ToString();
            ofd.Filter = App.Current.FindResource("loc_openFileDialogFilter").ToString();

            if ((bool)ofd.ShowDialog())
            {
                WndInsertPages wndInsertPages = new WndInsertPages();
                msg.Data.FileToMerge = ofd.FileName;
                Messenger.Default.Send<TMsgShowInsertPages, WndInsertPagesViewModel>(msg.Data);

                wndInsertPages.Owner = this;
                wndInsertPages.ShowDialog();
                string tempFile = wndInsertPages.ReturnValue as string;

                msg.Execute(new TMsgOpenFile(tempFile));
            }
        }


        // --- ROTATE PAGES
        void MsgShowRotatePages_Handler(GenericMessageAction<TMsgShowRotatePages, TMsgOpenFile> msg)
        {
            WndRotatePages wndRotatePages = new WndRotatePages();
            Messenger.Default.Send<TMsgShowRotatePages, WndRotatePagesViewModel>(msg.Data);

            wndRotatePages.Owner = this;
            wndRotatePages.ShowDialog();
            string tempFile = wndRotatePages.ReturnValue as string;

            msg.Execute(new TMsgOpenFile(tempFile));
        }


        // --- MERGE DOCUMENTS
        void MsgShowMergeDocuments_Handler(GenericMessageAction<TMsgShowMergeDocuments, TMsgOpenFile> msg)
        {
            WndMergeDocuments wndMergeDocuments = new WndMergeDocuments();
            Messenger.Default.Send<TMsgShowMergeDocuments, WndMergeDocumentsViewModel>(msg.Data);

            if (this.IsActive)
                wndMergeDocuments.Owner = this;

            wndMergeDocuments.ShowDialog();
            string tempFile = wndMergeDocuments.ReturnValue as string;

            msg.Execute(new TMsgOpenFile(tempFile));
        }


        // --- BURST
        void MsgShowBurst_Handler(TMsgShowBurst msg)
        {
            WndBurst wndBurst = new WndBurst();

            Messenger.Default.Send<TMsgShowBurst, WndBurstViewModel>(msg);

            wndBurst.Owner = this;
            wndBurst.ShowDialog();
        }


        // --- SECURITY
        void MsgShowSecurity_Handler(GenericMessageAction<TMsgShowSecurity, TMsgOpenFile> msg)
        {
            WndSecurity wndSecurity = new WndSecurity();
            Messenger.Default.Send<TMsgShowSecurity, WndSecurityViewModel>(msg.Data);

            wndSecurity.Owner = this;
            wndSecurity.ShowDialog();

            string tempFile = wndSecurity.ReturnValue as string;

            msg.Execute(new TMsgOpenFile(tempFile));
        }


        // --- ABOUT
        void MsgShowAbout_Handler(TMsgShowAbout msg)
        {
            WndAbout wndAbout = new WndAbout();

            wndAbout.Owner = this;
            wndAbout.ShowDialog();
        }

        #endregion

        
    }
}
