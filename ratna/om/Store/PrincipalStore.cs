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
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Utilities;

    #endregion

    internal class PrincipalStore
    {

        #region private fields

        private static PrincipalStore store;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        private PrincipalStore()
        {
        }

        #endregion

        #region public properties

        public static PrincipalStore Instance
        {
            get
            {
                if (store == null)
                {
                    lock (syncRoot)
                    {
                        if (store == null)
                        {
                            store = new PrincipalStore();
                        }
                    }
                }

                return store;
            }
        }

        #endregion

        #region public Methods

        public Principal Find(string query)
        {
            // locates the best match
            return PrincipalDbInteractor.Instance.Find(query);
        }

        

        #endregion

    }
}
