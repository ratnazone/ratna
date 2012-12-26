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
namespace Jardalu.Ratna.Web.Admin.service
{

    #region using

    using System;
    using System.Web.Script.Services;

    using Jardalu.Ratna.Web.Service;
    using System.Web.Services;

    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Store;

    #endregion

    [ScriptService]
    public class UsersService : ServiceBase
    {
        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string ActivateUser(string alias)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(alias))
            {
                output.Success = UserStore.Instance.ActivateUser(alias);
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string DeleteUser(string alias)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(alias))
            {
                output.Success = UserStore.Instance.DeleteUser(alias);
            }

            return output.GetJson();
        }
    }
    
}
