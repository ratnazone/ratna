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
namespace Jardalu.Ratna.Web.Service
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Services;
    using System.Web.Script.Services;
    using System.Web.UI;

    using Jardalu.Ratna.Core.Apps;
    using Jardalu.Ratna.Core.Comments;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Controls;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.UI.Snippets;
    using Jardalu.Ratna.Web.Utilities;
    using Jardalu.Ratna.Web.Applications;


    #endregion

    [ScriptService]
    public class comments : ServiceBase
    {
        #region private fields

        private Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string AddComment(string key, string name, string email, string url, string body, string permalink, string threadrenderer)
        {
            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (string.IsNullOrEmpty(name) ||
                !Utility.IsValidEmail(email) ||
                string.IsNullOrEmpty(body))
            {
                // not a valid comment
                output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Comments.Invalid"));
            }
            else
            {
                // image for the thread user.
                string imageUrl = null;

                RatnaUser user = base.ValidatedUser();

                if (user != null)
                {
                    imageUrl = user.Photo;
                }

                // save the comment
                Comment comment = new Comment();
                comment.Key = key;
                comment.Id = Utility.GetUniqueString();
                comment.Body = body;
                comment.Name = name;
                comment.Url = url;
                comment.Email = email;
                comment.Image = imageUrl;
                if (!string.IsNullOrEmpty(permalink))
                {
                    comment.PermaLink = permalink;
                }

                // invoke apps before saving comment
                AppEngine.ExecuteApps(AppEvent.CommentSaving, comment);

                // check for settings to put the comment in pending list
                SiteConfiguration config = SiteConfiguration.Read();
                if (config.IsCommentModerationOn)
                {
                    comment.Approved = false;
                }

                logger.Log(LogLevel.Info, "Saving comment from : {0}, key: {1}, id: {2}", comment.Email, comment.Key, comment.Id);
                CommentsPlugin.Instance.Add(comment);
                output.Success = true;

                //notify
                Notifier.Notify(string.Format(ResourceManager.GetLiteral("Comments.NewCommentSubject"), comment.Name) /* subject */,
                                string.Format(ResourceManager.GetLiteral("Comments.NewCommentBody"), comment.Name, comment.PermaLink) /* body */);    

                // add the snippet code
                if (!string.IsNullOrEmpty(threadrenderer))
                {
                    string oneRowOutput = GetOneRowOutput(threadrenderer, imageUrl, name, DateTime.Now.ToString(), url, body);

                    if (oneRowOutput != null)
                    {
                        logger.Log(LogLevel.Debug, "Got thread control [{0}] output.", threadrenderer);
                        string jsonHtml = Jardalu.Ratna.Web.Utility.SanitizeJsonHtml(oneRowOutput);
                        output.AddOutput(Constants.Json.Html, jsonHtml);
                    }
                }

                // invoke apps after comments saved
                AppEngine.ExecuteApps(AppEvent.CommentSaved, comment);
            }

            return output.GetJson();
        }

        #region private methods

        private string GetOneRowOutput(
            string threadControlName,
            string imageurl,
            string name,
            string time,
            string url,
            string message)
        {

            string oneRowOutput = null;

            // load the snippet control from the location
            Control control = FormlessPage.GetControl(SnippetManager.Instance.GetControlPath(threadControlName));
            IThreadControl threadControl = control as IThreadControl;

            if (threadControl != null)
            {
                logger.Log(LogLevel.Debug, "Got thread control [{0}], invoking to get output.", threadControl);

                try
                {
                    // if the imageurl is empty, assign a default one
                    if (string.IsNullOrEmpty(imageurl))
                    {
                        imageurl = "/images/gravatar.jpg";
                    }

                     oneRowOutput = threadControl.GetMessageRow(imageurl, name, url, time, message);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Warn, "Thread control [{0}] threw exception - {1}", threadControlName, ex);
                }
            }

            return oneRowOutput;
        }

        #endregion

    }
}
