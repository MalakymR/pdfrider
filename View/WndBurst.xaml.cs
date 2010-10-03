using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public partial class WndBurst : BaseWindow
    {
        public WndBurst()
        {
            InitializeComponent();

            Messenger.Default.Register<DialogMessage>(
                this,
                msg =>
                {
                    var result = MessageBox.Show(
                        msg.Content,
                        msg.Caption,
                        msg.Button);

                    // Send callback
                    msg.ProcessCallback(result);
                });

            this.DataContext = new WndBurstViewModel();
        }
    }
}
