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
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PDFRider.Updater
{
    /// <summary>
    /// Provides static methods for updates checking
    /// </summary>
    public class Updater
    {
        internal const string CREDENTIAL_FILE_NAME = "pdfrider.update.auth";
        internal static string CREDENTIAL_FILE = Path.Combine(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), App.NAME),
            CREDENTIAL_FILE_NAME);


        /// <summary>
        /// Loads Updater configuration from the configuration file
        /// </summary>
        /// <returns>Updater config</returns>
        public static UpdaterConfig LoadConfig()
        {
            UpdaterConfig config;

            try
            {
                config = new UpdaterConfig();
                
                config.CheckAtStartup = Properties.UpdaterSettings.Default.CheckAtStartup;
                config.RemoteUrl = Properties.UpdaterSettings.Default.RemoteUrl;
            }
            catch
            {
                config = null;
            }

            return config;
        }

        /// <summary>
        /// Saves Updater configuration to the file
        /// </summary>
        /// <param name="config">UpdaterConfig object containing the configurations to save</param>
        public static void SaveConfig(UpdaterConfig config)
        {
            Properties.UpdaterSettings.Default.CheckAtStartup = config.CheckAtStartup;
            Properties.UpdaterSettings.Default.Save();
        }


        /// <summary>
        /// Checks if a newer version of the application is available
        /// </summary>
        /// <returns>VersionInfo object containing informations about version checking</returns>
        public static VersionInfo CheckForUpdates(string currentVersion)
        {
            VersionInfo info = null;

            UpdaterConfig config = Updater.LoadConfig();
            if (config == null) return null;

            XmlDocument remoteDoc = null;

            // Try to load the remote version file.
            remoteDoc = GetRemoteData(config.RemoteUrl, null);
            
            if (remoteDoc == null)
            {
                // Maybe we are behind a proxy...
                WebProxy wp = new WebProxy(WebRequest.DefaultWebProxy.GetProxy(new Uri(config.RemoteUrl)));

                // Uses the stored credentials
                if (File.Exists(CREDENTIAL_FILE))
                {
                    XmlDocument credentialsDoc = new XmlDocument();
                    credentialsDoc.Load(CREDENTIAL_FILE);

                    string username = credentialsDoc.DocumentElement.SelectSingleNode("./username").InnerXml;
                    string password = credentialsDoc.DocumentElement.SelectSingleNode("./password").InnerXml;
                    
                    wp.Credentials = new NetworkCredential(username, CryptoDLL.FTCrypt.Decrypt(password));

                    remoteDoc = GetRemoteData(config.RemoteUrl, wp);
                }

                // If the stored credentials doesn't work, 
                // asks for new ones until they work.
                while (remoteDoc == null)
                {
                    WndCredentials wndCredentials = new WndCredentials();
                    WndCredentialsViewModel wndCredentialsViewModel = new WndCredentialsViewModel(wp.Address);

                    wndCredentials.DataContext = wndCredentialsViewModel;
                    wndCredentials.ShowDialog();
                    wp.Credentials = wndCredentialsViewModel.ReturnValue as NetworkCredential;

                    if (wp.Credentials == null) break;

                    remoteDoc = GetRemoteData(config.RemoteUrl, wp);
                }
            }
                
            // If we got the remote file, compare the versions
            if (remoteDoc != null)
            {
                info = new VersionInfo();

                info.NewVersion = remoteDoc.DocumentElement.SelectSingleNode("./LatestVersion").InnerXml;

                if (Updater.CompareVersions(info.NewVersion, currentVersion) > 0)
                {
                    info.NewVersionAvailable = true;
                }
            }
            

            return info;
        }

        /// <summary>
        /// Loads an XML document from a remote url, using the specified proxy.
        /// </summary>
        /// <param name="remoteUrl">Url of the remote XML document.</param>
        /// <param name="wp">WebProxy to use for internet connections. 
        /// Set to "null" if the connection doesn't require a proxy.</param>
        /// <returns></returns>
        private static XmlDocument GetRemoteData(string remoteUrl, WebProxy wp)
        {
            XmlDocument doc = new XmlDocument();

            WebClient wc = new WebClient();
            if (wp != null)
                wc.Proxy = wp;

            try
            {
                MemoryStream ms = new MemoryStream(wc.DownloadData(remoteUrl));
                XmlTextReader rdr = new XmlTextReader(ms);
            
                doc.Load(rdr);
            }
            catch
            {
                // Network error?
                doc = null;
            }
            
            return doc;
            
        }

        /// <summary>
        /// Compares two versions in the format Major.Minor.Build.Revision
        /// You can also supply shorter versions. In this case the version are evaluated from left to right
        /// (e.g. v1.5 > v1.4.3)
        /// </summary>
        /// <param name="version1">First version to compare.</param>
        /// <param name="version2">Second version to compare.</param>
        /// <returns>-1 if version1 < version2
        ///           0 if version1 == version2
        ///           1 if version1 > version2</returns>
        public static int CompareVersions(string version1, string version2)
        {
            int ret = 0;

            string[] v1 = version1.Split('.');
            string[] v2 = version2.Split('.');

            // Checks Major version
            if (int.Parse(v1[0]) < int.Parse(v2[0])) ret = -1;
            if (int.Parse(v1[0]) > int.Parse(v2[0])) ret = 1;
            if (int.Parse(v1[0]) == int.Parse(v2[0]))
            {
                if ((v1.Length == 1) || (v2.Length == 1))
                {
                    if (v1.Length < v2.Length) ret = -1;
                    if (v1.Length > v2.Length) ret = 1;
                }
                else
                {
                    // Checks Minor version
                    if (int.Parse(v1[1]) < int.Parse(v2[1])) ret = -1;
                    if (int.Parse(v1[1]) > int.Parse(v2[1])) ret = 1;
                    if (int.Parse(v1[1]) == int.Parse(v2[1]))
                    {
                        if ((v1.Length == 2) || (v2.Length == 2))
                        {
                            if (v1.Length < v2.Length) ret = -1;
                            if (v1.Length > v2.Length) ret = 1;
                        }
                        else
                        {
                            // Checks Revision
                            if (int.Parse(v1[2]) < int.Parse(v2[2])) ret = -1;
                            if (int.Parse(v1[2]) > int.Parse(v2[2])) ret = 1;
                            if (int.Parse(v1[2]) == int.Parse(v2[2]))
                            {
                                if ((v1.Length == 3) || (v2.Length == 3))
                                {
                                    if (v1.Length < v2.Length) ret = -1;
                                    if (v1.Length > v2.Length) ret = 1;
                                }
                                else
                                {
                                    // Checks Build
                                    if (int.Parse(v1[3]) < int.Parse(v2[3])) ret = -1;
                                    if (int.Parse(v1[3]) > int.Parse(v2[3])) ret = 1;
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }
    }
}
