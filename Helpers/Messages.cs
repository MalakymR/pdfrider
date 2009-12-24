using System;
using GalaSoft.MvvmLight;

namespace PDFRider
{
    public enum InformationType
    {
        All,
        OutOfDocument
    }

    /// <summary>
    /// Type of the message for requesting to display some informations.
    /// </summary>
    public class TMsgInformation
    {
        public TMsgInformation(InformationType type, string text)
        {
            this.Type = type;
            this.Text = text;
        }
        public TMsgInformation(string text)
            : this(InformationType.All, text)
        {
        }

        public InformationType Type { get; private set; }
        public string Text { get; private set; }
    }

    /// <summary>
    /// Type of the message for requesting to close a window.
    /// </summary>
    /// <remarks>
    /// I pass a generic ViewModel in the constructor, so when a window catches this message,
    /// it only needs to check if it's equal to its DataContext.
    /// </remarks>
    public class TMsgClose
    {
        public TMsgClose(ViewModelBase view_model, object dialog_return_value)
        {
            this.SenderViewModel = view_model;
            this.DialogReturnValue = dialog_return_value;
        }

        public TMsgClose(ViewModelBase view_model, bool? dialog_result)
        {
            this.SenderViewModel = view_model;
            this.DialogResult = dialog_result;
        }

        public TMsgClose(ViewModelBase view_model)
            : this(view_model, null)
        {
        }

        public ViewModelBase SenderViewModel { get; private set; }
        public bool? DialogResult { get; set; }

        /// <summary>
        /// A custom value that a dialog window can return in place of the standard bool? DialogResult
        /// </summary>
        public object DialogReturnValue { get; set; }
    }

    /// <summary>
    /// Type of the message for sending information about window closing status.
    /// </summary>
    /// <remarks>
    /// I pass a generic ViewModel in the constructor, so when a window catches this message,
    /// it only needs to check if it's equal to its DataContext.
    /// </remarks>
    public class TMsgClosing
    {
        public TMsgClosing(ViewModelBase view_model)
        {
            this.SenderViewModel = view_model;
            this.AskForSave = false;
            this.EventArgs = new System.ComponentModel.CancelEventArgs();
            
        }

        public ViewModelBase SenderViewModel { get; private set; }
        public bool AskForSave { get; set; }
        public System.ComponentModel.CancelEventArgs EventArgs { get; set; }
        
    }

    /// <summary>
    /// Type of the message for requesting to open a file.
    /// </summary>
    public class TMsgOpenFile
    {
        public TMsgOpenFile() { }
        
        public string Title { get; set; }
        public string Filter { get; set; }
        public string FileType { get; set; }
    }

    /// <summary>
    /// Type of the message to inform that a file needs to be loaded.
    /// </summary>
    public class TMsgLoadFile
    {
        /// <summary>
        /// Load file using file name
        /// </summary>
        /// <param name="file_name">Full name of the file to load</param>
        /// <param name="new_file">*True* if the file does not come from an operation</param>
        /// <param name="new_window">*True* to open the file in a new window</param>
        public TMsgLoadFile(string file_name, bool new_file, bool new_window)
        {
            this.FileName = file_name;
            this.NewFile = new_file;
            this.NewWindow = new_window;
        }

        /// <summary>
        /// Load file using file name
        /// </summary>
        /// <param name="file_name">Full name of the file to load</param>
        /// <param name="new_file">*True* if the file does not come from an operation</param>
        public TMsgLoadFile(string file_name, bool new_file)
            : this(file_name, new_file, false)
        {
            this.FileName = file_name;
            this.NewFile = new_file;
        }

        /// <summary>
        /// Load file using file name
        /// </summary>
        /// <param name="file_name">Full name of the file to load</param>
        public TMsgLoadFile(string file_name)
            : this(file_name, false)
        {
        }

        public string FileName { get; private set; }
        public bool NewFile { get; private set; }
        public bool NewWindow { get; private set; }
    }

    /// <summary>
    /// Type of the message for requesting to extract the pages from the document.
    /// </summary>
    public class TMsgShowExtractPages
    {
        public TMsgShowExtractPages(PDFDocument document) 
        {
            this.Document = document;
        }

        public PDFDocument Document { get; private set; }
    }

    /// <summary>
    /// Type of the message for requesting to delete pages from the document.
    /// </summary>
    public class TMsgShowDeletePages
    {
        public TMsgShowDeletePages(PDFDocument document)
        {
            this.Document = document;
        }

        public PDFDocument Document { get; private set; }
    }

    /// <summary>
    /// Type of the message for requesting to insert pages into the document.
    /// </summary>
    public class TMsgShowInsertPages
    {
        public TMsgShowInsertPages(PDFDocument document)
        {
            this.Document = document;
        }

        public PDFDocument Document { get; private set; }
        public string FileToMerge { get; set; }
    }

    /// <summary>
    /// Type of the message for requesting to show the information window.
    /// </summary>
    public class TMsgShowAbout
    {
        public TMsgShowAbout()
        {
        }
    }
}
