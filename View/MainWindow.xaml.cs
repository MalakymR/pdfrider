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

            Messenger.Default.Register<TMsgClosing>(this, MsgClosing_Handler);
            Messenger.Default.Register<TMsgOpenFile>(this, MsgOpenFile_Handler);
            Messenger.Default.Register<TMsgShowExtractPages>(this, MsgShowExtractPages_Handler);
            Messenger.Default.Register<TMsgShowDeletePages>(this, MsgShowDeletePages_Handler);
            Messenger.Default.Register<TMsgShowInsertPages>(this, MsgShowInsertPages_Handler);
            Messenger.Default.Register<TMsgShowMergeDocuments>(this, MsgShowMergeDocuments_Handler);
            Messenger.Default.Register<TMsgShowAbout>(this, MsgShowAbout_Handler);

            // Instantiate the data context (view model) AFTER registration of the messages.
            this.DataContext = new MainWindowViewModel();

            
        }

        #region Messages handlers

        void MsgClosing_Handler(TMsgClosing msg)
        {
            if (msg.AskForSave)
            {
                string text = App.Current.FindResource("loc_msgSaveChanges").ToString();
                MessageBoxResult result = MessageBox.Show(text, App.TITLE, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    msg.EventArgs.Cancel = true;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                    sfd.Title = this.FindResource("loc_saveFileDialogTitle").ToString();
                    sfd.DefaultExt = "pdf";
                    sfd.AddExtension = true;
                    sfd.Filter = App.Current.FindResource("loc_saveFileDialogFilter").ToString();

                    if ((bool)sfd.ShowDialog())
                    {
                        MainWindowViewModel mwvm = this.DataContext as MainWindowViewModel;

                        System.IO.File.Copy(mwvm.Uri, sfd.FileName);
                    }
                    else
                    {
                        msg.EventArgs.Cancel = true;
                    }
                }

            }
        }

        void MsgOpenFile_Handler(TMsgOpenFile msg)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = "pdf";
            ofd.Title = App.Current.FindResource("loc_openFileDialogTitle").ToString();
            ofd.Filter = App.Current.FindResource("loc_openFileDialogFilter").ToString();

            if ((bool)ofd.ShowDialog())
            {
                Messenger.Default.Send<TMsgLoadFile>(new TMsgLoadFile(ofd.FileName, true));
            }
        }

        void MsgShowExtractPages_Handler(TMsgShowExtractPages msg)
        {
            WndExtractPages wndExtractPages = new WndExtractPages();
            Messenger.Default.Send<TMsgShowExtractPages, WndExtractPagesViewModel>(msg);

            wndExtractPages.Owner = this;
            wndExtractPages.ShowDialog();
            string tempFile = wndExtractPages.ReturnValue as string;

            Messenger.Default.Send<TMsgLoadFile>(new TMsgLoadFile(tempFile, false, true));
        }

        void MsgShowDeletePages_Handler(TMsgShowDeletePages msg)
        {
            WndDeletePages wndDeletePages = new WndDeletePages();
            Messenger.Default.Send<TMsgShowDeletePages, WndDeletePagesViewModel>(msg);

            wndDeletePages.Owner = this;
            wndDeletePages.ShowDialog();
            string tempFile = wndDeletePages.ReturnValue as string;

            Messenger.Default.Send<TMsgLoadFile>(new TMsgLoadFile(tempFile));
        }

        void MsgShowInsertPages_Handler(TMsgShowInsertPages msg)
        {
            //Select the file to merge with the current document.
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = "pdf";
            ofd.Title = App.Current.FindResource("loc_openFileDialogTitle").ToString();
            ofd.Filter = App.Current.FindResource("loc_openFileDialogFilter").ToString();

            if ((bool)ofd.ShowDialog())
            {
                WndInsertPages wndInsertPages = new WndInsertPages();
                msg.FileToMerge = ofd.FileName;
                Messenger.Default.Send<TMsgShowInsertPages, WndInsertPagesViewModel>(msg);

                wndInsertPages.Owner = this;
                wndInsertPages.ShowDialog();
                string tempFile = wndInsertPages.ReturnValue as string;
                
                Messenger.Default.Send<TMsgLoadFile>(new TMsgLoadFile(tempFile));
            }
        }

        void MsgShowMergeDocuments_Handler(TMsgShowMergeDocuments msg)
        {
            
            WndMergeDocuments wndMergeDocuments = new WndMergeDocuments();
            Messenger.Default.Send<TMsgShowMergeDocuments, WndMergeDocumentsViewModel>(msg);
            
            if(this.IsActive) 
                wndMergeDocuments.Owner = this;
            
            wndMergeDocuments.ShowDialog();
            string tempFile = wndMergeDocuments.ReturnValue as string;
            
            Messenger.Default.Send<TMsgLoadFile>(new TMsgLoadFile(tempFile));
        }

        void MsgShowAbout_Handler(TMsgShowAbout msg)
        {
            WndAbout wndAbout = new WndAbout();

            wndAbout.Owner = this;
            wndAbout.ShowDialog();
        }

        #endregion

        
    }
}
