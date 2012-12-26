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
namespace Jardalu.Ratna.Profile
{

    #region using
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Resource;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;

    #endregion

    public class User : IPrincipal, Principal
    {

        #region Private Fields

        private long id;
        private string alias;
        private string firstName;
        private string lastName;
        private string email;
        private string displayName;
        private string photo;
        private string description;

        private Dictionary<Group, bool> groupMemberships = new Dictionary<Group, bool>(2);

        #endregion

        #region Public Properties

        public long Id
        {
            get
            {
                return id;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("value");
                }

                // value change from 0 -> higher number allowed
                if (this.id > 0)
                {
                    throw new InvalidOperationException("id already present");
                }

                id = value;
            }
        }

        public long PrincipalId
        {
            get;
            set;
        }

        /// <summary>
        /// An unique value for user.
        /// 
        /// Allowed characters in Alias are 0-9, a-z, ., _, -
        /// </summary>
        public string Alias
        {
            get
            {
                return alias;
            }
            set
            {

                //if alias is already set, it cannot be set anymore
                if (!string.IsNullOrEmpty(this.alias))
                {
                    throw new InvalidOperationException("Alias cannot be updated");
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                if (value.Length > CoreConstants.FieldSize.UserAlias)
                {
                    throw new MessageException(ResourceManager.GetMessage(UserMessages.AliasSizeTooBig));
                }

                #region rule check

                // special check first character
                char[] chars = value.ToCharArray();


                //alias must not start with dot or underscore
                for(int i=0; i<chars.Length;i++)
                {
                    char c = chars[i];

                    if (i == 0)
                    {
                        if (!char.IsLetterOrDigit(c))
                        {
                            throw new MessageException(ResourceManager.GetMessage(UserMessages.AliasFirstCharacterRule));
                        }
                    }

                    if (!char.IsLetterOrDigit(c))
                    {
                        if (c != '.' || c != '_')
                        {
                            throw new MessageException(ResourceManager.GetMessage(UserMessages.AliasAllowedCharacters));
                        }
                    }
                }

                #endregion

                this.alias = value;

            }
        }

        public string DisplayName
        {
            get
            {
                string dn = null;

                if (string.IsNullOrEmpty(displayName))
                {
                    dn = string.Format("{0} {1}", FirstName, LastName);
                }
                else
                {
                    dn = displayName;
                }

                return dn;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.displayName = value;
                this.IsDirty = true;
            }
        }

        public string FirstName
        {
            get { return this.firstName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.firstName = value;
                this.IsDirty = true;
            }
        }

        public string LastName
        {
            get { return this.lastName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.lastName = value;
                this.IsDirty = true;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        public string Email
        {
            get { return this.email; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                if (!Utility.IsValidEmail(value))
                {
                    throw new MessageException(ResourceManager.GetMessage(UserMessages.EmailNotValid));
                }

                // users email cannot get updated
                if (!string.IsNullOrEmpty(this.email))
                {
                    throw new InvalidOperationException("Email cannot be updated");
                }

                this.email = value;
                this.IsDirty = true;
            }
        }
      
        public bool IsDeleted
        {
            get;
            set;
        }

        public bool IsActivated
        {
            get;
            set;
        }

        public DateTime CreatedTime
        {
            get;
            set;
        }

        public DateTime LastLoginTime
        {
            get;
            set;
        }

        public string LastIPAddress
        {
            get;
            set;
        }

        public string Photo
        {
            get
            {
                return this.photo;
            }
            set
            {
                this.photo = value;
                this.IsPhotoDirty = true;
            }
        }

        public IIdentity Identity 
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Private Properties

        private bool IsDirty
        {
            get;
            set;
        }

        private bool IsPhotoDirty
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            if (Id == 0)
            {
                // an user which does not exist in store cannot be updated
                throw new InvalidOperationException(" Update cannot be called on user that does not exist");
            }

            if (IsDirty)
            {
                UserStore.Instance.UpdateUser(this);
            }

            if (IsPhotoDirty)
            {
                UserStore.Instance.UpdateUserPhoto(this);
                this.IsPhotoDirty = false;
            }
        }

        public bool IsMemberOfGroup(Group g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            bool isMember = false;

            // every user is a visitor
            if (g == GroupStore.Instance.Visitor)
            {
                isMember = true;
            }
            else
            {
                if (groupMemberships.ContainsKey(g))
                {
                    isMember = groupMemberships[g];
                }
                else
                {
                    //read membership
                    isMember = GroupStore.Instance.IsGroupMember(this, g);
                    lock(groupMemberships)
                    {
                        groupMemberships[g] = isMember;
                    }
                }
            }

            return isMember;
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        public IList<Group> GetMembershipGroups(int start, int count, out int total)
        {
            return UserStore.Instance.GetMembershipGroups(this.Alias, start, count, out total);
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion

    }
}
