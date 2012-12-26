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
    using System.Data;
    using System.Text;

    using Jardalu.Ratna.Web.Resource;
    using Jardalu.Ratna.Core.Articles;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Plugins;
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Profile;
    using Jardalu.Ratna.Core.Acls;

    #endregion

    public partial class versions : ArticleBasePage
    {

        protected long ResourceId
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            this.menu.Selected = ResourceManager.GetLiteral("Admin.Articles.Versions");

            foreach (Tuple<string, string> tuple in GetMenuItems())
            {
                this.menu.AddMenu(tuple.Item1, tuple.Item2);
            }

            if (!string.IsNullOrEmpty(UrlKey))
            {
                this.Title = ResourceManager.GetLiteral("Admin.Articles.Versions");

                //load the article for editing
                Article article = null;
                bool success = ArticleStore.Instance.TryGetArticle(UrlKey, PublishingStage.Draft, out article);

                if (success)
                {

                    this.ResourceId = article.ResourceId;

                    //should not show error
                    this.errorDiv.Visible = false;

                    // populate the values
                    this.urlspan.InnerHtml = article.UrlKey;
                    this.ownerspan.InnerHtml = article.Owner.DisplayName;
                    this.lastModifiedSpan.InnerHtml = article.LastModifiedDate.ToString();
                    this.versionspan.InnerHtml = article.Version.ToString();
                    this.createdDateSpan.InnerHtml = article.CreatedDate.ToString();

                    // need to populate the url to the editing of the page
                    PopulateNavigationAndBreadCrumb(article);

                    // get the permissions for the article
                    IDictionary<Principal, AclType> permissionsHash = AclsStore.Instance.GetAcls(article, 0);

                    //create source for data population
                    DataTable table = new DataTable();
                    table.Columns.Add("Id");
                    table.Columns.Add("Name");
                    table.Columns.Add("Acl");

                    foreach (Principal principal in permissionsHash.Keys)
                    {
                        AclType acl = permissionsHash[principal];
                        DataRow row = table.NewRow();
                        row["Id"] = principal.PrincipalId;
                        row["Name"] = principal.Name;
                        row["Acl"] = GetAclString(acl);
                        table.Rows.Add(row);
                    }

                    //bind the data
                    repeater.DataSource = table;
                    repeater.DataBind();

                    // get the versions for the article.
                    this.articleVersionsList.UrlKey = this.UrlKey;
                    this.articleVersionsList.VersionsTable = ArticleStore.Instance.GetVersions(UrlKey);

                }
                else
                {
                    //no article found with the url key
                    this.content.Visible = false;
                    this.errorDiv.Visible = true;
                }

            }
            else
            {
                this.content.Visible = false;
                this.errorDiv.Visible = true;
            }
        }

        #region private methods

        private void PopulateNavigationAndBreadCrumb(Article article)
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                if (IsPageView)
                {
                    navigation.Selected = Constants.Navigation.Pages;
                }
                else
                {
                    navigation.Selected = Constants.Navigation.Articles;
                }
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Common.Home"), Constants.Urls.AdminUrl);
                
                if (IsPageView)
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Pages"), Constants.Urls.Pages.Url);
                    breadcrumb.Add(article.Title, Constants.Urls.Pages.EditUrlWithKey + article.UrlKey);
                }
                else
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Articles"), Constants.Urls.Articles.Url);
                    breadcrumb.Add(article.Title, Constants.Urls.Articles.EditUrlWithKey + article.UrlKey);
                }
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Articles.Versions"), Constants.Urls.Articles.VersionsUrlWithKey + article.UrlKey);
            }
        }

        private string GetAclString(AclType acls)
        {

            StringBuilder builder = new StringBuilder();

            if ((acls & AclType.Read) == AclType.Read)
            {
                builder.Append(ResourceManager.GetLiteral("Admin.Articles.Options.Permissions.Acls.Read"));
            }

            if ((acls & AclType.Write) == AclType.Write)
            {
                if (builder.Length != 0)
                {
                    builder.Append(", ");
                }
                builder.Append(ResourceManager.GetLiteral("Admin.Articles.Options.Permissions.Acls.Write"));
            }

            if ((acls & AclType.Delete) == AclType.Delete)
            {
                if (builder.Length != 0)
                {
                    builder.Append(", ");
                }
                builder.Append(ResourceManager.GetLiteral("Admin.Articles.Options.Permissions.Acls.Delete"));
            }

            if ((acls & AclType.Grant) == AclType.Grant)
            {
                if (builder.Length != 0)
                {
                    builder.Append(", ");
                }
                builder.Append(ResourceManager.GetLiteral("Admin.Articles.Options.Permissions.Acls.Grant"));
            }

            return builder.ToString();
        }

        #endregion
    }

}
