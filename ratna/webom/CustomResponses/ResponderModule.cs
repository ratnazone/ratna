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
namespace Jardalu.Ratna.Web.CustomResponses
{

    #region using

    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Service;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.AppData;

    #endregion

    public class ResponderModule : IHttpModule
    {

        #region private fields

        private static Logger logger;
        private static object syncRoot = new object();

        #endregion

        #region ctor

        static ResponderModule()
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
            context.EndRequest += new EventHandler(ResponderModule_EndRequest);
        }

        #endregion

        #region protected methods

        protected void ResponderModule_EndRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;

            ServiceOutput output = null;
            string responseText = null;
            CustomResponse customReponse = CustomResponse.Read();
            string redirectUrl = null;

            #region non 200 response
            if (context.Response.StatusCode != (int)HttpStatusCode.OK)
            {

                logger.Log(LogLevel.Debug, "Response status code [{0}.{1}] url : {2}", 
                            context.Response.StatusCode, context.Response.SubStatusCode, context.Request.RawUrl);

                switch (context.Response.StatusCode)
                {
                    case (int)HttpStatusCode.NotFound :

                        if (context.Response.SubStatusCode == 13)
                        {
                            // upload size exceeding - content size too large.

                            output = new ServiceOutput();
                            output.Success = false;

                            output.AddOutput(
                                        Constants.Json.Error,
                                        string.Format(ResourceManager.GetLiteral("Errors.UploadMaxSizeExceeded"),
                                        Configuration.GetMaxUploadSize())
                                    );
                        }
                        else
                        {
                            // check for custom response.
                            if (!string.IsNullOrEmpty(customReponse.PageNotFound))
                            {
                                if (context.Request.RawUrl != customReponse.PageNotFound)
                                {
                                    redirectUrl = customReponse.PageNotFound;
                                }
                            }
                            else
                            {
                                // the defined 404 was not found
                                // read the default page for 404.
                                responseText = GetStandard404Response();
                            }
                        }

                        break;

                    case (int)HttpStatusCode.InternalServerError:

                        if (context.Request.RawUrl.EndsWith(".asmx"))
                        {
                            //send internal server for asmx pages.
                            output = new ServiceOutput();
                            output.Success = false;
                            output.AddOutput(
                                    Constants.Json.Error,
                                    string.Format(ResourceManager.GetLiteral("Errors.InternalServerError"),
                                    WebContext.Current.StickyId)
                                );
                        }
                        else
                        {
                            // check for redirect
                            if (!string.IsNullOrEmpty(customReponse.InteralServerError))
                            {
                                redirectUrl = customReponse.InteralServerError;
                            }
                            else
                            {
                                // display the standard err
                                responseText = GetStandardErrResponse();
                            }
                        }

                        if (HttpContext.Current.Error != null)
                        {
                            // log the output
                            logger.Log(LogLevel.Error, "Exception serving {0} - {1}", HttpContext.Current.Request.RawUrl, HttpContext.Current.Error);
                        }
                        else
                        {
                            logger.Log(LogLevel.Error, "Internal Server Error serving url [{0}]", HttpContext.Current.Request.RawUrl);
                        }

                        break;

                    default:
                        // any other error condition ?
                        if (context.Response.StatusCode >= 400 &&
                            !string.IsNullOrEmpty(customReponse.OtherErrors))
                        {
                            redirectUrl = customReponse.OtherErrors;
                        }
                        break;
                }

            }

            #endregion

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                // one of the known path for redirecting the url.
                context.Response.Clear();
                context.Response.Redirect(Jardalu.Ratna.Web.Utility.ResolveUrl(redirectUrl));
            }

            if (output != null)
            {
                // json output
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                context.Response.Write(output.GetJson());
                context.Response.Flush();
                context.Response.End();
            }
            else
            {
                if (responseText != null)
                {
                    //standard error response
                    context.Response.Clear();
                    context.Response.Write(responseText);
                    context.Response.Flush();
                    context.Response.End();
                }
            }

        }

        #endregion

        #region private methods

        private static string GetStandard404Response()
        {
            return DataReader.ReadFileContents("errors/404.xml");
        }

        private static string GetStandardErrResponse()
        {
            return DataReader.ReadFileContents("errors/err.xml");
        }

        #endregion

    }

}
