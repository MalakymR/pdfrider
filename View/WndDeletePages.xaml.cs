using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PDFRider
{
    public partial class WndDeletePages : BaseWindow
    {
        public WndDeletePages()
        {
            InitializeComponent();

            this.DataContext = new WndDeletePagesViewModel();
        }
        
    }
}
