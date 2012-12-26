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
    using System.Runtime.Serialization;

    using Jardalu.Ratna.Core.Acls;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Database;

    #endregion

    /// <summary>
    /// Resource is a basic building block. 
    /// 
    /// For example, an article is a resource.
    /// </summary>
    [DataContract]
    public abstract class Resource
    {

        #region private fields

        private User owner;

        #endregion

        #region public properties

        /// <summary>
        /// Id that uniquely identifies a Resource.
        /// </summary>
        public long ResourceId
        {
            get;
            set;
        }

        /// <summary>
        /// Owner for the resource. Typically "system" is the owner for most
        /// of the resources, some resources may have a different owner.
        /// 
        /// for example : a comment's owner is the user that added the comment
        /// </summary>
        public User Owner
        {
            get { return this.owner; }
            set
            {
                this.owner = value;
            }
        }

        #endregion

        #region public methods

        public void SetAcl(Principal principal, AclType acls)
        {
            AclsStore.Instance.SetAcls(this, GetActorId(), principal.PrincipalId, acls);
        }

        public void RemoveAcl(Principal principal)
        {
            SetAcl(principal, AclType.None);
        }

        public void RemoveAcl(Principal principal, AclType aclType)
        {
            AclType permissions = AclsDbInteractor.Instance.GetAcls(this.ResourceId, GetActorId(), principal.PrincipalId);
            if (permissions != AclType.None)
            {
                AclType acls = permissions & ~aclType;
                SetAcl(principal, acls);
            }
        }

        public bool HasPermission(Principal principal, AclType aclType)
        {

            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            bool hasPermission = false;

            // if the principal and owner are the same, there is no need to
            // go to the DB

            if (principal.PrincipalId == this.Owner.PrincipalId)
            {
                hasPermission = true;
            }
            else
            {
                AclType permissions = AclsDbInteractor.Instance.GetAcls(this.ResourceId, GetActorId(), principal.PrincipalId);
                if (permissions != AclType.None)
                {
                    if ((permissions | aclType) == aclType)
                    {
                        hasPermission = true;
                    }
                }
            }

            return hasPermission;
        }
        
        #endregion

        #region private methods

        private long GetActorId()
        {
            User user = UserStore.Instance.Guest;

            if (Context.Current != null)
            {
                user = Context.Current.User;
            }

            return user.Id;
        }

        #endregion

    }

}
