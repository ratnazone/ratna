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
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Services;
    using System.Web.Script.Services;

    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Exceptions;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Service;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Applications;

    #endregion

    [ScriptService]
    public class apps : ServiceBase
    {

        #region private fields

        private Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        #region public methods

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Activate(long id, bool enable)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            // get the app
            App app = AppStore.Instance.GetApp(id);
            if (app != null)
            {
                try
                {

                    // for abstract apps, its file must be valid dll and the
                    // entry point must be valid entry point
                    if (app.IsAbstractApp())
                    {
                        // will throw exception when the app is not valid.
                        AppEngine.ValidateDefinedApp(app);
                    }

                    logger.Log(LogLevel.Info, "Attempting to activate app - {0}.", app.Id);
                    app.Activate(enable);
                    output.Success = true;
                }
                catch (MessageException me)
                {
                    logger.Log(LogLevel.Error, "Unable to activate app - {0}. MessageException - {1}", app.Id, me);
                    output.AddOutput(Constants.Json.Error, me.Message);
                }
            }

            return output.GetJson();
        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string SaveProperties(long id, string properties)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            logger.Log(LogLevel.Debug, "SaveProperties called for id - {0}.", id);

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            // no point in trying to save if there are no properties.
            if (!string.IsNullOrEmpty(properties))
            {
                try
                {
                    // get the app
                    App app = AppStore.Instance.GetApp(id);
                    if (app != null)
                    {
                        logger.Log(LogLevel.Info, "Attempting to save propeties for app - {0}.", app.Id);

                        // split the fields
                        string[] tokens = properties.Split(',');
                        foreach (string field in tokens)
                        {
                            if (string.IsNullOrEmpty(field))
                            {
                                continue;
                            }

                            // get the property value
                            string propertyValue = HttpContext.Current.Request[field];
                            app.SetFieldValue(field, propertyValue);
                        }

                        app.Update();

                        // success
                        output.Success = true;
                    }
                }
                catch (MessageException me)
                {
                    logger.Log(LogLevel.Error, "Unable to save properties app - {0}. MessageException - {1}", id, me);
                    output.AddOutput(Constants.Json.Error, me.Message);

                    output.Success = false;
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Delete(long id)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            // get the app
            App app = AppStore.Instance.GetApp(id);
            if (app != null)
            {
                try
                {
                    AppInstaller.Uninstall(app);
                    output.Success = true;
                }
                catch (MessageException me)
                {
                    logger.Log(LogLevel.Error, "Unable to delete app - {0}. MessageException - {1}", app.Id, me);
                    output.AddOutput(Constants.Json.Error, me.Message);
                }
            }

            return output.GetJson();
        }

        #endregion

    }
}
