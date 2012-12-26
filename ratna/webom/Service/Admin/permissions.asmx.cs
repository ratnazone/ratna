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

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Exceptions;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Service;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Core.Acls;

    #endregion

    [ScriptService]
    public class PermissionsService : ServiceBase
    {

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string SearchUserOrGroup(string query)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(query))
            {
                //find user or group
                Principal principal = PrincipalStore.Instance.Find(query);
                if (principal != null)
                {
                    bool isGroup = false;

                    if (principal.GetType() == typeof(Group))
                    {
                        isGroup = true;
                    }

                    // add name, principalid, group properties
                    output.AddOutput("name", principal.Name);
                    output.AddOutput("principalId", principal.PrincipalId);
                    output.AddOutput("isGroup", isGroup);
                    output.Success = true;
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string SetAcls(long resourceId, long principalId, int acls)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            AclType aclType = (AclType)acls;

            //set acls
            try
            {
                AclsStore.Instance.SetAcls(resourceId, user.PrincipalId, principalId, aclType);
                output.Success = true;
            }
            catch (MessageException me)
            {
                output.AddOutput(Constants.Json.Error, me.Message);
            }


            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string DeleteAcls(long resourceId, long principalId)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            //set acls
            try
            {
                AclsStore.Instance.DeleteAcls(resourceId, user.PrincipalId, principalId);
                output.AddOutput("principalId", principalId);
                output.Success = true;
            }
            catch (MessageException me)
            {
                output.AddOutput(Constants.Json.Error, me.Message);
            }


            return output.GetJson();
        }
    }
}
