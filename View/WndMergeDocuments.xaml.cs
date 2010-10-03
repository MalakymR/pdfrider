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

            this.DataContext = new WndMergeDocumentsViewModel();
        }

    }
}
