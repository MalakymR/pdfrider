using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public abstract class WindowViewModel : ViewModelBase
    {
        #region Properties

        #region Information

        string _information = "";
        public string Information
        {
            get
            {
                return this._information;
            }
            set
            {
                this._information = value;
                Messenger.Default.Send<TMsgInformation>(new TMsgInformation(value));
                RaisePropertyChanged("Information");
            }
        }

        #endregion

        #endregion


        #region Commands

        #region CmdClose

        RelayCommand _cmdClose;
        public ICommand CmdClose
        {
            get
            {
                if (this._cmdClose == null)
                {
                    this._cmdClose = new RelayCommand(() => this.DoCmdClose());
                }
                return this._cmdClose;
            }
        }
        private void DoCmdClose()
        {
            Messenger.Default.Send<TMsgClose>(new TMsgClose(this));
        }

        #endregion

        #endregion

    }
}
