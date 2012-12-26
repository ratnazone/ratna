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
namespace Jardalu.Ratna.Web.Admin.pages.settings
{

    #region using

    using System;
    using System.Web.UI.WebControls;
    
    using Jardalu.Ratna.Web.Admin.controls;
    using Jardalu.Ratna.Web.Plugins;
    using Jardalu.Ratna.Web.Resource;

    #endregion

    public partial class editautopath : SettingsBasePage
    {

        #region public properties

        public string Path
        {
            get
            {
                return Request["path"];
            }
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateNavigationAndBreadCrumb();
            this.menu.Selected = ResourceManager.GetLiteral("Admin.Settings.AutoPaths.Title");

            foreach (Tuple<string, string> tuple in GetMenuItems())
            {
                this.menu.AddMenu(tuple.Item1, tuple.Item2);
            }

            // check if this is a new autopath add or
            // just an edit to an existing autopath.
            if (!string.IsNullOrEmpty(Path))
            {
                this.Title = ResourceManager.GetLiteral("Admin.Settings.AutoPaths.EditPath");
                this.headerLiteral.Text = ResourceManager.GetLiteral("Admin.Settings.AutoPaths.EditPath");
                DisplayForEditPath(Path);
            }
            else
            {
                this.Title = ResourceManager.GetLiteral("Admin.Settings.AutoPaths.AddNew");
                this.headerLiteral.Text = ResourceManager.GetLiteral("Admin.Settings.AutoPaths.AddNew");
                DisplayForNewPath();
            }

        }

        #endregion

        #region private methods

        private void PopulateNavigationAndBreadCrumb()
        {
            Navigation navigation = Master.FindControl("navigation") as Navigation;
            if (navigation != null)
            {
                navigation.Selected = Constants.Navigation.Settings;
            }

            BreadCrumb breadcrumb = Master.FindControl("breadcrumb") as BreadCrumb;
            if (breadcrumb != null)
            {
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Home"), Constants.Urls.AdminUrl);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Breadcrumb.Settings"), Constants.Urls.Settings.Url);
                breadcrumb.Add(ResourceManager.GetLiteral("Admin.Settings.AutoPaths.Title"), Constants.Urls.Settings.AutoPathsUrl);

                if (string.IsNullOrEmpty(Path))
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Settings.AutoPaths.AddNew"), Constants.Urls.Settings.EditAutoPathUrl);
                }
                else
                {
                    breadcrumb.Add(ResourceManager.GetLiteral("Admin.Settings.AutoPaths.EditPath"), Constants.Urls.Settings.EditAutoPathUrlWithKey + Path);
                }

            }
        }

        private void DisplayForNewPath()
        {
            RenderPathTypes(null);
        }

        private void DisplayForEditPath(string path)
        {
            CollectionPath collectionPath = CollectionPathPlugin.Instance.Read(path);
            if (collectionPath != null)
            {
                this.pathtitle.Value = collectionPath.Title;
                this.pathurl.Value = collectionPath.Path;
                this.pathurl.Disabled = true;
                this.pathNavigation.Value = collectionPath.Navigation;
                this.pagesize.Value = collectionPath.PageSize.ToString();
            }

            RenderPathTypes(collectionPath);
        }

        private void RenderPathTypes(CollectionPath collectionPath)
        {
            // add the types
            pathTypesSelect.Items.Add(new ListItem(ResourceManager.GetLiteral("Admin.Articles.Post"), CollectionType.BlogArticle.ToString()));
            pathTypesSelect.Items.Add(new ListItem(ResourceManager.GetLiteral("Admin.Media.Photo"), CollectionType.Photo.ToString()));

            if (collectionPath != null)
            {
                switch (collectionPath.CollectionType)
                {
                    case CollectionType.BlogArticle :
                        pathTypesSelect.SelectedIndex = 0;
                        break;
                    case CollectionType.Photo :
                        pathTypesSelect.SelectedIndex = 1;
                        break;
                }
            }

        }

        #endregion

    }
}
