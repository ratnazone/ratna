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
    using System.Web;
    using System.Web.Services;
    using System.Web.Script.Services;

    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Store;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Security;
    using Jardalu.Ratna.Web.Service;
    
    #endregion

    [ScriptService]
    public class ProfileService : ServiceBase
    {
        #region private fields

        private Logger logger;

        #endregion

        #region ctor

        public ProfileService()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region public methods

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Update(string displayName, string firstName, string lastName, string description)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;
            
            // set the user data
            user.DisplayName = displayName;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Description = description;

            try
            {
                UserStore.Instance.UpdateUser(user);
                output.Success = true;
            }
            catch(Exception ex)
            {
                // log the error
                MessageException me = ex as MessageException;
                if (me != null)
                {
                    output.AddOutput(Constants.Json.Error, me.Message);
                }

                logger.Log(LogLevel.Debug, "Unable to update user : {0}", ex);
            }

            return output.GetJson();
        }

        #endregion

    }
}
