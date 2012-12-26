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

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Store;

    #endregion

    public class Group : ICrudObject, Principal
    {

        #region public properties

        public long Id
        {
            get;
            set;
        }

        public long PrincipalId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        #endregion

        #region public methods

        public void AddMember(User u)
        {
            if (u == null)
            {
                throw new ArgumentNullException("u");
            }

            GroupStore.Instance.AddMember(u, this);
        }

        public void RemoveMember(User u)
        {
            if (u == null)
            {
                throw new ArgumentNullException("u");
            }

            GroupStore.Instance.RemoveMember(u, this);
        }

        public bool IsMember(User u)
        {
            return GroupStore.Instance.IsGroupMember(u, this);
        }

        public IList<User> GetMembers()
        {
            throw new NotImplementedException();
        }

        public IList<User> GetMembers(int start, int count, out int total)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            bool equals = false;

            Group g = obj as Group;
            if (g != null)
            {
                equals = (g.Id == this.Id);
            }

            return equals;
        }

        public override int GetHashCode()
        {
            return (int)this.Id;
        }

        public void Copy(Group g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            this.Id = g.Id;
            this.Name = g.Name;
            this.Description = g.Description;
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region ICrudObject implementation

        public void Read()
        {
            GroupStore.Instance.Read(this);
        }

        public void Update()
        {
            GroupStore.Instance.Update(this);
        }

        public void Create()
        {
            GroupStore.Instance.Create(this);
        }

        public bool Exists()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
