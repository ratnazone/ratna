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

    using Jardalu.Ratna.Administration;
    using Jardalu.Ratna.Database;

    #endregion

    public class SiteStore
    {

        #region private fields

        private static SiteStore store;
        private static object syncRoot = new object();

        private bool isDirty = true;
        private List<Site> sites = new List<Site>(1);

        #endregion

        #region ctor

        private SiteStore()
        {
        }

        #endregion

        #region public properties

        public static SiteStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new SiteStore();
                        }
                    }
                }

                return store;
            }
        }

        #endregion

        #region public methods

        public IList<Site> GetSites()
        {
            if (isDirty)
            {
                lock (syncRoot)
                {
                    if (isDirty)
                    {
                        sites.Clear();

                        IList<Tuple<int, string, string>> siteData = SiteDbInteractor.Instance.GetSites();

                        foreach (Tuple<int, string, string> tuple in siteData)
                        {
                            Site site = new Site();
                            site.Id = tuple.Item1;
                            site.Host = tuple.Item2;
                            site.Title = tuple.Item3;

                            sites.Add(site);
                        }

                        //fetch from db
                        isDirty = false;
                    }
                }
            }

            List<Site> clone = new List<Site>(sites.Count);
            clone.AddRange(sites);

            return clone;
        }

        public bool TryGet(string host, out Site site)
        {
            #region argument

            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException("host");
            }

            #endregion

            site = null;

            bool success = false;

            IList<Site> sites = GetSites();
            foreach (Site s in sites)
            {
                if (s.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
                {
                    site = s;
                    success = true;
                    break;
                }
            }

            return success;
        }

        public Site AddSite(string host, string title)
        {
            #region argument

            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException("host");
            }

            #endregion

            isDirty = true;

            Tuple<int, string, string> tuple = SiteDbInteractor.Instance.AddSite(host, title);

            Site site = new Site() { Id = tuple.Item1, Host = tuple.Item2, Title = tuple.Item3 };
            return site;
        }

        public void ProvisionSite(Site site, string email, string alias, string password)
        {
            #region arguments

            if (site == null)
            {
                throw new ArgumentNullException("site");
            }

            if (site.Id == 0)
            {
                throw new ArgumentException("site.Id is 0");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            #endregion

            if (!site.IsProvisioned)
            { 
                // call to provision
                SiteDbInteractor.Instance.ProvisionSite(site.Id, email, alias, password);
            }
        }

        public void DeleteSite(string host)
        {
            #region argument

            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException("host");
            }

            #endregion

            IList<Site> sites = GetSites();
            Site s = new Site() { Host = host };
            if (sites.Contains(s))
            {

                SiteDbInteractor.Instance.DeleteSite(host);

                // call the actual delete
                isDirty = true;
            }
        }

        #endregion

    }
}
