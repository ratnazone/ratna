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

namespace Jardalu.Ratna.Web.Controls.Common
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.UI;
    using System.Web;

    using Jardalu.Ratna.Core;

    #endregion

    /// <summary>
    /// ScriptManager works only when <form runat="server"></form> exists on the page
    /// </summary>
    public partial class ClientJavaScript : System.Web.UI.UserControl
    {
        #region private fields

        /// <summary>
        /// Keeps track of the script includes and script variables.
        /// </summary>
        private static Dictionary<Guid, Pair> scripts = new Dictionary<Guid, Pair>();

        private static object syncRoot = new object();

        private const string scriptTagFormat = @"<script type=""text/javascript"" src=""{0}""></script>";
        private const string scriptVariableFormat = @"var {0}={1};";

        #endregion

        #region ctor

        public ClientJavaScript()
        {
            // get the sticky id for the request
            Guid stickyId = WebContext.Current.StickyId;
            if (!scripts.ContainsKey(stickyId))
            {
                lock (syncRoot)
                {
                    if (!scripts.ContainsKey(stickyId))
                    {
                        Dictionary<string, Pair> scriptIncludes = new Dictionary<string, Pair>();
                        Dictionary<string, Pair>  scriptVariables = new Dictionary<string, Pair>();

                        scripts[stickyId] = new Pair() { First = scriptIncludes, Second = scriptVariables };
                    }
                }
            }
        }

        #endregion

        #region public methods

        public static void Clean()
        {
            if (scripts.ContainsKey(WebContext.Current.StickyId))
            {
                lock (syncRoot)
                {
                    if (scripts.ContainsKey(WebContext.Current.StickyId))
                    {
                        scripts.Remove(WebContext.Current.StickyId);
                    }
                }
            }
        }

        public void RegisterClientScriptInclude(string key, string url)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (ScriptIncludes == null)
            {
                throw new InvalidOperationException();
            }

            if (!ScriptIncludes.ContainsKey(key))
            {
                ScriptIncludes[key] = new Pair(url, false);
            }
        }

        public void RegisterClientScriptVariable(string variableName, object value)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                throw new ArgumentNullException("variableName");
            }

            if (!ScriptVariables.ContainsKey(variableName))
            {
                ScriptVariables[variableName] = new Pair(value, false);
            }
        }

        #endregion

        #region private properties

        private Dictionary<string, Pair> ScriptIncludes
        {
            get
            {
                return scripts[WebContext.Current.StickyId].First as Dictionary<string, Pair>;
            }
        }

        private Dictionary<string, Pair> ScriptVariables
        {
            get
            {
                return scripts[WebContext.Current.StickyId].Second as Dictionary<string, Pair>;
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
                StringBuilder script = new StringBuilder();

                if (ScriptIncludes.Count > 0)
                {
                    foreach (Pair pair in ScriptIncludes.Values)
                    {
                        string url = pair.First as string;
                        bool rendered = (bool)pair.Second;
                        if (!rendered)
                        {
                            script.AppendLine(string.Format(scriptTagFormat, url));
                        }
                        pair.Second = true;
                    }
                }

                if (ScriptVariables.Count > 0)
                {

                    script.AppendLine(@"<script type=""text/javascript"">");

                    foreach (KeyValuePair<string, Pair> kvp in ScriptVariables)
                    {
                        string variableName = kvp.Key;
                        object value = kvp.Value.First;

                        bool rendered = (bool)kvp.Value.Second;
                        if (!rendered)
                        {

                        string data = null;
                        if (value == null)
                        {
                            data = string.Format(scriptVariableFormat, variableName, "null");
                        }
                        else if (value.GetType() == typeof(string))
                        {
                            data = string.Format(scriptVariableFormat, variableName, string.Format("\"{0}\"", value));
                        }
                        else
                        {
                            data = string.Format(scriptVariableFormat, variableName, value);
                        }

                        script.AppendLine(data);
                        }

                        kvp.Value.Second = true;
                    }

                    script.AppendLine("</script>");
                }

                this.clientJavaScript.Text = script.ToString();
                //hasRendered = true;
        }

        #endregion
    }
}
