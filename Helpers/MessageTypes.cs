using System;
using System.Collections.Generic;

namespace PDFRider
{
    /* The main purpose of message types is to pass information from view to view model and viceversa,
     * or between view models.
     * I would use structs for them, but I have to use classes when I must avoid or override 
     * the default constructor. */

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
    public struct TMsgClosing
    {
        public bool AskForSave { get; set; }
        public object Data { get; set; }
        public bool Cancel { get; set; }
    }


    /// <summary>
    /// Type of the message to open a file.
    /// </summary>
    public struct TMsgOpenFile
    {
        public TMsgOpenFile(string file_name, bool new_file, bool new_window)
            : this()
        {
            this.FileName = file_name;
            this.NewFile = new_file;
            this.NewWindow = new_window;

            this.Multiselect = false;

        }

        public TMsgOpenFile(string file_name, bool new_file)
            : this(file_name, new_file, false)
        {
            this.FileName = file_name;
            this.NewFile = new_file;
        }

        public TMsgOpenFile(string file_name)
            : this(file_name, false)
        {
        }

        /// <summary>
        /// Full name of the file to open
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Full names of the files to open
        /// </summary>
        public string[] FileNames { get; set; }

        /// <summary>
        /// *True* if the file does not come from an operation
        /// </summary>
        public bool NewFile { get; set; }

        /// <summary>
        /// *True* to open the file in a new window
        /// </summary>
        public bool NewWindow { get; set; }

        /// <summary>
        /// Allow to open more than one file
        /// </summary>
        public bool Multiselect { get; set; }

    }


    public struct TMsgSaveFile
    {
    }


    public struct TMsgSelectFolder
    {
        public string Description { get; set; }
    }


    /// <summary>
    /// Base class for messages requesting to show a tool window.
    /// </summary>
    public class TMsgShowToolWindow
    {
        public TMsgShowToolWindow(PDFDocument document)
        {
            this.Document = document;
        }

        public PDFDocument Document { get; private set; }
    }


    /// <summary>
    /// Type of the message for requesting to extract the pages from the document.
    /// </summary>
    public class TMsgShowExtractPages : TMsgShowToolWindow
    {
        public TMsgShowExtractPages(PDFDocument document)
            : base(document)
        { }
    }


    /// <summary>
    /// Type of the message for requesting to delete pages from the document.
    /// </summary>
    public class TMsgShowDeletePages : TMsgShowToolWindow
    {
        public TMsgShowDeletePages(PDFDocument document)
            : base(document)
        { }
    }


    /// <summary>
    /// Type of the message for requesting to insert pages into the document.
    /// </summary>
    public class TMsgShowInsertPages : TMsgShowToolWindow
    {
        public TMsgShowInsertPages(PDFDocument document)
            : base(document)
        { }

        public string FileToMerge { get; set; }
    }


    /// <summary>
    /// Type of the message for requesting to rotate the pages of the document.
    /// </summary>
    public class TMsgShowRotatePages : TMsgShowToolWindow
    {
        public TMsgShowRotatePages(PDFDocument document)
            : base(document)
        { }

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
    /// Type of the message for requesting to burst the document.
    /// </summary>
    public class TMsgShowBurst : TMsgShowToolWindow
    {
        public TMsgShowBurst(PDFDocument document)
            : base(document)
        { }

    }


    /// <summary>
    /// Type of the message for requesting to open the security window.
    /// </summary>
    public class TMsgShowSecurity : TMsgShowToolWindow
    {
        public TMsgShowSecurity(PDFDocument document)
            : base(document)
        { }

    }


    /// <summary>
    /// Type of the message for requesting to confirm a password.
    /// </summary>
    public struct TMsgConfirmPassword
    {
        public enum PasswordTypes
        {
            Open,
            Edit
        }

        public TMsgConfirmPassword(System.Security.SecureString password, PasswordTypes type)
            :this()
        {
            this.OriginalPassword = password;
            this.PasswordType = type;
        }

        public System.Security.SecureString OriginalPassword { get; set; }
        public PasswordTypes PasswordType { get; set; }
    }


    /// <summary>
    /// Type of the message for requesting to enter a password.
    /// </summary>
    public struct TMsgEnterPassword
    {
        public TMsgEnterPassword(PDFDocument document)
            :this()
        {
            this.Document = document;
        }

        public PDFDocument Document { get; private set; }
    }

    /// <summary>
    /// Type of the message for requesting to show the information window.
    /// </summary>
    public struct TMsgShowAbout
    {
    }
}
