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
namespace Jardalu.Ratna.Web.Admin.articles
{

    #region using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Web.Admin.pages;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public class ArticleBasePage : PagerSupportPage
    {
        #region private fields

        private bool viewPage = false;
        private string view = "article";
        private PublishingStage stage = PublishingStage.Published;
        private string urlKey;
        private bool isInitialized = false;
        private object syncRoot = new object();

        #endregion

        #region public properties

        public bool IsPageView
        {
            get
            {
                this.Initialize();
                return viewPage;
            }
        }


        public PublishingStage Stage
        {
            get
            {
                this.Initialize();
                return this.stage;
            }
        }

        public string View
        {
            get
            {
                this.Initialize();
                return this.view;
            }
        }

        public string UrlKey
        {
            get
            {
                this.Initialize();
                return this.urlKey;
            }
        }

        #endregion

        #region protected methods

        protected void Initialize()
        {
            if (!isInitialized)
            {
                lock (syncRoot)
                {
                    if (!isInitialized)
                    {
                        string style = this.Request["view"];
                        if (!string.IsNullOrEmpty(style) &&
                            style.Equals("page", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            viewPage = true;
                            view = "page";
                        }

                        string type = this.Request["stage"];
                        if (!string.IsNullOrEmpty(type))                         
                        {
                            if (type.Equals("draft", StringComparison.OrdinalIgnoreCase))
                            {
                                stage = PublishingStage.Draft;
                            }
                            else if (style.Equals("published", StringComparison.OrdinalIgnoreCase))
                            {
                                stage = PublishingStage.Published;
                            }
                        }

                        urlKey = Request.QueryString[Constants.UrlIdentifier] as string;

                        isInitialized = true;
                    }
                }
            }
        }

        protected IList<Tuple<string,string>> GetMenuItems()
        {
            List<Tuple<string, string>> menuItems = new List<Tuple<string, string>>();

            if (IsPageView)
            {
                menuItems.Add(new Tuple<string, string>(
                                    ResourceManager.GetLiteral("Admin.Pages.Page") /* title */,
                                    Constants.Urls.Pages.EditUrlWithKey + UrlKey /* href */)
                             );

                menuItems.Add(new Tuple<string, string>(
                                    ResourceManager.GetLiteral("Admin.Articles.Metadata") /* title */,
                                    Constants.Urls.Pages.MetadataUrlWithKey + UrlKey /* href */)
                            );
                //menuItems.Add(new Tuple<string, string>(
                //                    ResourceManager.GetLiteral("Admin.Articles.Media") /* title */,
                //                    Constants.Urls.Pages.MediaUrlWithKey + UrlKey /* href */)
                //            );
               menuItems.Add(new Tuple<string, string>(
                                    ResourceManager.GetLiteral("Admin.Articles.Edit.Versions") /* title */,
                                    Constants.Urls.Pages.VersionsUrlWithKey + UrlKey /* href */)
                            );
            }
            else
            {
                menuItems.Add(new Tuple<string, string>(
                                    ResourceManager.GetLiteral("Admin.Articles.Post") /* title */,
                                    Constants.Urls.Articles.EditUrlWithKey + UrlKey /* href */)
                             );
                menuItems.Add(new Tuple<string, string>(
                                    ResourceManager.GetLiteral("Admin.Articles.Metadata") /* title */,
                                    Constants.Urls.Articles.MetadataUrlWithKey + UrlKey /* href */)
                            );
                menuItems.Add(new Tuple<string, string>(
                                   ResourceManager.GetLiteral("Admin.Articles.Media") /* title */,
                                   Constants.Urls.Articles.MediaUrlWithKey + UrlKey /* href */)
                           );
                menuItems.Add(new Tuple<string, string>(
                                    ResourceManager.GetLiteral("Admin.Articles.Edit.Versions") /* title */,
                                    Constants.Urls.Articles.VersionsUrlWithKey + UrlKey /* href */)
                            );
            }


            return menuItems;
        }

        #endregion

    }

}
