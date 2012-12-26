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
namespace Jardalu.Ratna.Store
{

    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Database;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Core.Acls;
    using Jardalu.Ratna.Profile;

    #endregion

    public class UrlRedirectionStore
    {

        #region private fields

        private static UrlRedirectionStore store;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private UrlRedirectionStore()
        {
        }

        #endregion

        #region public properties

        public static UrlRedirectionStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new UrlRedirectionStore();
                        }
                    }
                }

                return store;
            }
        }

        #endregion

        #region public methods

        public bool TryGet(string url, out string redirectUrl)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            return RedirectDbInteractor.Instance.GetMapping(url, out redirectUrl);
        }

        public void Add(string url, string redirectUrl)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (string.IsNullOrEmpty(redirectUrl))
            {
                throw new ArgumentNullException("redirectUrl");
            }

            RedirectDbInteractor.Instance.AddMapping(url, redirectUrl);
        }

        public void Delete(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            RedirectDbInteractor.Instance.DeleteMapping(url);
        }


        #endregion

    }
}
