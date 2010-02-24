using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PDFRider
{
    public partial class WndInsertPages : BaseWindow
    {
        public WndInsertPages()
        {
            InitializeComponent();

            this.DataContext = new WndInsertPagesViewModel();
        }

    }
}
