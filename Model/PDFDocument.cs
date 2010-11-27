﻿/*
 *    Copyright 2009, 2010 Francesco Tonucci
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
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace PDFRider
{
    /// <summary>
    /// Contains properties and methods for working with the PDF document.
    /// </summary>
    public class PDFDocument
    {
        public enum OperationStates
        {
            Ok,
            PageRangeEntireDocument,
            PageRangeOutOfDocument,
            TooManyFiles,
            WrongPassword
        }

        public enum InsertPositions
        {
            Beginning,
            End,
            Custom
        }

        public enum PageIntervals
        {
            All,
            Odd,
            Even
        }

        //public enum Orientations
        //{
        //    All,
        //    Vertical,
        //    Horizontal
        //}

        public enum Rotations
        {
            North,  // 0 
            South,  // 180
            East,   // 90
            West,   // 90
            Left,   // -90
            Right,  // +90
            Down    // +180
        }


        private static string BASE_DIR = AppDomain.CurrentDomain.BaseDirectory;

        // File generated by the dump_data function of pdftk:
        //   it contains various informations about the pdf file.
        private const string DUMP_DATA_FILE_NAME = "pdfdata.txt";
        private static string DUMP_DATA_FILE = Path.Combine(BASE_DIR, DUMP_DATA_FILE_NAME);

        public const int MAX_HANDLES = 26;
        private static char[] PDF_HANDLES = new char[MAX_HANDLES] {'A','B','C','D','E','F','G','H','I','J',
            'K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};

        // Suffix used to obtain a valid file name
        static System.Text.RegularExpressions.Regex _fileSuffixRegex = new System.Text.RegularExpressions.Regex(
                @"(\[[0-9]+\])$"); // Any number inside square brackets
 

        public PDFDocument(string fileName)
        {
            this.FullName = fileName;

            PDFFileInfo info = PDFDocument.GetInfo(fileName);
            this.NumberOfPages = info.NumberOfPages;
            this.PageLabelStart = info.PageLabelStart;

            this.HasInfo = true;
            if (this.NumberOfPages == 0)
            {
                this.HasInfo = false;
            }

        }

        #region Properties

        /// <summary>
        /// Tells if it was possible to get informations about the pdf file
        /// using the dump_data function. If this value is FALSE, no operation can be
        /// done on the file.
        /// </summary>
        public bool HasInfo { get; private set; }

        /// <summary>
        /// Full path of the pdf file.
        /// </summary>
        public string FullName { get; private set; }
        
        /// <summary>
        /// Name of the pdf file.
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileName(this.FullName);
            }
        }

        /// <summary>
        /// Total number of pages of the document.
        /// </summary>
        public int NumberOfPages { get; private set; }

        /// <summary>
        /// There can be 20 pages in a document, but the page counting could start from 50.
        /// In this case, PageLabelStart will be 50.
        /// You need to know this value because if a user selects, for example, 54 as start page
        /// you need to subtract this value from it in order to obtain the actual page to pass to pdftk.
        /// </summary>
        public int PageLabelStart { get; private set; }


        public bool IsChanged { get; set; }


        #endregion


        #region Public methods

        /// <summary>
        /// Extracts contiguous pages from a pdf file and saves them into a new file.
        /// </summary>
        /// <param name="pageStart">First page to extract</param>
        /// <param name="pageEnd">Last page to extract</param>
        /// <param name="outputFileName">Path of the new file</param>
        /// <returns></returns>
        public OperationStates ExtractPages(int pageStart, int pageEnd, ref string outputFileName)
        {
            OperationStates state = OperationStates.Ok;

            if ((pageStart < 1) || (pageStart > this.NumberOfPages) ||
                (pageEnd < 1) || (pageEnd > this.NumberOfPages))
            {
                state = OperationStates.PageRangeOutOfDocument;
                return state;
            }

            string range = pageStart.ToString() + "-" + pageEnd.ToString();

            GetValidOutputFileName(ref outputFileName, 1);

            string args = "\"" + this.FullName + "\" cat " +
                range + " output \"" + outputFileName + "\"";

            RunPdftk(args);
                        
            return state;
        }


        /// <summary>
        /// Deletes one or more contiguous pages from a pdf file.
        /// </summary>
        /// <param name="pageStart">First page to delete</param>
        /// <param name="pageEnd">Last page to delete</param>
        /// <param name="outputFileName">Path of the new file (original file without the selected pages)</param>
        /// <param name="overwriteOriginal">Tells if the program should overwrite the original file</param>
        /// <returns></returns>
        public OperationStates DeletePages(int pageStart, int pageEnd, ref string outputFileName, bool overwriteOriginal)
        {
            OperationStates state = OperationStates.Ok;

            if ((pageStart < 1) || (pageStart > this.NumberOfPages) ||
                (pageEnd < 1) || (pageEnd > this.NumberOfPages) ||
                (pageStart > pageEnd))
            {
                state = OperationStates.PageRangeOutOfDocument;
                return state;
            }

            string rangeStart = pageStart - 1 > 0 ? "1-" + (pageStart - 1).ToString() : "";
            string rangeEnd = pageEnd < this.NumberOfPages ? (pageEnd + 1).ToString() + "-end" : "";

            if ((rangeStart == "") && (rangeEnd == ""))
            {
                state = OperationStates.PageRangeEntireDocument;
                return state;
            }

            GetValidOutputFileName(ref outputFileName, 1);
            
            string args = "\"" + this.FullName + "\" cat " +
                rangeStart + " " + rangeEnd + " output \"" + outputFileName + "\"";

            RunPdftk(args);

            return state;
        }


        /// <summary>
        /// Inserts pages of a pdf file into this document at a specified position.
        /// </summary>
        /// <param name="pageStart">Page to start insert from (after)</param>
        /// <param name="position">One of InsertPositions to start insert from</param>
        /// <param name="fileToMerge">Path of the file to merge with this document</param>
        /// <param name="outputFileName">Path of the new file</param>
        /// <returns></returns>
        public OperationStates InsertPages(int pageStart, InsertPositions position, string fileToMerge, ref string outputFileName)
        {
            OperationStates state = OperationStates.Ok;

            if ((pageStart < 0) || (pageStart > this.NumberOfPages))
            {
                state = OperationStates.PageRangeOutOfDocument;
                return state;
            }

            string range = "";

            if ((position == InsertPositions.Beginning) || (pageStart == 0))
            {
                range = "B A";
            }
            else if ((position == InsertPositions.End) || (pageStart == this.NumberOfPages))
            {
                range = "A B";
            }
            else
            {
                range = "A1-" + pageStart.ToString() + " B A" + (pageStart + 1).ToString() + "-end";
            }

            GetValidOutputFileName(ref outputFileName, 1);
            
            string args = "A=\"" + this.FullName + "\" B=\"" + fileToMerge + "\" cat " +
                range + " output \"" + outputFileName + "\"";

            RunPdftk(args);

            return state;
        }


        /// <summary>
        /// Rotates pages from a pdf file and saves them into a new file.
        /// </summary>
        /// <param name="pageStart">First page to rotate</param>
        /// <param name="pageEnd">Last page to rotate</param>
        /// <param name="interval">Page interval (all, even, odd)</param>
        /// <param name="rotation">Rotation type</param>
        /// <param name="outputFileName">Path of the new file</param>
        /// <returns></returns>
        public OperationStates RotatePages(int pageStart, int pageEnd, PageIntervals interval,
            Rotations rotation, ref string outputFileName)
        {
            OperationStates state = OperationStates.Ok;

            if ((pageStart < 1) || (pageStart > this.NumberOfPages) ||
                (pageEnd < 1) || (pageEnd > this.NumberOfPages))
            {
                state = OperationStates.PageRangeOutOfDocument;
                return state;
            }

            string s_rotation = "";
            switch (rotation)
            {
                case Rotations.North:
                    s_rotation += "N";
                    break;
                case Rotations.South:
                    s_rotation += "S";
                    break;
                case Rotations.East:
                    s_rotation += "E";
                    break;
                case Rotations.West:
                    s_rotation += "W";
                    break;
                case Rotations.Left:
                    s_rotation += "L";
                    break;
                case Rotations.Right:
                    s_rotation += "R";
                    break;
                case Rotations.Down:
                    s_rotation += "D";
                    break;
            }

            string range = "";

            if (pageStart > 1)
            {
                range += "1-" + (pageStart - 1).ToString() + " ";
            }

            /* pdftk rotates the pages using the "cat" option, which catenates pages from a pdf
             * into another pdf file. If I specify an odd or even range (e.g. 1-10odd) only odd
             * or even pages within that range are catenated in the output file, but that's not
             * what I want. I need ALL pages to be present in the new file and only odd or even
             * pages be rotated. To achieve this I need to break the interval like below. */
            switch (interval)
            {
                case PageIntervals.Even:
                    for (int i = pageStart; i <= pageEnd; i++)
                    {
                        range += i.ToString();
                        range += (i % 2 == 0) ? s_rotation + " " : " ";
                    }
                    break;
                case PageIntervals.Odd:
                    for (int i = pageStart; i <= pageEnd; i++)
                    {
                        range += i.ToString();
                        range += (i % 2 != 0) ? s_rotation + " " : " ";
                    }
                    break;
                case PageIntervals.All:
                    for (int i = pageStart; i <= pageEnd; i++)
                    {
                        range += i.ToString() + s_rotation + " ";
                    }
                    break;
            }        

            if (pageEnd < this.NumberOfPages)
            {
                range += " " + (pageEnd + 1).ToString() + "-" + this.NumberOfPages.ToString();
            }

            GetValidOutputFileName(ref outputFileName, 1);
            
            string args = "\"" + this.FullName + "\" cat " +
                range + " output \"" + outputFileName + "\"";

            RunPdftk(args);

            return state;
        }


        /// <summary>
        /// Merge some PDF documents togheter.
        /// </summary>
        /// <param name="filesToMerge">List of file to merge</param>
        /// <param name="outputFileName">Path of the new file</param>
        public static OperationStates MergeDocuments(PDFFileInfo[] filesToMerge, ref string outputFileName)
        {
            OperationStates state = OperationStates.Ok;

            if (filesToMerge.Length > PDF_HANDLES.Length)
            {
                state = OperationStates.TooManyFiles;
                return state;
            }

            GetValidOutputFileName(ref outputFileName, 1);
            
            string args = "";
            for (int i = 0; i < filesToMerge.Length; i++)
            {
                args += PDF_HANDLES[i] + "=\"" + filesToMerge[i].FullName + "\" ";
            }
            args += "cat ";
            for (int i = 0; i < filesToMerge.Length; i++)
            {
                if (filesToMerge[i].PageRanges.Count > 0)
                {
                    foreach (string s in filesToMerge[i].PageRanges)
                    {
                        args += PDF_HANDLES[i] + s + " ";
                    }
                }
                else
                {
                    args += PDF_HANDLES[i] + " ";
                }
            }
            args += "output \"" + outputFileName + "\"";

            RunPdftk(args);
            
            return state;
        }


        /// <summary>
        /// Splits a single, input PDF document into individual	pages.
        /// </summary>
        /// <param name="destinationDirectory">Directory where save the burst pages.</param>
        /// <param name="prefix">Prefix used to name the burst pages.</param>
        /// <returns></returns>
        public OperationStates Burst(string destinationDirectory, string prefix)
        {
            OperationStates state = OperationStates.Ok;

            if (Directory.Exists(destinationDirectory))
            {
                if (String.Compare(Path.GetFullPath(destinationDirectory).TrimEnd('\\'),
                                   Path.GetFullPath(BASE_DIR).TrimEnd('\\'),
                                   StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    // Copy pdftk.exe to destinationDirectory, because burst files are placed in the 
                    // pdftk working directory.
                    File.Copy(Path.Combine(BASE_DIR, "pdftk.exe"), Path.Combine(destinationDirectory, "pdftk.exe"), true);
                }
                
                string inputFileName = this.FullName;

                string args = "\"" + inputFileName + "\" burst output " + prefix + "%04d.pdf";

                RunPdftk(args, destinationDirectory);

                // Wait 1 sec to allow the system to release the pdftk.exe process
                // I hate this but I didn't find a better solution...
                System.Threading.Thread.Sleep(1000);

                // Remove the pdftk.exe previously copied
                File.Delete(Path.Combine(destinationDirectory, "pdftk.exe"));

                // Remove doc_data.txt (report file generated by pdftk)
                File.Delete(Path.Combine(destinationDirectory, "doc_data.txt"));
            }
            
            return state;
        }


        /// <summary>
        /// Encrypt a PDF document with a user or owner password.
        /// </summary>
        /// <param name="userPassword">Password to open the document. Set null if not needed.</param>
        /// <param name="ownerPassword">Password to edit the document. Set null if not needed.</param>
        /// <returns></returns>
        public OperationStates Encrypt(System.Security.SecureString userPassword, System.Security.SecureString ownerPassword,
            bool allowPrinting, bool allowDegradatedPrinting, bool allowModifyContents, bool allowAssembly,
            bool allowCopyContents, bool allowScreenReaders, bool allowModifyAnnotations,
            bool allowFillIn, bool allowAllFeatures, ref string outputFileName)
        {
            OperationStates state = OperationStates.Ok;

            if ((userPassword == null) && (ownerPassword == null)) return state;

            string allow = "";
            if (allowAllFeatures)
            {
                allow = "AllFeatures";
            }
            else
            {
                if (allowPrinting)
                {
                    allow += "Printing ";
                }
                if (allowDegradatedPrinting)
                {
                    allow += "DegradatedPrinting ";
                }
                if (allowModifyContents)
                {
                    allow += "ModifyContents ";
                    allowAssembly = true;
                }
                if (allowAssembly)
                {
                    allow += "Assembly ";
                }
                if (allowCopyContents)
                {
                    allow += "CopyContents ";
                    allowScreenReaders = true;
                }
                if (allowScreenReaders)
                {
                    allow += "ScreenReaders ";
                }
                if (allowModifyAnnotations)
                {
                    allow += "ModifyAnnotations ";
                    allowFillIn = true;
                }
                if (allowFillIn)
                {
                    allow += "FillIn ";
                }
            }

            string passwords = "";
            if (userPassword != null)
            {
                IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(userPassword);

                passwords += "user_pw " + System.Runtime.InteropServices.Marshal.PtrToStringAuto(ptr);
            }
            if (ownerPassword != null)
            {
                IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(ownerPassword);

                passwords += " owner_pw " + System.Runtime.InteropServices.Marshal.PtrToStringAuto(ptr);

                if (allow != "")
                {
                    passwords += " allow " + allow;
                }
            }

            if (passwords != "")
            {
                string inputFileName = this.FullName;
                GetValidOutputFileName(ref outputFileName, 1);

                string args = "\"" + inputFileName + "\" output \"" + outputFileName + "\" " + passwords;

                userPassword = null;
                ownerPassword = null;

                RunPdftk(args);
            }
            
            return state;
        }


        /// <summary>
        /// Try to decrypt the document with the given password
        /// </summary>
        /// <param name="password">Password to decrypt the document</param>
        /// <returns></returns>
        public OperationStates TryDecrypt(System.Security.SecureString password)
        {
            OperationStates state = OperationStates.Ok;

            string inputFileName = this.FullName;

            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(password);
            string pwd = System.Runtime.InteropServices.Marshal.PtrToStringAuto(ptr);

            string args = "\"" + inputFileName + "\" input_pw " + pwd + " dump_data output pw.check dont_ask";

            pwd = null;

            if (RunPdftk(args) > 0)
            {
                state = OperationStates.WrongPassword;
            }

            if (File.Exists("pw.check"))
                File.Delete("pw.check");

            return state;
        }


        /// <summary>
        /// Decrypt the document with the given password
        /// </summary>
        /// <param name="password">Password to decrypt the document</param>
        /// <returns></returns>
        public OperationStates Decrypt(System.Security.SecureString password, ref string outputFileName)
        {
            OperationStates state = OperationStates.Ok;

            string inputFileName = this.FullName;

            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(password);
            string pwd = System.Runtime.InteropServices.Marshal.PtrToStringAuto(ptr);

            GetValidOutputFileName(ref outputFileName, 1);

            string args = "\"" + inputFileName + "\" input_pw " + pwd + " output \"" + outputFileName + "\" dont_ask";

            pwd = null;

            if (RunPdftk(args) > 0)
            {
                state = OperationStates.WrongPassword;
            }

            return state;
        }


        /// <summary>
        /// Open the current PDF document with the default application.
        /// </summary>
        public void Open()
        {
            Process p = new Process();

            p.StartInfo.FileName = this.FullName;
            
            p.Start();
        }

        /// <summary>
        /// Open a PDF document with the default application.
        /// </summary>
        /// <param name="fileName">Path of the file to open</param>
        public static void Open(string fileName)
        {
            Process p = new Process();

            p.StartInfo.FileName = fileName;

            p.Start();
        }

        /// <summary>
        /// Open a PDF document with PDFRider.
        /// </summary>
        /// <param name="fileName">Path of the file to open</param>
        public static void OpenWithPdfRider(string fileName, string args)
        {
            Process p = new Process();

            p.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "PDFRider.exe");
            p.StartInfo.Arguments = "\"" + fileName + "\" " + args;
            
            p.Start();
        }

        #endregion


        #region Private methods

        /// <summary>
        /// Executes pdftk with the provided arguments.
        /// </summary>
        /// <param name="args">Command line arguments to pass to pdftk.</param>
        private static int RunPdftk(string args)
        {
            return RunPdftk(args, BASE_DIR);
        }

        /// <summary>
        /// Executes pdftk with the provided arguments.
        /// </summary>
        /// <param name="args">Command line arguments to pass to pdftk.</param>
        /// <param name="workingDirectory">Working directory for pdftk.</param>
        private static int RunPdftk(string args, string workingDirectory)
        {
            int ret = 0;

            Process p = new Process();

            p.StartInfo.FileName = "pdftk.exe";
            p.StartInfo.WorkingDirectory = workingDirectory;

            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            p.StartInfo.Arguments = args;

            p.Start();
            p.WaitForExit();
            ret = p.ExitCode;
            p.Close();

            return ret;
        }


        /// <summary>
        /// Gets informations about the pdf file specified by <paramref name="path"/>
        /// using the "dump_data" function of pdftk.
        /// </summary>
        /// <param name="path">Full path of the file that you want to get information from.</param>
        /// <returns>PDFFileInfo object storing basic information about PDF document.</returns>
        public static PDFFileInfo GetInfo(string path)
        {
            PDFFileInfo info = new PDFFileInfo();

            //Creates a process for pdftk and sets its parameters.
            Process p = new Process();

            //Handles possible errors of pdftk.
            try
            {
                p.StartInfo.FileName = "pdftk.exe";
                p.StartInfo.WorkingDirectory = BASE_DIR;

                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //Sets pdftk command line arguments.
                //You must use quotation marks to specify command line arguments that have
                //spaces inside them, or they will be considered as different arguments.
                p.StartInfo.Arguments = "\"" + path + "\" dump_data output \"" + DUMP_DATA_FILE + "\" dont_ask";

                p.Start();
                p.WaitForExit();
                //Console.WriteLine("pdftk exit code: " + p.ExitCode.ToString());
                p.Close();

                if (File.Exists(DUMP_DATA_FILE))
                {
                    info.FullName = path;

                    //Searches the informations inside pdftk dump_data file.
                    using (StreamReader sr = new StreamReader(DUMP_DATA_FILE))
                    {
                        String line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("NumberOfPages"))
                            {
                                info.NumberOfPages = short.Parse(line.Remove(0, 15));
                            }

                            if (line.StartsWith("PageLabelStart"))
                            {
                                info.PageLabelStart = short.Parse(line.Remove(0, 16));
                            }
                        }
                    }

                    // PageLabelStart is used to get the effective number of pages,
                    // so it must be greater than 0
                    if (info.PageLabelStart == 0)
                        info.PageLabelStart = 1;
                }

            }
            catch
            {
            }
            finally
            {
                if (File.Exists(DUMP_DATA_FILE))
                    File.Delete(DUMP_DATA_FILE);
            }

            return info;
        }



        /// <summary>
        /// Checks if <paramref name="fileName"/> already exists end generate a valid file name.
        /// </summary>
        /// <param name="fileName">The file name to check</param>
        /// <param name="fileIndex">Start index to name the file. You should set this to 1.</param>
        /// <remarks>The application hangs if you try to write a file (in this case a temporary file)
        /// that is in use by another process (or an instance of this process).</remarks>
        private static void GetValidOutputFileName(ref string fileName, short fileIndex)
        {
            /* 1. Check if fileName exists
             * 2. If it does, check if it ends with a number inside square brackets ([fileIndex])
             * 3. If it does, remove the suffix
             * 4. Append the suffix "[fileIndex]" to fileName
             * 5. Restart the check
             * 6. fileName will be set to a valid file name. */

            if (File.Exists(fileName))
            {
                if (_fileSuffixRegex.IsMatch(Path.GetFileNameWithoutExtension(fileName)))
                {
                    int suffix_length = fileIndex.ToString().Length + 2;
                    fileName = Path.Combine(Path.GetDirectoryName(fileName),
                        Path.GetFileNameWithoutExtension(fileName).Remove(
                        Path.GetFileNameWithoutExtension(fileName).Length - suffix_length) +
                        Path.GetExtension(fileName));
                }

                fileName = Path.Combine(Path.GetDirectoryName(fileName),
                    Path.GetFileNameWithoutExtension(fileName) + "[" + fileIndex.ToString() + "]" +
                    Path.GetExtension(fileName));

                fileIndex++;
                GetValidOutputFileName(ref fileName, fileIndex);
            }
        }

        #endregion

    }


    
}
