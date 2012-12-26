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

    #endregion

    [Access(Group = "Administrator")]
    [ScriptService]
    public class ConfigurationService : ServiceBase
    {

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Update(bool isLogEnabled, string logLevel)
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
            Configuration configuration = Configuration.Instance;
            configuration.IsLoggingOn = isLogEnabled;

            //logging level
            LogLevel level = LogLevel.Info;
            if (!Enum.TryParse<LogLevel>(logLevel, out level))
            {
                level = LogLevel.Info;
            }

            configuration.LoggingLevel = level;

            try
            {
                configuration.Update();

                // apply the logging
                Jardalu.Ratna.Utilities.Logger.IsEnabled = Configuration.Instance.IsLoggingOn;
                Jardalu.Ratna.Utilities.Logger.EnabledLevel = Configuration.Instance.LoggingLevel;

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
