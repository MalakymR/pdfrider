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

using System.Windows.Forms;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider
{
    /// <summary>
    /// A class to display MessageBoxes and CommonDialog windows.
    /// </summary>
    public class DialogController
    {
        public DialogController()
        {
            // Standard message to display a dialog message
            Messenger.Default.Register<DialogMessage>(this, MsgDialogMessage_Handler);

            Messenger.Default.Register<GenericMessageAction<MsgOpenFile, MsgOpenFile>>(this, this.MsgOpenFile_Handler);
            Messenger.Default.Register<GenericMessageAction<MsgSaveFile, MsgOpenFile>>(this, this.MsgSaveFile_Handler);
            Messenger.Default.Register<GenericMessageAction<MsgSelectFolder, string>>(this, this.MsgOpenFolder_Handler);
        }

        void MsgDialogMessage_Handler(DialogMessage msg)
        {
            var result = System.Windows.MessageBox.Show(
                        msg.Content,
                        msg.Caption,
                        msg.Button,
                        msg.Icon);

            // Send callback
            msg.ProcessCallback(result);
        }

        // --- OPEN FILE
        void MsgOpenFile_Handler(GenericMessageAction<MsgOpenFile, MsgOpenFile> msg)
        {
            MsgOpenFile data = msg.Data;

            FileDialogResult r = DialogController.ShowOpenFileDialog(data.Multiselect);
            data.FileName = r.FileName;
            data.FileNames = r.FileNames;
            data.NewFile = r.CommonDialogReturn;
            msg.Execute(data);
        }


        // --- SAVE FILE
        void MsgSaveFile_Handler(GenericMessageAction<MsgSaveFile, MsgOpenFile> msg)
        {
            FileDialogResult r = DialogController.ShowSaveFileDialog();
            msg.Execute(new MsgOpenFile(r.FileName, r.CommonDialogReturn));
        }



        // --- OPEN FOLDER
        void MsgOpenFolder_Handler(GenericMessageAction<MsgSelectFolder, string> msg) //MsgSelectFolder msg)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            fbd.Description = msg.Data.Description;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                msg.Execute(fbd.SelectedPath);
            }
        }


        /// <summary>
        /// Shows a OpenFileDialog window with standard settings for this application.
        /// </summary>
        /// <param name="multiselect">Indicates whether OpenFileDialog should allow multiple file selection</param>
        /// <returns>A structure containing information returned by a CommonDialog window.</returns>
        public static FileDialogResult ShowOpenFileDialog(bool multiselect)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "pdf";
            dlg.Multiselect = multiselect;
            dlg.Title = App.Current.FindResource("loc_openFileDialogTitle").ToString();
            dlg.Filter = App.Current.FindResource("loc_openFileDialogFilter").ToString();

            DialogResult dlgresult = dlg.ShowDialog();

            FileDialogResult result = new FileDialogResult();
            if (dlgresult == DialogResult.OK)
            {
                result.CommonDialogReturn = true;
            }
            result.FileName = dlg.FileName;

            ///* In multiselection, OpenFileDialog puts the last selected file at the beginning of the list (array)
            // * of the returned file. I avoid this with the below code. */


            List <string> fileNames = new List<string>();
            fileNames.AddRange(dlg.FileNames);
            if (fileNames.Count > 1)
            {
                fileNames.RemoveAt(0);
                fileNames.Add(dlg.FileNames[0]);
            }

            result.FileNames = fileNames.ToArray();

            return result;
        }

        public static FileDialogResult ShowOpenFileDialog()
        {
            return ShowOpenFileDialog(false);
        }

        /// <summary>
        /// Shows a SaveFileDialog window with standard settings for this application.
        /// </summary>
        /// <returns>A structure containing information returned by a CommonDialog window.</returns>
        public static FileDialogResult ShowSaveFileDialog()
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.DefaultExt = "pdf";
            dlg.Title = App.Current.FindResource("loc_saveFileDialogTitle").ToString();
            dlg.Filter = App.Current.FindResource("loc_saveFileDialogFilter").ToString();
            dlg.AddExtension = true;

            DialogResult dlgresult = dlg.ShowDialog();

            FileDialogResult result = new FileDialogResult();
            
            if (dlgresult == DialogResult.OK)
            {
                result.CommonDialogReturn = true;
            }
            result.FileName = dlg.FileName;

            return result;
        }

        
    }


    public struct FileDialogResult
    {
        public string[] FileNames { get; set; }
        public string FileName { get; set; }
        public bool CommonDialogReturn { get; set; }
    }
}
