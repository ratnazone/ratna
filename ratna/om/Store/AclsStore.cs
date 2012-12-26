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

    public class AclsStore
    {

        #region private fields

        private static AclsStore store;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private AclsStore()
        {
        }

        #endregion

        #region public properties

        public static AclsStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new AclsStore();
                        }
                    }
                }

                return store;
            }
        }
       
        #endregion

        #region public methods

        public void SetAcls(Resource resource, long actorId, long principalId, AclType acls)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            SetAcls(resource.ResourceId, actorId, principalId, acls);
        }

        public void SetAcls(long resourceId, long actorId, long principalId, AclType acls)
        {
            AclsDbInteractor.Instance.SetAcls(resourceId, actorId, principalId, acls);
        }

        public IDictionary<Principal, AclType> GetAcls(Resource resource, long actorId)
        {
            return AclsDbInteractor.Instance.GetAcls(resource.ResourceId, actorId);
        }

        public void DeleteAcls(long resourceId, long actorId, long principalId)
        {
            AclsDbInteractor.Instance.DeleteAcls(resourceId, actorId, principalId);
        }
        
        #endregion

    }
}
