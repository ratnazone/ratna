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

namespace Jardalu.Ratna.Web.Service
{
    #region using

    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.UI;

    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.Security;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.UI.Snippets;

    #endregion

    public class ServiceBase : System.Web.Services.WebService
    {

        #region private fields

        private static Logger logger;

        #endregion

        #region ctor

        static ServiceBase()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region protected methods

        protected Ratna.Profile.User ValidatedUser()
        {
            Ratna.Profile.User user = AuthenticationUtility.Instance.GetLoggedUser();

            if (user != null)
            {
                SetContext(user);
            }

            return user;
        }

        /// <summary>
        /// checks for access attribute defined on the class
        /// and method. 
        /// 
        /// to get an access to the method, user must have access
        /// to both the conditions defined at class level and method level.
        /// </summary>
        /// <param name="user">user for which access is checked</param>
        /// <returns>false if the user does not have access</returns>
        protected bool IsAccessAllowed(Ratna.Profile.User user)
        {

            if (user == null)
            {
                return false;
            }

            bool access = false;
            AccessAttribute accessAttribute = null;

            object[] attributes = this.GetType().GetCustomAttributes(false);
            accessAttribute = FindAccessAttribute(attributes);
            access = HasUserAccess(user, accessAttribute);

            // if the user has access to the class level, make sure
            // user also has access at the method level.
            if (access)
            {
                attributes = MethodBase.GetCurrentMethod().GetCustomAttributes(false);
                accessAttribute = FindAccessAttribute(attributes);
                access = HasUserAccess(user, accessAttribute);
            }

            return access;
        }

        protected void SetContext(Ratna.Profile.User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            Jardalu.Ratna.Core.Context.SetCurrentContext(user);
        }

        protected string SendAccessDenied()
        {
            ServiceOutput output = new ServiceOutput();
            output.Success = false;
            output.AddOutput(Constants.Json.Message, ResourceManager.GetLiteral("Admin.Common.AccessDenied"));
            return output.GetJson();
        }

        protected void AddSnippet(ServiceOutput output, MethodBase methodBase)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            if (methodBase == null)
            {
                throw new ArgumentNullException("methodBase");
            }

            // get the action associated with the call
            SnippetAction action = SnippetManager.Instance.Parse(HttpContext.Current.Request.Params);
            if (action.IsEnabled)
            {

                bool snippetMatch = false; 

                //get the name of the snippet that needs to be loaded
                string snippetName = action.Name;

                // make sure the method can support this snippet.
                object[] attributes = methodBase.GetCustomAttributes(typeof(SupportedSnippetAttribute), false);
                foreach (object a in attributes)
                {
                    SupportedSnippetAttribute attribute = a as SupportedSnippetAttribute;
                    if (attribute != null && 
                        attribute.Name.Equals(attribute.Name, StringComparison.OrdinalIgnoreCase) )
                    {
                        snippetMatch = true;
                        break;
                    }
                }

                logger.Log(LogLevel.Debug, "SnippetName - [{0}] , Match - {1}", snippetName, snippetMatch);

                // asked snippet can be supported by the service.
                if (snippetMatch && 
                    SnippetManager.Instance.IsRegistered(snippetName))
                {
                    // load the snippet control from the location
                    Control control = FormlessPage.GetControl(SnippetManager.Instance.GetControlPath(snippetName));
                    SnippetControl snippet = control as SnippetControl;

                    if (snippet != null)
                    {
                        logger.Log(LogLevel.Debug, "Got snippet [{0}], invoking to get output.", snippetName);

                        //set the control values
                        snippet.SetProperties(action.Properties);
                        output.AddOutput(Constants.Json.Html, snippet.GetJsonHtml());
                    }
                }
            }
        }

        #endregion

        #region private methods

        private AccessAttribute FindAccessAttribute(object[] attributes)
        {
            AccessAttribute accessAttribute = null;

            if (attributes != null)
            {
                foreach (object attribute in attributes)
                {
                    AccessAttribute tempaccessAttribute = attribute as AccessAttribute;
                    if (tempaccessAttribute != null)
                    {
                        accessAttribute = tempaccessAttribute;

                        break;
                    }
                }
            }

            return accessAttribute;
        }

        private bool HasUserAccess(Ratna.Profile.User user, AccessAttribute accessAttribute)
        {
            bool access = false;

            if (accessAttribute != null)
            {
                Group g = null;
                if (GroupStore.Instance.TryGetGroup(accessAttribute.Group, out g))
                {
                    if (user.IsMemberOfGroup(g))
                    {
                        access = true;
                    }
                }
            }
            else
            {
                access = true;
            }

            return access;
        }

        #endregion

    }
}
