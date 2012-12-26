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

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Database;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Exceptions;

    #endregion

    internal class GroupStore
    {

        #region private fields

        private static GroupStore store;
        private static object syncRoot = new object();

        private Group visitor;
        private Group author;
        private Group editor;
        private Group administrator;

        #endregion

        #region ctor

        private GroupStore()
        {
            administrator = new Group() { Id = 1, Name = CoreConstants.Group.Administrator };
            visitor = new Group() { Id = 2, Name = CoreConstants.Group.Visitor };
            author = new Group() { Id = 3, Name = CoreConstants.Group.Author };
            editor = new Group() { Id = 4, Name = CoreConstants.Group.Editor };
        }

        #endregion

        #region public properties

        public static GroupStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new GroupStore();
                        }
                    }
                }

                return store;
            }
        }

        public Group Adminsitrator
        {
            get
            {
                return administrator;
            }
        }

        public Group Visitor
        {
            get
            {
                return visitor;
            }
        }

        public Group Author
        {
            get
            {
                return author;
            }
        }

        public Group Editor
        {
            get
            {
                return editor;
            }
        }

        #endregion

        #region public methods

        public bool IsGroupMember(Principal u, Group g)
        {
            #region arguments

            if (u == null)
            {
                throw new ArgumentNullException("u");
            }
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            #endregion

            return GroupDbInteractor.Instance.IsGroupMember(u.PrincipalId, g.Id);
        }

        public void AddMember(Principal u, Group g)
        {
            #region arguments

            if (u == null)
            {
                throw new ArgumentNullException("u");
            }
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            #endregion

            GroupDbInteractor.Instance.AddGroupMember(u.PrincipalId, g.Id);
        }

        public void RemoveMember(Principal u, Group g)
        {
            #region arguments

            if (u == null)
            {
                throw new ArgumentNullException("u");
            }
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            #endregion

            GroupDbInteractor.Instance.RemoveGroupMember(u.PrincipalId, g.Id);
        }

        public Group Create(Group g)
        {
            #region argument

            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (g.Id != 0)
            {
                throw new ArgumentException("group Id");
            }

            if (string.IsNullOrEmpty(g.Name))
            {
                throw new ArgumentException("group Name");
            }

            #endregion

            long id;
            long principalId;

            GroupDbInteractor.Instance.Create(g.Name, g.Description, out id, out principalId);
            g.Id = id;
            g.PrincipalId = principalId;

            return g;
        }

        public void Update(Group g)
        {
            #region argument

            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (g.Id == 0)
            {
                throw new ArgumentException("group Id");
            }

            if (string.IsNullOrEmpty(g.Name))
            {
                throw new ArgumentException("group Name");
            }

            #endregion

            GroupDbInteractor.Instance.Update(g.Id, g.Description);
        }

        public void Read(Group g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            Group r = GroupDbInteractor.Instance.Read(g.Name);
            g.Copy(r);
        }

        public Group Read(string name)
        {

            #region arguments

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            #endregion

            Group g = null;

            TryGetGroup(name, out g);

            return g;
        }

        public bool TryGetGroup(string name, out Group group)
        {
            bool success = false;

            try
            {
                group = GroupDbInteractor.Instance.Read(name);
                success = true;
            }
            catch (MessageException)
            {
                group = null;
            }

            return success;
        }

        public bool DoesGroupExists(string name)
        {
            Group g;
            return TryGetGroup(name, out g);
        }

        #endregion

    }
}
