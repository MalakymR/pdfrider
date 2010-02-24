using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public partial class WndMergeDocuments : BaseWindow
    {
        public WndMergeDocuments()
        {
            InitializeComponent();

            Messenger.Default.Register<TMsgOpenFile>(this, MsgOpenFile_Handler);

            this.DataContext = new WndMergeDocumentsViewModel();
        }

        void MsgOpenFile_Handler(TMsgOpenFile msg)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = "pdf";
            ofd.Multiselect = msg.Multiselect;
            ofd.Title = App.Current.FindResource("loc_openFileDialogTitle").ToString();
            ofd.Filter = App.Current.FindResource("loc_openFileDialogFilter").ToString();
            
            if ((bool)ofd.ShowDialog())
            {
                Messenger.Default.Send<TMsgAddFiles>(new TMsgAddFiles(ofd.FileNames.ToList<string>()));
            }
        }
    }
}
