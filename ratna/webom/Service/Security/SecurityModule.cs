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
namespace Jardalu.Ratna.Web.Security
{

    #region using

    using System;
    using System.Web;

    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.CustomResponses;
    using Jardalu.Ratna.Web.AppData;

    #endregion

    public class SecurityModule : IHttpModule
    {
        #region private fields

        private static Logger logger;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        static SecurityModule()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region IHttpModule Members

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(SecurityModule_BeginRequest);
        }

        

        #endregion

        #region private methods

        private void SecurityModule_BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            string rawUrl = HttpContext.Current.Request.RawUrl;

            if (Jardalu.Ratna.Web.Utility.IsBlockedPath(rawUrl))
            {
                context.Response.Clear();
                context.Response.Write(GetProhibitedResponse());
                context.Response.Flush();
                context.Response.End();
            }
        }

        private static string GetProhibitedResponse()
        {
            return DataReader.ReadFileContents("errors/prohibited.xml");
        }

        #endregion

    }

}
