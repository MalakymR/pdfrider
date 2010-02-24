using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PDFRider
{
    public partial class WndExtractPages : BaseWindow
    {
        public WndExtractPages()
        {
            InitializeComponent();

            this.DataContext = new WndExtractPagesViewModel();
        }
    }
}
