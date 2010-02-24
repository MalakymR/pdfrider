using System;
using System.Collections.Generic;

namespace PDFRider
{
    public enum InformationType
    {
        All,
        PageRangeOutOfDocument
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
    public class TMsgClose
    {
        public TMsgClose(WindowViewModel sender_view_model, object dialog_return_value)
        {
            this.SenderViewModel = sender_view_model;
            this.DialogReturnValue = dialog_return_value;
        }
        public TMsgClose(WindowViewModel sender_view_model, bool? dialog_result)
        {
            this.SenderViewModel = sender_view_model;
            this.DialogResult = dialog_result;
        }
        public TMsgClose(WindowViewModel sender_view_model)
            : this(sender_view_model, null)
        {
        }

        public WindowViewModel SenderViewModel { get; private set; }
        public bool? DialogResult { get; set; }

        /// <summary>
        /// A custom value that a dialog window can return in place of the standard "bool? DialogResult"
        /// </summary>
        public object DialogReturnValue { get; set; }
    }

    /// <summary>
    /// Type of the message for sending information about window closing status.
    /// </summary>
    public class TMsgClosing
    {
        public TMsgClosing()
        {
            this.AskForSave = false;
            this.EventArgs = new System.ComponentModel.CancelEventArgs();
            
        }

        public bool AskForSave { get; set; }
        public System.ComponentModel.CancelEventArgs EventArgs { get; set; }
    }

    /// <summary>
    /// Type of the message for requesting to open a file.
    /// </summary>
    public class TMsgOpenFile
    {
        public TMsgOpenFile() 
        {
            this.Multiselect = false;
        }
        
        public bool Multiselect { get; set; }
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
    /// Type of the message for passing a list of files.
    /// </summary>
    public class TMsgAddFiles
    {
        public TMsgAddFiles(List<string> files)
        {
            this.Files = files;
        }

        public List<string> Files { get; set; }
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
    /// Type of the message for requesting to merge documents.
    /// </summary>
    public class TMsgShowMergeDocuments
    {
        public TMsgShowMergeDocuments(List<string> filesToMerge)
        {
            this.FilesToMerge = filesToMerge;
        }

        public TMsgShowMergeDocuments()
            : this(new List<string>())
        {
        }

        public List<string> FilesToMerge { get; private set; }
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
