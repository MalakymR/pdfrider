using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    public class WndMergeDocumentsViewModel : WindowViewModel
    {
        public static int MAX_FILES = PDFDocument.MAX_HANDLES;

        FileToMergeViewModel _selectedFile;

        public WndMergeDocumentsViewModel()
        {
            Messenger.Default.Register<TMsgShowMergeDocuments>(this, MsgShowMergeDocuments_Handler);
            Messenger.Default.Register<TMsgAddFiles>(this, MsgAddFiles_Handler);

            this.Files = new ObservableCollection<FileToMergeViewModel>();
        }

        #region Properties

        public ObservableCollection<FileToMergeViewModel> Files { get; private set; }
        
        public FileToMergeViewModel SelectedFile 
        {
            get
            {
                return this._selectedFile;
            }
            private set
            {
                this._selectedFile = value;
                RaisePropertyChanged("SelectedFile");
            }
        }


        #endregion


        #region Commands

        #region CmdMerge

        RelayCommand _cmdMerge;
        public ICommand CmdMerge
        {
            get
            {
                if (this._cmdMerge == null)
                {
                    this._cmdMerge = new RelayCommand(() => this.DoCmdMerge());
                }
                return this._cmdMerge;
            }
        }

        private void DoCmdMerge()
        {
            bool ok = true;
            foreach (FileToMergeViewModel f in this.Files)
            {
                if (!f.IsValidInterval)
                {
                    ok = false;
                    break;
                }
            }
            if (!ok)
            {
                this.Information = App.Current.FindResource("loc_correctErrors").ToString();
            }
            else
            {
                string tempFileName = System.IO.Path.GetFileNameWithoutExtension(this.Files[0].FileName) +
                    App.Current.FindResource("loc_tempMerged").ToString() +
                    System.IO.Path.GetExtension(this.Files[0].FileName);
                string tempFile = System.IO.Path.Combine(App.TEMP_DIR, tempFileName);

                PDFFile[] filesToMerge = (from f in this.Files
                                          select f.PdfFile).ToArray<PDFFile>();

                PDFDocument.OperationStates state = PDFDocument.MergeDocuments(filesToMerge, tempFile);

                if (state == PDFDocument.OperationStates.TooManyFiles)
                {
                    this.Information = String.Format(
                        App.Current.FindResource("loc_tooManyFiles").ToString(), MAX_FILES); ;
                }
                else
                {
                    Messenger.Default.Send<TMsgClose>(new TMsgClose(this, tempFile));
                }
            }
        }


        #endregion

        #region CmdSelectFile

        // This is the easiest way I found to work with the selected item of a ListView.
        // See also SelectedCommandBehavior.

        RelayCommand<FileToMergeViewModel> _cmdSelectFile;
        public ICommand CmdSelectFile
        {
            get
            {
                if (this._cmdSelectFile == null)
                {
                    this._cmdSelectFile = new RelayCommand<FileToMergeViewModel>((x) => this.DoCmdSelectFile(x));
                }
                return this._cmdSelectFile;
            }
        }

        private void DoCmdSelectFile(FileToMergeViewModel file)
        {
            this.SelectedFile = file;
        }


        #endregion

        #region CmdMoveUp

        RelayCommand _cmdMoveUp;
        public ICommand CmdMoveUp
        {
            get
            {
                if (this._cmdMoveUp == null)
                {
                    this._cmdMoveUp = new RelayCommand(() => this.DoCmdMoveUp(),
                        () => this.CanDoCmdMoveUp);
                }
                return this._cmdMoveUp;
            }
        }

        private void DoCmdMoveUp()
        {
            int i = this.Files.IndexOf(this.SelectedFile);
            this.Files.Move(i, --i);
        }

        private bool CanDoCmdMoveUp
        {
            get
            {
                return ((this.Files.Count > 1) &&
                    (this.SelectedFile != null) &&
                    (this.Files.IndexOf(this.SelectedFile) > 0));
            }
        }

        #endregion

        #region CmdMoveDown

        RelayCommand _cmdMoveDown;
        public ICommand CmdMoveDown
        {
            get
            {
                if (this._cmdMoveDown == null)
                {
                    this._cmdMoveDown = new RelayCommand(() => this.DoCmdMoveDown(),
                        () => this.CanDoCmdMoveDown);
                }
                return this._cmdMoveDown;
            }
        }

        private void DoCmdMoveDown()
        {
            int i = this.Files.IndexOf(this.SelectedFile);
            this.Files.Move(i, ++i);
        }

        private bool CanDoCmdMoveDown
        {
            get
            {
                return ((this.Files.Count > 1) &&
                    (this.SelectedFile != null) &&
                    (this.Files.IndexOf(this.SelectedFile) < this.Files.Count - 1));
            }
        }

        #endregion

        #region CmdAddFile

        RelayCommand _cmdAddFile;
        public ICommand CmdAddFile
        {
            get
            {
                if (this._cmdAddFile == null)
                {
                    this._cmdAddFile = new RelayCommand(() => this.DoCmdAddFile());
                }
                return this._cmdAddFile;
            }
        }

        private void DoCmdAddFile()
        {
            Messenger.Default.Send<TMsgOpenFile, WndMergeDocuments>(new TMsgOpenFile()
            {
                Multiselect = true
            });
        }

        #endregion

        #region CmdRemoveFile

        RelayCommand _cmdRemoveFile;
        public ICommand CmdRemoveFile
        {
            get
            {
                if (this._cmdRemoveFile == null)
                {
                    this._cmdRemoveFile = new RelayCommand(() => this.DoCmdRemoveFile(),
                        () => this.CanDoCmdRemoveFile);
                }
                return this._cmdRemoveFile;
            }
        }

        private void DoCmdRemoveFile()
        {
            this.Files.Remove(this.SelectedFile);
        }

        private bool CanDoCmdRemoveFile
        {
            get
            {
                return ((this.Files.Count > 1) &&
                    (this.SelectedFile != null));
            }
        }

        #endregion

        #endregion


        #region Message handlers

        void MsgShowMergeDocuments_Handler(TMsgShowMergeDocuments msg)
        {
            this.Files.Clear();
            this.AddFiles(msg.FilesToMerge);
        }


        void MsgAddFiles_Handler(TMsgAddFiles msg)
        {
            this.AddFiles(msg.Files);
        }

        #endregion


        #region Private (support) methods

        void AddFiles(IEnumerable<string> files)
        {
            foreach (string f in files)
            {
                this.Files.Add(new FileToMergeViewModel(f));
            }
        }

        #endregion
    }
}
