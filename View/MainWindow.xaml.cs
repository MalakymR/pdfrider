using System;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();

            Messenger.Default.Register<TMsgClose>(this, MsgClose_Handler);
            Messenger.Default.Register<TMsgClosing>(this, MsgClosing_Handler);
            Messenger.Default.Register<TMsgOpenFile>(this, MsgOpenFile_Handler);
            Messenger.Default.Register<TMsgShowExtractPages>(this, MsgShowExtractPages_Handler);
            Messenger.Default.Register<TMsgShowDeletePages>(this, MsgShowDeletePages_Handler);
            Messenger.Default.Register<TMsgShowInsertPages>(this, MsgShowInsertPages_Handler);
            Messenger.Default.Register<TMsgShowAbout>(this, MsgShowAbout_Handler);

        }

        #region Messages handlers

        void MsgClose_Handler(TMsgClose msg)
        {
            if (msg.SenderViewModel == this.DataContext) this.Close();
        }

        void MsgClosing_Handler(TMsgClosing msg)
        {
            //First check if the sender is the data context of this window !
            if (msg.SenderViewModel == this.DataContext)
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
                            MainWindowViewModel mwvm = msg.SenderViewModel as MainWindowViewModel;

                            System.IO.File.Copy(mwvm.Uri, sfd.FileName);
                        }
                        else
                        {
                            msg.EventArgs.Cancel = true;
                        }
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
            string tempFile = wndExtractPages.ShowDialog();
            
            Messenger.Default.Send<TMsgLoadFile>(new TMsgLoadFile(tempFile, false, true));
        }

        void MsgShowDeletePages_Handler(TMsgShowDeletePages msg)
        {
            WndDeletePages wndDeletePages = new WndDeletePages();
            Messenger.Default.Send<TMsgShowDeletePages, WndDeletePagesViewModel>(msg);

            wndDeletePages.Owner = this;
            string tempFile = wndDeletePages.ShowDialog();

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
                string tempFile = wndInsertPages.ShowDialog();

                Messenger.Default.Send<TMsgLoadFile>(new TMsgLoadFile(tempFile));
            }
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
