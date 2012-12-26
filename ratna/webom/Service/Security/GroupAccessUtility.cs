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
namespace Jardalu.Ratna.Web.Security
{
    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;

    #endregion

    public class GroupAccessUtility
    {

        /// <summary>
        /// Cache contains the list of groups with the name
        /// 
        /// Administrator|Visitor will return list of Administrator and Visitor
        /// </summary>
        class GroupListCache
        {
            public int SiteId;
            public Dictionary<string, IList<Group>> GroupDictionary = new Dictionary<string, IList<Group>>();
        }

        #region private fields

        private static object syncRoot = new object();
        private static GroupAccessUtility instance;

        private static Dictionary<int, GroupListCache> cache = new Dictionary<int, GroupListCache>();

        #endregion

        #region public fields

        public const char Delimiter = '|';

        #endregion

        #region ctor

        private GroupAccessUtility()
        {
        }

        #endregion

        #region public properties

        public static GroupAccessUtility Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new GroupAccessUtility();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region public methods

        public bool HasAccess(User user, string groups)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            bool hasAccess =false;

            IList<Group> groupsList = GetGroups(groups);
            if (groupsList == null || groupsList.Count == 0)
            {
                hasAccess = true;
            }
            else
            {
                foreach (Group g in groupsList)
                {
                    if (g.IsMember(user))
                    {
                        hasAccess = true;
                        break;
                    }
                }
            }

            return hasAccess;
        }

        #endregion

        #region private methods

        private GroupListCache GetSiteCache()
        {
            GroupListCache groupCache = null;
            int siteId = WebContext.Current.Site.Id;

            if (!cache.ContainsKey(siteId))
            {
                lock (syncRoot)
                {
                    if (!cache.ContainsKey(siteId))
                    {
                        groupCache = new GroupListCache();
                        groupCache.SiteId = siteId;
                        cache[siteId] = groupCache;
                    }
                }
            }

            groupCache = cache[siteId];

            return groupCache;
        }

        private IList<Group> GetGroups(string groups)
        {
            GroupListCache groupCache = GetSiteCache();
            IList<Group> groupList = null;

            if (!string.IsNullOrEmpty(groups))
            {
                if (!groupCache.GroupDictionary.ContainsKey(groups))
                {
                    lock (syncRoot)
                    {
                        if (!groupCache.GroupDictionary.ContainsKey(groups))
                        {
                            List<Group> newGroupList = new List<Group>();
                            string[] groupsSplitted = groups.Split(Delimiter);
                            foreach (string groupName in groupsSplitted)
                            {
                                Group g = null;
                                if (GroupStore.Instance.TryGetGroup(groupName, out g))
                                {
                                    newGroupList.Add(g);
                                }
                            }
                            groupCache.GroupDictionary[groups] = newGroupList;
                        }
                    }
                }

                groupList = groupCache.GroupDictionary[groups];
            }

            return groupList;
        }

        #endregion

    }

}
