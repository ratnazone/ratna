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
namespace Jardalu.Ratna.Web.Upload.Actions
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Web;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Core.Media;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Graphics;
    using Jardalu.Ratna.Web.Security;

    #endregion

    public class BlogPhotoUploadedAction : PhotoUploadedAction
    {

        #region private fields

        private Logger logger;

        #endregion

        #region ctor

        public BlogPhotoUploadedAction()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        public override void Saved(HttpRequest request, string actualPath, string vpath)
        {
            #region arguments

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            #endregion

            // save through the base
            base.Saved(request, actualPath, vpath);

            // check if success saving the image.
            if (this.Success)
            {
                // associate image to the blog.
                string urlKey = request["urlkey"];
                if (!string.IsNullOrEmpty(urlKey))
                {
                    // load the blog article.
                    Article article = null;

                    if (ArticleStore.Instance.TryGetArticle(urlKey, PublishingStage.Draft, out article))
                    {
                        logger.Log(LogLevel.Debug, "Article's handler id - '{0}'", article.HandlerId);
                        if (BlogArticleHandler.CanHandle(article))
                        {
                            BlogArticle blogArticle = new BlogArticle(article);
                            if (blogArticle != null)
                            {
                                logger.Log(LogLevel.Info, "Photo '{0}' added to the article urlkey '{1}'.", vpath, urlKey);

                                //associate the image to articles gallery
                                blogArticle.AddImage(vpath);
                                blogArticle.Update();
                            }
                        }
                    }
                }
                else
                {
                    logger.Log(LogLevel.Warn, "Blog photo was uploaded successfully, however there was no article urlkey supplied.");

                    //delete the uploaded image
                    this.Success = false;
                    this.DeleteUploaded = true;
                }
            }

        }
    }

}
