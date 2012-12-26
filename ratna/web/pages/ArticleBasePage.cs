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
namespace Jardalu.Ratna.Web.Pages
{
    #region using

    using System;
    using System.Web.Routing;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Core.Pages;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.Security;
    using RatnaUser = Jardalu.Ratna.Profile.User;

    using log4net;
    using Jardalu.Ratna.Templates;
    using Jardalu.Ratna.Core.Navigation;
    using System.Collections.Generic;

    #endregion

    public class ArticleBasePage : SingleItemPage
    {
        #region private fields

        private Article article;
        private bool loaded;
        private object syncRoot = new object();

        private static Logger logger;

        #endregion

        #region ctor

        static ArticleBasePage()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region protected properties

        protected string UrlKey
        {
            get
            {
                return Request[Constants.UrlIdentifier] as string;
            }
        }

        protected Article Article
        {
            get
            {
                if (!loaded)
                {
                    lock (syncRoot)
                    {
                        if (!loaded)
                        {
                            article = GetArticle();
                            loaded = true;
                        }
                    }
                }

                return article;
            }
        }

        protected PublishingStage Stage
        {
            get
            {
                PublishingStage stage = PublishingStage.Published;

                string s = Request[Constants.StageIdentifier];
                if (!string.IsNullOrEmpty(s))
                {
                    if (!Enum.TryParse<PublishingStage>(s, out stage))
                    {
                        stage = PublishingStage.Published;
                    }
                }

                return stage;
            }
        }

        protected int Version
        {
            get
            {
                int version = Constants.LatestVersion;

                string x = Request[Constants.VersionIdentifier];
                if (!Int32.TryParse(x, out version))
                {
                    version = Constants.LatestVersion;
                }

                return version;
            }
        }

        #endregion

        #region private methods

        private Article GetArticle()
        {
            Article article = null;

            if (Version == -1)
            {
                //fetch the article
                ArticleStore.Instance.TryGetArticle(UrlKey, Stage, out article);
            }
            else
            {
                ArticleStore.Instance.TryGetArticle(UrlKey, Version, out article);
            }

            //incase of draft stage or the fetch is not the latest version, 
            //make sure the user is the owner of the article.
            if (article != null && 
                (Stage != PublishingStage.Published || Version != Constants.LatestVersion))
            {
                RatnaUser user = AuthenticationUtility.Instance.GetLoggedUser();
                if (user == null ||
                    (user != null && article.Owner.PrincipalId != user.PrincipalId))
                {
                    // make the article null
                    article = null;
                }
            }

            return article;
        }

        protected string GetTagLink(Tag tag)
        {
            #region arguments

            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            #endregion

            string tagLink = "";

            DynamicPage dPage = Page as DynamicPage;
            if (dPage != null)
            {

                RouteValueDictionary rvd = new RouteValueDictionary();
                rvd.Add(Constants.PageRouteIdentifier, 1);

                if (!string.IsNullOrEmpty(tag.Name))
                {
                    rvd.Add(Constants.SearchRouteIdentifier, tag.Name);
                }

                tagLink = Page.GetRouteUrl(dPage.RouteName, rvd);
            }

            return tagLink;
        }

        public static Control GetArticleControl(MasterPage masterPage)
        {
            Control control = null;

            RatnaMasterPage master = masterPage as RatnaMasterPage;
            if (master != null)
            {
                IPageStyle pageStyle = master.PageStyle;

                if (pageStyle != null)
                {
                    // check for article control
                    string articleControl = pageStyle.ArticleControl;

                    if (!string.IsNullOrEmpty(articleControl))
                    {
                        // check the file exists
                        try
                        {
                            string controlPath = string.Format("{0}/{1}", master.AppRelativeTemplateSourceDirectory, articleControl);

                            control = master.LoadControl(master.ResolveUrl(articleControl));
                        }
                        catch (Exception exception)
                        {
                            // unable to load the control.
                            logger.Log(LogLevel.Warn, "Unable to load the control at : [{0}], error message : {1}", articleControl, exception);
                        }
                    }
                }
            }

            return control;
        }        

        public static Control GetArticleSummaryControl(MasterPage masterPage)
        {
            Control control = null;

            RatnaMasterPage master = masterPage as RatnaMasterPage;
            if (master != null)
            {
                IPageStyle pageStyle = master.PageStyle;

                if (pageStyle != null)
                {
                    // check for article summary control
                    string articleSummaryControl = pageStyle.ArticleSummaryControl;

                    if (!string.IsNullOrEmpty(articleSummaryControl))
                    {
                        // check the file exists
                        try
                        {
                            string controlPath = string.Format("{0}/{1}", master.AppRelativeTemplateSourceDirectory, articleSummaryControl);

                            control = master.LoadControl(master.ResolveUrl(articleSummaryControl));
                        }
                        catch (Exception exception)
                        {
                            // unable to load the control.
                            logger.Log( LogLevel.Warn, "Unable to load the control at : [{0}], error message : {1}", articleSummaryControl, exception);
                        }
                    }
                }
            }

            return control;
        }

        #endregion

        #region protected methods

        protected void AddMasterPageInformation(Article article)
        {
            // set the navigation and breadcrumb data if master page supports it
            RatnaMasterPage masterPage = this.Master as RatnaMasterPage;
            if (masterPage != null)
            {
                masterPage.FetchUrl = UrlKey;
            }

            if (masterPage != null &&
                masterPage.PageStyle != null)
            {

                //navigation

                INavigationTag navigationTag = article as INavigationTag;
                if (navigationTag != null &&
                        navigationTag.Name != null)
                {
                    masterPage.SetNavigation(navigationTag.Name);
                }

                //breadcrumb
                if (masterPage.PageStyle.IsBreadcrumbSupported)
                {
                    IBreadcrumbControl breadcrumb = this.Master.FindControl(masterPage.PageStyle.BreadcrumbControlName) as IBreadcrumbControl;
                    BuildBreadcrumbData(breadcrumb, article.UrlKey);
                }

                //sidenavigation
                if (masterPage.PageStyle.IsSideNavigationSupported)
                {
                    if (masterPage.PageStyle.SideNavigationControlName != null &&
                        masterPage.PageStyle.SideNavigationRoots != null &&
                        masterPage.PageStyle.SideNavigationRoots.Count > 0)
                    {
                        // fill the data.
                        ISideNavigationControl sidenavigation = this.Master.FindControl(masterPage.PageStyle.SideNavigationControlName) as ISideNavigationControl;
                        BuildSideNavigationData(sidenavigation, article.UrlKey, masterPage.PageStyle.SideNavigationRoots);
                    }
                }

            }
        }

        #endregion

    }
}
