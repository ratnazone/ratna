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
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Utilities;

    #endregion

    public class UserStore
    {

        #region private fields

        private static UserStore store;
        private static object syncRoot = new object();

        private User system;
        private User guest;

        #endregion

        #region ctor

        private UserStore()
        {
            system = LoadUser(CoreConstants.User.SystemAlias);
            guest = LoadUser(CoreConstants.User.GuestAlias);
        }

        #endregion

        #region public properties

        public static UserStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new UserStore();
                        }
                    }
                }

                return store;
            }
        }

        public User System
        {
            get
            {
                return system;
            }
        }

        public User Guest
        {
            get
            {
                return guest;
            }
        }

        #endregion

        #region public methods

        public User LoadUser(string alias)
        {
            if (string.IsNullOrEmpty("alias"))
            {
                throw new ArgumentNullException("alias");
            }

            User u = UserDbInteractor.Instance.Read(alias);

            return u;
        }

        public User LoadUser(long userId)
        {
            return UserDbInteractor.Instance.Read(userId);
        }

        public IList<User> GetUsers(string query, bool isActive, bool isDeleted, int start, int count,out int total)
        {
            return UserDbInteractor.Instance.Find(query, isActive, isDeleted, start, count, out total);
        }

        public bool DoesUserExists(string alias)
        {
            return UserDbInteractor.Instance.DoesUserExists(alias);
        }

        public bool ActivateUser(string alias, string activationCode)
        {
            bool activated = false;

            try
            {
                UserDbInteractor.Instance.ActivateUser(alias, activationCode);
                activated = true;
            }
            catch (MessageException)
            {
            }

            return activated;
        }

        public bool ActivateUser(string alias)
        {
            bool activated = false;

            try
            {
                long actorId = 0;

                if (Context.Current != null)
                {
                    actorId = Context.Current.User.PrincipalId;

                    UserDbInteractor.Instance.ActivateUser(alias, actorId);
                    activated = true;
                }
            }
            catch (MessageException)
            {
            }

            return activated;
        }

        public bool DeleteUser(string alias)
        {
            bool deleted = false;

            try
            {
                if (Context.Current != null)
                {
                    long actorId = Context.Current.User.PrincipalId;

                    UserDbInteractor.Instance.DeleteUser(alias, actorId);
                    deleted = true;
                }
            }
            catch (MessageException)
            {
            }

            return deleted;
        }

        public User CreateUser(User user, string password)
        {

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            long id;
            long principalId;

            UserDbInteractor.Instance.Create(user.Alias, 
                                                user.Email, 
                                                PasswordUtility.GetPasswordHash(password), 
                                                user.DisplayName, 
                                                user.FirstName, 
                                                user.LastName,
                                                user.Description,
                                                out id,
                                                out principalId
                                            );
            user.Id = id;
            user.PrincipalId = principalId;

            return user;

        }

        public User UpdateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            UserDbInteractor.Instance.Update(user.Alias,
                                                user.DisplayName,
                                                user.FirstName,
                                                user.LastName,
                                                user.Description
                                            );
            return user;
        }

        public void UpdateUserPhoto(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            UserDbInteractor.Instance.Update(user.Alias,
                                               user.Photo
                                            );
        }

        public void UpdateUserPassword(string alias, string oldPassword, string newPassword, bool isHash)
        {
            if (!isHash)
            {
                oldPassword = PasswordUtility.GetPasswordHash(oldPassword);
                newPassword = PasswordUtility.GetPasswordHash(newPassword);
            }

            UserDbInteractor.Instance.ChangePassword(alias,
                    oldPassword,
                    newPassword
                );
        }

        public bool SignIn(string alias, string password, bool isHash, out string cookie, out DateTime expiry)
        {
            if (!isHash)
            {
                password = PasswordUtility.GetPasswordHash(password);
            }

            return UserDbInteractor.Instance.Signin(alias, password, out cookie, out expiry);
        }

        public void Signout(string alias)
        {
            UserDbInteractor.Instance.Signout(alias);
        }

        public bool ValidateUserCookie(string alias, string cookie)
        {
            return UserDbInteractor.Instance.IsUserCookieValid(alias, cookie);
        }

        public IList<Group> GetMembershipGroups(string alias, int start, int count, out int total)
        {
            return UserDbInteractor.Instance.GetMembershipGroups(alias, start, count, out total);
        }

        #endregion

    }
}
