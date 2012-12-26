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
    using System.Web;
    using System.Web.Services;
    using System.Web.Script.Services;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Exceptions;
    using RatnaUser = Jardalu.Ratna.Profile.User;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Service;
    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Web.UI.Snippets;
    using Jardalu.Ratna.Core.Navigation;

    #endregion

    [ScriptService]
    public class ArticleService : ServiceBase
    {

        #region private fields

        private Logger logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string ValidateUrlKey(string urlKey)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (!(IsAccessAllowed(user)))
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {

                //make sure the url is relative and valid.
                if (Uri.IsWellFormedUriString(urlKey, UriKind.Relative))
                {
                    output.Success = !ArticleStore.Instance.Exists(urlKey);
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Save(string urlKey, string title, string body)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {

                //make sure the url is relative and valid.
                if (!Uri.IsWellFormedUriString(urlKey, UriKind.Relative))
                {
                    // failed because URL is not valid.
                    output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Articles.Edit.Url.Validate.Error"));
                }
                else
                {
                    try
                    {
                        bool exists = ArticleStore.Instance.Exists(urlKey);

                        BlogArticle article = new BlogArticle();
                        article.UrlKey = urlKey;

                        // if the article already exists, read the article
                        // to sync with latest version
                        if (exists)
                        {
                            article.Read(PublishingStage.Draft);
                        }

                        article.Title = title;
                        article.Body = body;

                        if (!exists)
                        {
                            article.Owner = user;
                            article.Create();
                        }
                        else
                        {
                            article.Update();
                        }

                        output.Success = true;
                    }
                    catch (MessageException me)
                    {
                        logger.Log(LogLevel.Error, "Unable to save article. MessageException - {0}", me);
                        output.AddOutput(Constants.Json.Error, me.Message);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(LogLevel.Error, "Unable to save article. Exception - {0}", ex);
                        output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Articles.Edit.Create.Error"));
                    }
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string SavePage(string urlKey, string title, string body)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {
                //make sure the url is relative and valid.
                if (!Uri.IsWellFormedUriString(urlKey, UriKind.Relative))
                {
                    // failed because URL is not valid.
                    output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Articles.Edit.Url.Validate.Error"));
                }
                else
                {
                    try
                    {
                        bool exists = ArticleStore.Instance.Exists(urlKey);

                        StaticArticle article = new StaticArticle();
                        article.UrlKey = urlKey;

                        // if the article already exists, read the article
                        // to sync with latest version
                        if (exists)
                        {
                            article.Read(PublishingStage.Draft);
                        }

                        article.Title = title;
                        article.Body = body;

                        if (!exists)
                        {
                            article.Owner = user;
                            article.Create();
                        }
                        else
                        {
                            article.Update();
                        }

                        output.Success = true;
                    }
                    catch (MessageException me)
                    {
                        logger.Log(LogLevel.Error, "Unable to save page. MessageException - {0}", me);
                        output.AddOutput(Constants.Json.Error, me.Message);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(LogLevel.Error, "Unable to save page. Exception - {0}", ex);
                        output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Articles.Edit.Create.Error"));
                    }
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string SaveMetadata(string urlKey, string navigationtab, string tags, string description, string summary)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {
                try
                {
                    bool exists = ArticleStore.Instance.Exists(urlKey);

                    if (exists)
                    {

                        BlogArticle article = new BlogArticle();
                        article.UrlKey = urlKey;
                        article.Read(PublishingStage.Draft);
                        article.RemoveTags();

                        article.Description = description ?? string.Empty;
                        ((INavigationTag)article).Name = navigationtab ?? string.Empty; ;
                        
                        if (!string.IsNullOrEmpty(tags))
                        {
                            article.AddTags(tags);
                        }

                        article.Summary = summary ?? string.Empty;
                        article.Update();

                        output.Success = true;
                    }
                }
                catch (MessageException me)
                {
                    logger.Log(LogLevel.Error, "Unable to save article metadata. MessageException - {0}", me);
                    output.AddOutput(Constants.Json.Error, me.Message);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, "Unable to save article metadata. Exception - {0}", ex);
                    output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Articles.Edit.Create.Error"));
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string SavePageMetadata(string urlKey, string navigationtab, string tags, string description, string head)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {
                try
                {
                    bool exists = ArticleStore.Instance.Exists(urlKey);

                    if (exists)
                    {

                        StaticArticle article = new StaticArticle();
                        article.UrlKey = urlKey;
                        article.Read(PublishingStage.Draft);
                        article.RemoveTags();

                        ((INavigationTag)article).Name = navigationtab ?? string.Empty;;
                        
                        if (!string.IsNullOrEmpty(tags))
                        {
                            article.AddTags(tags);
                        }

                        article.Head = head ?? string.Empty;
                        article.Description = description ?? string.Empty;
                        article.Update();

                        output.Success = true;
                    }
                }
                catch (MessageException me)
                {
                    logger.Log(LogLevel.Error, "Unable to save article metadata. MessageException - {0}", me);
                    output.AddOutput(Constants.Json.Error, me.Message);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, "Unable to save article metadata. Exception - {0}", ex);
                    output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Articles.Edit.Create.Error"));
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string AddImages(string urlKey, string[] images)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {
                try
                {

                    // photos can't be added to published articles
                    Article article = ArticleStore.Instance.GetArticle(urlKey, PublishingStage.Draft);
                    if (article != null)
                    {
                        if (BlogArticleHandler.CanHandle(article))
                        {
                            BlogArticle blogArticle = new BlogArticle(article);

                            foreach (string image in images)
                            {
                                blogArticle.AddImage(image);
                            }

                            blogArticle.Update();
                            output.Success = true;
                        }
                    }
                }
                catch (MessageException me)
                {
                    output.AddOutput(Constants.Json.Error, me.Message);
                }
                catch
                {
                    output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Articles.Edit.Update.Error"));
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string RemoveImage(string urlKey, string images)
        {
            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {
                try
                {

                    // photos can't be added to published articles
                    Article article = ArticleStore.Instance.GetArticle(urlKey, PublishingStage.Draft);
                    if (article != null)
                    {

                        //multiple images can be deleted at the same time.
                        if (images != null)
                        {
                            //check if there are multiple images to be deleted.
                            string[] tokens = images.Split(',');

                            if (tokens.Length > 0 && BlogArticleHandler.CanHandle(article))
                            {
                                BlogArticle blogArticle = new BlogArticle(article);
                                foreach (string token in tokens)
                                {
                                    if (!string.IsNullOrEmpty(token))
                                    {
                                        blogArticle.RemoveImage(token);
                                    }
                                }
                                blogArticle.Update();
                                output.Success = true;
                            }
                        }
                    }
                }
                catch (MessageException me)
                {
                    output.AddOutput(Constants.Json.Error, me.Message);
                }
                catch
                {
                    output.AddOutput(Constants.Json.Error, ResourceManager.GetLiteral("Admin.Articles.Edit.Update.Error"));
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string Delete(string urlKey)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {
                try
                {
                    ArticleStore.Instance.Delete(urlKey);

                    // add the location to the output as well.
                    output.AddOutput("urlKey", urlKey);
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
        public string DeleteVersion(string urlKey, int version)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {
                try
                {
                    ArticleStore.Instance.Delete(urlKey, version);
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
        [SupportedSnippet("ArticleListRow")]
        public string Publish(string urlKey)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {
                try
                {
                    logger.Log(LogLevel.Debug, "Publishing article at url - {0}", urlKey);

                    ArticleStore.Instance.Publish(urlKey);

                    logger.Log(LogLevel.Info, "Article published at url - {0}", urlKey);

                    // get the title for the article
                    Article article = ArticleStore.Instance.GetArticle(urlKey, PublishingStage.Published);

                    logger.Log(LogLevel.Debug, "Version of the published article - {0}", article.Version);

                    // add the location to the output as well.
                    output.AddOutput("urlKey", urlKey);
                    output.AddOutput("title", article.Title);

                    // published article must move to the published column. snippet generation.
                    this.AddSnippet(output, System.Reflection.MethodBase.GetCurrentMethod());

                    output.Success = true;
                }
                catch (MessageException me)
                {
                    logger.Log(LogLevel.Error, "Unable to publish article. Exception - {0}", me);
                    output.AddOutput(Constants.Json.Error, me.Message);
                }
            }

            return output.GetJson();
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string MarkForReview(string urlKey)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {
                try
                {
                    ArticleStore.Instance.Publish(urlKey);

                    // get the title for the article
                    Article article = ArticleStore.Instance.GetArticle(urlKey, PublishingStage.InReview);

                    // add the location to the output as well.
                    output.AddOutput("urlKey", urlKey);
                    output.AddOutput("title", article.Title);
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
        public string DeleteMultiple(string urlKeys)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKeys))
            {
                try
                {
                    string[] tokens = urlKeys.Split(',');
                    List<string> urlList = new List<string>();

                    foreach (string token in tokens)
                    {
                        if (!string.IsNullOrEmpty(token))
                        {
                            urlList.Add(token);
                            logger.Log(LogLevel.Info, "Adding article at url '{0}' to get deleted", token);
                        }
                    }

                    logger.Log(LogLevel.Info, "Calling delete on multiple articles");
                    ArticleStore.Instance.Delete(urlList);
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
        public string PublishMultiple(string urlKeys)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKeys))
            {
                try
                {
                    string[] tokens = urlKeys.Split(',');
                    List<string> urlList = new List<string>();

                    foreach (string token in tokens)
                    {
                        if (!string.IsNullOrEmpty(token))
                        {
                            urlList.Add(token);
                            logger.Log(LogLevel.Info, "Adding article at url '{0}' to get published", token);
                        }
                    }

                    logger.Log(LogLevel.Info, "Calling publish on multiple articles");
                    ArticleStore.Instance.Publish(urlList);
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
        public string Revert(string urlKey, int version)
        {

            //make sure the user has access
            RatnaUser user = base.ValidatedUser();
            if (user == null)
            {
                return SendAccessDenied();
            }

            ServiceOutput output = new ServiceOutput();
            output.Success = false;

            if (!string.IsNullOrEmpty(urlKey))
            {
                try
                {
                    ArticleStore.Instance.Revert(urlKey, version);
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
