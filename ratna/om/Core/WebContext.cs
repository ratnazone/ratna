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
namespace Jardalu.Ratna.Core
{
    #region using

    using System;
    using System.Web;
    
    using Jardalu.Ratna.Administration;
    using Jardalu.Ratna.Core;

    #endregion

    public class WebContext
    {
        #region private fields

        private static WebContext current = new WebContext();
        private static object syncRoot = new object();
        private static Site UnInitializedSite = new Site() { Id = 1, Title = string.Empty, Host = "default" };

        #endregion

        #region properties

        public static WebContext Current
        {
            get
            {
                return current;
            }
        }

        public static void Initialize(Site site)
        {
            if (site == null)
            {
                throw new ArgumentNullException("site");
            }

            UnInitializedSite = site;
        }

        #endregion

        #region public properties

        public Guid StickyId
        {
            get
            {
                Guid stickyId = Guid.Empty;

                if (HttpContext.Current != null &&
                    HttpContext.Current.Items.Contains(PublicConstants.StickyId))
                {
                    stickyId = (Guid)HttpContext.Current.Items[PublicConstants.StickyId];
                }

                return stickyId;
            }
        }

        public Site Site
        {
            get
            {
                //default
                Site s = UnInitializedSite;

                if (HttpContext.Current != null &&
                    HttpContext.Current.Items.Contains(PublicConstants.Site))
                {
                    s = (Site)HttpContext.Current.Items[PublicConstants.Site];
                }

                return s;
            }
        }

        public ResponseContent ResponseContent
        {
            get
            {
                ResponseContent content = null;

                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Items.Contains(PublicConstants.ResponseContext))
                    {
                        content = (ResponseContent)HttpContext.Current.Items[PublicConstants.ResponseContext];
                    }
                    else
                    {
                        lock(syncRoot)
                        {
                            if (HttpContext.Current.Items.Contains(PublicConstants.ResponseContext))
                            {
                                content = (ResponseContent)HttpContext.Current.Items[PublicConstants.ResponseContext];
                            }
                            else
                            {
                                content = new ResponseContent();
                                HttpContext.Current.Items[PublicConstants.ResponseContext] = content;
                            }
                        }
                    }
                }

                return content;
            }
        }

        #endregion

    }
}
