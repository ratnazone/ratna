/*
    Copyright (c) 2012, Jardalu LLC. (http://jardalu.com)
        
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
  
    For complete licensing, see license.txt or visit http://ratnazone.com/v0.2/license.txt

*/
namespace Jardalu.Ratna.Web.UI.Snippets
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Web;

    using Jardalu.Ratna.Utilities;

    #endregion

    public class SnippetManager
    {
        
        #region private fields

        private static object syncRoot = new object();
        private static SnippetManager instance;
        private static Logger logger;

        private static string[] controlsPath = new string[1] { "~/r-admin/controls" };

        private Dictionary<string, string> controls = new Dictionary<string, string>();

        #endregion

        #region ctor

        static SnippetManager()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        private SnippetManager()
        {
            if (controlsPath != null)
            {
                foreach (string rootPath in controlsPath)
                {
                    // get the actual path
                    string actualPath = HttpContext.Current.Server.MapPath(rootPath);

                    RegisterControlsFromFolder(new DirectoryInfo(actualPath));
                }
            }
        }

        #endregion

        #region public properties

        public static SnippetManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SnippetManager();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region public methods

        public SnippetAction Parse(NameValueCollection queryString)
        {

            // rules for parsing
            //
            // _s must be 1
            // _sn must be present if _s is present
            // _sk is optional

            SnippetAction action = new SnippetAction();

            if (queryString != null)
            {
                string s = queryString[Constants.Snippet.SnippetIdentifier];
                int svalue;

                logger.Log(LogLevel.Debug, "Parsing queryString with - [{0}:{1}] ", Constants.Snippet.SnippetIdentifier, s);

                if (Int32.TryParse(s, out svalue) && 
                    svalue == 1)
                {
                    // look for _sn
                    string sn = queryString[Constants.Snippet.SnippetNameIdentifier];
                    logger.Log(LogLevel.Debug, "Getting snippet name - [{0}:{1}] ", Constants.Snippet.SnippetNameIdentifier, sn);
                    if (!string.IsNullOrEmpty(sn))
                    {
                        action.Name = sn;
                        action.IsEnabled = true;
                    }

                    string sk = queryString[Constants.Snippet.SnippetKeysIdentifier];
                    logger.Log(LogLevel.Debug, "Getting snippet keys - [{0}:{1}] ", Constants.Snippet.SnippetKeysIdentifier, sk);
                    if (!string.IsNullOrEmpty(sk))
                    {
                        string[] tokens = sk.Split(Constants.Snippet.SnippetKeysToken, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string token in tokens)
                        {                            
                            action.Add(token, queryString[token]);
                        }
                    }
                }
            }

            return action;
        }

        public void RegisterControl(string name, string location)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentNullException("location");
            }

            if (!this.controls.ContainsKey(name))
            {
                lock (syncRoot)
                {
                    if (!this.controls.ContainsKey(name))
                    {
                        this.controls[name] = location;
                    }
                }
            }
        }

        public bool IsRegistered(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return this.controls.ContainsKey(name);
        }

        public string GetControlPath(string name)
        {
            bool exists = IsRegistered(name);

            if (!exists)
            {
                throw new InvalidOperationException();
            }

            return this.controls[name];
        }

        #endregion

        #region private methods

        private void RegisterControlsFromFolder(DirectoryInfo dInfo)
        {
            if (dInfo != null)
            {
                DirectoryInfo[] infos = dInfo.GetDirectories();
                if (infos != null)
                {
                    foreach (DirectoryInfo info in infos)
                    {
                        RegisterControlsFromFolder(info);
                    }
                }

                // read all the controls and register them
                FileInfo[] fInfos = dInfo.GetFiles("*.ascx");
                if (fInfos != null)
                {
                    foreach (FileInfo fInfo in fInfos)
                    {

                        string snippetName = Path.GetFileNameWithoutExtension(fInfo.Name);
                        logger.Log(LogLevel.Info, "Registering control [{0}] from [{1}]", snippetName, fInfo.FullName);

                        // extract the name of the control
                        // abc.ascx ==> abc and register with that name
                        RegisterControl(
                                snippetName, 
                                Jardalu.Ratna.Web.Utility.GetVirtualPath(fInfo.FullName)
                            );
                    }
                }
            }
        }

        #endregion

    }
}
