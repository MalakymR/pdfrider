/*
 *    Copyright 2009-2011 Francesco Tonucci
 * 
 * This file is part of PDFRider.
 * 
 * PDFRider is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * PDFRider is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with PDFRider; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 * 
 * 
 * Project page: http://pdfrider.codeplex.com
*/

using System;
using System.Collections.Generic;

namespace PDFRider
{
    /* ***************************************************************************************
     * Define here the classes that a sender object can use to pass data to a recipient.
     * 
     * The classes are also used to identify the type of the massage sent, so that a recipient
     * can register itself to handle that specific message.
     * *************************************************************************************** */

    
    /// <summary>
    /// Base class for messages requesting to show a tool window.
    /// </summary>
    public abstract class MsgShowToolWindow
    {
        public MsgShowToolWindow(PDFDocument document)
        {
            this.Document = document;
        }

        public PDFDocument Document { get; private set; }
    }


    /// <summary>
    /// Message for requesting to close a window.
    /// </summary>
    public class MsgClose
    {
        public MsgClose(WindowViewModel senderViewModel)
        {
            this.SenderViewModel = senderViewModel;
        }
        
        public WindowViewModel SenderViewModel { get; private set; }
    }

    /// <summary>
    /// Message for sending information about window closing status.
    /// </summary>
    public class MsgClosing
    {
        public object Data { get; set; }
        public bool Cancel { get; set; }
    }


    /// <summary>
    /// Message for requesting to open a file.
    /// </summary>
    public class MsgOpenFile
    {
        public MsgOpenFile(string fileName, bool newFile, bool newWindow)
        {
            this.FileName = fileName;
            this.NewFile = newFile;
            this.NewWindow = newWindow;

            this.Multiselect = false;
        }

        public MsgOpenFile(string fileName, bool newFile)
            : this(fileName, newFile, false)
        {
            this.FileName = fileName;
            this.NewFile = newFile;
        }

        public MsgOpenFile(string fileName)
            : this(fileName, false)
        {
        }

        public MsgOpenFile()
            : this(null)
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


    /// <summary>
    /// Message for requesting to save a file
    /// </summary>
    public class MsgSaveFile { }


    /// <summary>
    /// Message for requesting to select a folder
    /// </summary>
    public class MsgSelectFolder
    {
        public string Description { get; set; }
    }



    /// <summary>
    /// Message for requesting to extract the pages from the document.
    /// </summary>
    public class MsgShowExtractPages : MsgShowToolWindow
    {
        public MsgShowExtractPages(PDFDocument document)
            : base(document)
        { }
    }


    /// <summary>
    /// Message for requesting to delete pages from the document.
    /// </summary>
    public class MsgShowDeletePages : MsgShowToolWindow
    {
        public MsgShowDeletePages(PDFDocument document)
            : base(document)
        { }
    }


    /// <summary>
    /// Message for requesting to insert pages into the document.
    /// </summary>
    public class MsgShowInsertPages : MsgShowToolWindow
    {
        public MsgShowInsertPages(PDFDocument document)
            : base(document)
        { }

        public string FileToMerge { get; set; }
    }


    /// <summary>
    /// Message for requesting to rotate the pages of the document.
    /// </summary>
    public class MsgShowRotatePages : MsgShowToolWindow
    {
        public MsgShowRotatePages(PDFDocument document)
            : base(document)
        { }

    }


    /// <summary>
    /// Message for requesting to merge documents.
    /// </summary>
    public class MsgShowMergeDocuments
    {
        public MsgShowMergeDocuments(List<string> filesToMerge)
        {
            this.FilesToMerge = filesToMerge;
        }

        public MsgShowMergeDocuments()
            : this(new List<string>())
        {
        }

        public List<string> FilesToMerge { get; private set; }
    }


    /// <summary>
    /// Message for requesting to burst the document.
    /// </summary>
    public class MsgShowBurst : MsgShowToolWindow
    {
        public MsgShowBurst(PDFDocument document)
            : base(document)
        { }

    }

    /// <summary>
    /// Message for requesting to open the security window.
    /// </summary>
    public class MsgShowSecurity : MsgShowToolWindow
    {
        public MsgShowSecurity(PDFDocument document)
            : base(document)
        { }

    }


    /// <summary>
    /// Message for requesting to confirm a password.
    /// </summary>
    public class MsgShowConfirmPassword
    {
        public MsgShowConfirmPassword(System.Security.SecureString password, PasswordTypes type)
        {
            this.OriginalPassword = password;
            this.PasswordType = type;
        }

        public System.Security.SecureString OriginalPassword { get; set; }
        public PasswordTypes PasswordType { get; set; }
    }


    /// <summary>
    /// Message for requesting to enter a password.
    /// </summary>
    public class MsgShowEnterPassword
    {
        public MsgShowEnterPassword(PDFDocument document)
        {
            this.Document = document;
        }

        public PDFDocument Document { get; private set; }
    }

    /// <summary>
    /// Message for requesting to show the "new version available" window.
    /// </summary>
    public class MsgShowNewVersion
    {
        public MsgShowNewVersion(Updater.VersionInfo info)
        {
            this.Info = info;
        }

        public Updater.VersionInfo Info { get; set; }
    }

    /// <summary>
    /// Message for requesting to show the information window.
    /// </summary>
    public class MsgShowAbout
    {
    }
}
