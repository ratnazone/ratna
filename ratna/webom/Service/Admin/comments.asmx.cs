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

    using Jardalu.Ratna.Web.Service;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Core.Comments;

    #endregion

    [ScriptService]
    public class CommentsService : ServiceBase
    {

        static CommentsService()
        {
            // ensure that the comment plugin is activated
            CommentsPlugin plugin = new CommentsPlugin();
            plugin.Register();
            plugin.Activate();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        [Access(Group = "Administrator")]
        public string Approve(string uid)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            Guid guid = Guid.Empty;
            if (Guid.TryParse(uid, out guid))
            {
                    try
                    {
                        Comment comment = CommentsPlugin.Instance.Read(guid /* uid */);
                        if (!comment.Approved)
                        {
                            comment.Approved = true;
                            CommentsPlugin.Instance.Save(comment);
                        }
                        output.AddOutput("uid", uid);
                        output.Success = true;
                    }
                    catch (MessageException me)
                    {
                        output.AddOutput(Constants.Json.Error, me.Message);
                    }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        [Access(Group = "Administrator")]
        public string Delete(string uid)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            Guid guid = Guid.Empty;
            if (Guid.TryParse(uid, out guid))
            {
                try
                {
                    CommentsPlugin.Instance.Delete(guid /*uid*/);
                    output.AddOutput("uid", uid);
                    output.Success = true;
                }
                catch (MessageException me)
                {
                    output.AddOutput(Constants.Json.Error, me.Message);
                }
            }

            return output.GetJson();
        }

    }

}
