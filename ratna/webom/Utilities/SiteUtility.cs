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
namespace Jardalu.Ratna.Web.Utilities
{

    #region using

    using System;
    using System.Web;

    using Jardalu.Ratna.Administration;
    using Jardalu.Ratna.Store;

    #endregion

    public class SiteUtility
    {
        public static bool AssignSitePropeties()
        {
            bool success = false;

            HttpRequest request = null;

            if (HttpContext.Current != null)
            {
                request = HttpContext.Current.Request;
            }

            if (request != null)
            {

                // assign the site
                Site site = null;
                if (SiteStore.Instance.TryGet(request.Url.Host, out site) ||
                    SiteStore.Instance.TryGet(Jardalu.Ratna.Administration.Site.DefaultHost, out site))
                {
                    HttpContext.Current.Items[Jardalu.Ratna.Core.PublicConstants.Site] = site;
                }

                //if unable to get the site, check if the site contains just "www." prefix
                if (site == null)
                {
                    string host = request.Url.Host;
                    if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                    {
                        // get the site without the prefix
                        host = host.Substring(4);
                        if(SiteStore.Instance.TryGet(host, out site))
                        {
                            HttpContext.Current.Items[Jardalu.Ratna.Core.PublicConstants.Site] = site;
                        }
                    }
                }

                if (site != null)
                {
                    success = true;
                }
            
            }

            return success;
        }
    }
}
