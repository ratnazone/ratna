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
namespace Jardalu.Ratna.Web
{

    #region using

    using System;

    using Jardalu.Ratna.Core;

    #endregion

    public static class SitePaths
    {
        #region constants
        
        public const string basePath = "sites";
        public const string content = "content";
        public const string app = "apps";
        public const string template = "templates";

        public const string ContentVirtualPath = "/r-content/";
        public const string ContentFolderName = "r-content";

        public const string TemplateVirtualPath = "/templates/";

        #endregion

        public static string Content
        {
            get
            {
                return string.Format("~/{0}/{1}/{2}", basePath, WebContext.Current.Site.Id, content);
            }
        }

        public static string App
        {
            get
            {
                return string.Format("~/{0}/{1}/{2}", basePath, WebContext.Current.Site.Id, app);
            }
        }

        public static string Template
        {
            get
            {
                return string.Format("~/{0}/{1}/{2}", basePath, WebContext.Current.Site.Id, template);
            }
        }


    }
}
