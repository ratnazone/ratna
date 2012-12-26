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
namespace Jardalu.Ratna.Web.UrlRewrite
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;


    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Web.Plugins;
    using Jardalu.Ratna.Utilities;

    #endregion

    public class UrlRewriterManager
    {
        #region Private Fields

        private static UrlRewriterManager instance;
        private static object syncRoot = new object();

        private Logger logger;

        #endregion

        #region ctor

        private UrlRewriterManager()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region Public Properties

        public static UrlRewriterManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new UrlRewriterManager();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Public Methods

        public bool FindRuleMatch(string originalUrl, out string rewriteUrl)
        {
            // need not worry about query string here
            // they are taken care at a layer above.
            rewriteUrl = null;
            bool match = false;

            // default view stage is always published
            PublishingStage stage = PublishingStage.Published;
            int version = Constants.LatestVersion;

            if (originalUrl != null)
            {
                Article article = null;

                // for an article to view in draft mode, the page url
                // must end with __draft
                if (originalUrl.EndsWith(Constants.DraftStageIdentifier))
                {

                    stage = PublishingStage.Draft;

                    // modify the originalUrl to accomodate for draft
                    int index = originalUrl.LastIndexOf(Constants.DraftStageIdentifier);
                    originalUrl = originalUrl.Substring(0, index);
                }

                // for an article to get the version, the page url
                // must end with __version#1
                int versionIdentifierIndex = originalUrl.IndexOf(Constants.FetchVersionIdentifier);
                if (versionIdentifierIndex != -1)
                {
                    string versionString = originalUrl.Substring(versionIdentifierIndex + Constants.FetchVersionIdentifier.Length );
                    logger.Log(LogLevel.Debug, "Versioned paged asked for url - {0}, version - {1}", originalUrl, versionString);
                    if (Int32.TryParse(versionString,out version))
                    {
                        //version was asked for, rewrite the original url.
                        int index = originalUrl.LastIndexOf(Constants.FetchVersionIdentifier);
                        originalUrl = originalUrl.Substring(0, index);
                    }
                    else
                    {
                        version = Constants.LatestVersion;
                    }
                }


                bool isDefaultPageStatic = SiteConfiguration.Read().IsDefaultPageStatic;

                // special rule for "/" location
                // if static page handler handles /default, / should be mapped to /default
                if (originalUrl == "/" &&
                    isDefaultPageStatic)
                {
                    originalUrl = Constants.DefaultPageUrl;
                }

                if (version != Constants.LatestVersion &&
                        ArticleStore.Instance.TryGetArticle(originalUrl, version, out article))
                {
                    match = true;
                }
                else
                {
                    if (ArticleStore.Instance.TryGetArticle(originalUrl, stage, out article))
                    {
                        match = true;
                    }
                }

                if (match)
                {
                    ArticleHandler handler = PluginStore.Instance.Get(article.HandlerId) as ArticleHandler;

                    int index = 0;

                    // page has syntax of {UrlKey}{Stage}{Version}
                    string preparedRule = PrepareForFormat(handler.ItemPage, "UrlKey", index++);
                    preparedRule = PrepareForFormat(preparedRule, "Stage", index++);
                    preparedRule = PrepareForFormat(preparedRule, "Version", index++);
                    rewriteUrl = string.Format(preparedRule, article.UrlKey, stage.ToString(), version);
                }

                // check if the url is a urlpath that can be served
                // as a collection page
                if (!match)
                {
                    match = FindRuleMatchCollectionPages(originalUrl, out rewriteUrl);
                }

            }

            return match;
        }

        #endregion

        #region private methods

        private static string PrepareForFormat(string rule, string propertyName, int index)
        {
            string prepared = rule;

            if (rule != null)
            {
                string original = "{" + propertyName + "}";
                prepared = rule.Replace(original, "{"+ index + "}");
            }

            return prepared;
        }

        private bool FindRuleMatchCollectionPages(string originalUrl, out string rewriteUrl)
        {

            logger.Log(LogLevel.Debug, "FindRuleMatchCollectionPages called for url - {0}", originalUrl);

            bool match = false;

            string collectionUrl = null;
            string query = null;
            int page = 1;

            // extract parameters from the original url
            ExtractCollectionParameters(originalUrl, out collectionUrl, out page, out query);

            rewriteUrl = null;

            CollectionPath collectionPath = null;
            Gallery gallery = null;

            // make sure that the URL can be resolved as a collection page
            // collection pages are defined by the user. By default, there is no collection page.
            IList<string> paths = Utility.GetUrlPaths(originalUrl);
            if (paths != null && paths.Count > 0)
            {
                foreach (string path in paths)
                {
                    collectionPath = CollectionPathPlugin.Instance.Read(path);
                    if (collectionPath != null)
                    {
                        logger.Log(LogLevel.Info, "Found collection path [{0}] for url - {1}", collectionPath.Path, originalUrl);
                        break;
                    }
                }
            }

            string pageUrlStyle = null;
            
            if (collectionPath != null)
            {
                switch (collectionPath.CollectionType)
                {
                    case CollectionType.BlogArticle:
                        pageUrlStyle = PagesUrl.BlogArticleCollection;
                        break;

                    case CollectionType.Photo:
                        pageUrlStyle = PagesUrl.PhotosCollection;
                        break;
                }

            }

            if (pageUrlStyle == null)
            {
                // check for gallery.
                gallery = GalleryPlugin.Instance.Read(collectionUrl);

                if (gallery != null)
                {
                    pageUrlStyle = PagesUrl.PhotosCollection;
                }
            }

            // collectionPageUrlStyle must be resolved by now
            if (pageUrlStyle != null)
            {
                int index = 0;

                // syntax - {UrlPath}{Page}{Query}
                string preparedRule = PrepareForFormat(pageUrlStyle, "UrlPath", index++);
                preparedRule = PrepareForFormat(preparedRule, "Page", index++);
                preparedRule = PrepareForFormat(preparedRule, "Query", index++);
                preparedRule = PrepareForFormat(preparedRule, "Title", index++);
                preparedRule = PrepareForFormat(preparedRule, "Nav", index++);

                if (collectionPath != null)
                {
                    rewriteUrl = string.Format(preparedRule, collectionUrl, page, query, collectionPath.Title, collectionPath.Navigation);
                }
                else
                {
                    rewriteUrl = string.Format(preparedRule, collectionUrl, page, query, gallery.Title, gallery.Navigation);
                }

                //match found
                match = true;

            }

            else
            {
                // no collection path was found.
                logger.Log(LogLevel.Debug, "No collection path found for URL - {0}", originalUrl);
            }

            return match;
        }

        private static void ExtractCollectionParameters(string originalUrl, out string collectionUrl, out int page, out string query)
        {

            // collectionpages/pagenumber ( example - /News/, /News/3?p=3&q=, /Gallery/World_Cup_2011?p=2&q=sachin )
            // attempts to grab pagenumber, query and url

            collectionUrl = originalUrl;
            page = 1;
            query = string.Empty;

            if (!string.IsNullOrEmpty(originalUrl))
            {
                NameValueCollection qscoll = HttpUtility.ParseQueryString(originalUrl);
                query = qscoll[Constants.QueryIdentifier];

                if (query == null)
                {
                    query = string.Empty;
                }

                if (!Int32.TryParse(qscoll[Constants.PageRouteIdentifier], out page))
                {
                    page = 1;
                }

                int index = originalUrl.IndexOf('?');
                if (index != -1)
                {
                    collectionUrl = originalUrl.Substring(0, index);
                }
            }

        }

        #endregion

    }


}
