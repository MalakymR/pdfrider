using System;
using System.Windows;
using System.Windows.Controls;

namespace PDFRider
{
    public partial class WndAbout : Window
    {
        public WndAbout()
        {
            InitializeComponent();

            this.AppName = App.TITLE;
            this.AppVersion = App.VERSION;

            // A ViewModel is very useless for this window!
            this.DataContext = this;
        }

        public string AppName { get; private set; }
        public string AppVersion { get; private set; }

        // To open link in wpf window applications you must handle this event like this
        private void linkCodePlex_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }
    }
}
