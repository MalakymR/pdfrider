using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// A base class for the view models of windows
    /// </summary>
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
            this.OnCmdClose();
        }

        #endregion

        protected virtual void OnCmdClose()
        {
            Messenger.Default.Send<TMsgClose>(new TMsgClose(this));
        }

        #endregion

    }
}
