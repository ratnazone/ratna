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
    using Jardalu.Ratna.Database;

    #endregion

    public class TagStore
    {

        #region private fields

        private static TagStore store;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private TagStore()
        {
        }

        #endregion

        #region public properties

        public static TagStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new TagStore();
                        }
                    }
                }

                return store;
            }
        }
       
        #endregion

        #region public Methods

        public void Register(Guid key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            TagDbInteractor.Instance.RegisterKey(key);
            
        }

        public void AddTag(Guid key, long resourceId, Tag tag)
        {
            #region arguments

            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            if (key == Guid.Empty)
            {
                throw new ArgumentException("key");
            }

            #endregion

            TagDbInteractor.Instance.AddTag(key, resourceId, tag.Name, tag.Weight);
        }

        public void AddTags(Guid key, long resourceId, IList<Tag> tags)
        {
            #region arguments

            if (tags == null)
            {
                throw new ArgumentNullException("tags");
            }

            if (key == Guid.Empty)
            {
                throw new ArgumentException("key");
            }

            #endregion

            TagDbInteractor.Instance.AddTags(key, resourceId, tags);
        }

        public void DeleteTag(Guid key, long resourceId, Tag tag)
        {
        }

        public IList<Tag> GetTags(Guid key, long resourceId)
        {
            #region arguments

            if (key == Guid.Empty)
            {
                throw new ArgumentException("key");
            }

            #endregion

            return TagDbInteractor.Instance.GetTags(key, resourceId);
        }
       
        #endregion

    }
}
