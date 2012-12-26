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
    using Jardalu.Ratna.Templates;
    using Jardalu.Ratna.Exceptions;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Plugins;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    [Access(Group = "Administrator")]
    [ScriptService]
    public class SiteConfigurationService : ServiceBase
    {

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Update(string locale, 
            bool isCommentModerated)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            //get the configuration.
            SiteConfiguration configuration = SiteConfiguration.Read();
            if (!string.IsNullOrEmpty(locale))
            {
                configuration.Locale = locale;
            }

            configuration.IsCommentModerationOn = isCommentModerated;


            try
            {
                configuration.Update();               
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
        public string UpdateSmtp(
            string smtpAddress, 
            string smtpUserName, 
            string smtpPassword,
            string smtpFrom)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            //get the configuration.
            NotificationConfiguration configuration = NotificationConfiguration.Read();

            configuration.SmtpUserName = smtpUserName;
            if (!string.IsNullOrEmpty(smtpPassword))
            {
                configuration.SmtpPassword = smtpPassword;
            }
            configuration.SmtpAddress = smtpAddress;

            // if the from is specified, it must be email address
            if (!string.IsNullOrEmpty(smtpFrom) &&  Utility.IsValidEmail(smtpFrom))
            {
                configuration.FromAddress = smtpFrom;
            }

            try
            {
                configuration.Update();
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
        public string UpdateNotification(
                string notificationEmail,
                bool comment,
                bool formsResponse
            )
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            bool validEmail = Utility.IsValidEmail(notificationEmail);

            if (validEmail)
            {
                //get the configuration.
                NotificationConfiguration configuration = NotificationConfiguration.Read();
                configuration.NotifyToEmail = notificationEmail;
                configuration.NotifyOnComment = comment;
                configuration.NotifyOnFormResponse = formsResponse;

                try
                {
                    configuration.Update();
                    output.Success = true;
                }
                catch (MessageException me)
                {
                    output.AddOutput(Constants.Json.Error, me.Message);
                }
            }
            else
            {
                output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Errors.InvalidEmailAddress"));
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string UpdateCustomResponses(
            string error404,
            string error500,
            string otherErrors)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            //get the custom response.
            CustomResponse customResponse = CustomResponse.Read();

            customResponse.PageNotFound = error404;
            customResponse.InteralServerError = error500;
            customResponse.OtherErrors = otherErrors;

            try
            {
                customResponse.Update();
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
