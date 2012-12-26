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
    using System.Collections.Generic;
    using System.Web.Routing;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Navigation;
    using Jardalu.Ratna.Core.Media;
    using Jardalu.Ratna.Core.Pages;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Templates;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.UI;
    using Jardalu.Ratna.Web.Security;
    using RatnaUser = Jardalu.Ratna.Profile.User;

    #endregion

    public class MediaBasePage : SingleItemPage
    {
        #region private fields

        private BaseMedia media;
        private bool loaded;
        private object syncRoot = new object();

        private static Logger logger;

        #endregion

        #region ctor

        static MediaBasePage()
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

        protected BaseMedia Media
        {
            get
            {
                if (!loaded)
                {
                    lock (syncRoot)
                    {
                        if (!loaded)
                        {
                            media = GetMedia();
                            loaded = true;
                        }
                    }
                }

                return media;
            }
        }

        #endregion

        #region private methods

        private BaseMedia GetMedia()
        {
            BaseMedia media = null;

            

            return media;
        }

        public static Control GetPhotoControl(MasterPage masterPage)
        {
            Control control = null;

            RatnaMasterPage master = masterPage as RatnaMasterPage;
            if (master != null)
            {
                IPageStyle pageStyle = master.PageStyle;

                if (pageStyle != null)
                {
                    // check for photo control
                    string photoControl = pageStyle.PhotoControl;

                    if (!string.IsNullOrEmpty(photoControl))
                    {
                        // check the file exists
                        try
                        {
                            string controlPath = string.Format("{0}/{1}", master.AppRelativeTemplateSourceDirectory, photoControl);

                            control = master.LoadControl(master.ResolveUrl(photoControl));
                        }
                        catch (Exception exception)
                        {
                            // unable to load the control.
                            logger.Log(LogLevel.Warn, "Unable to load the control at : [{0}], error message : {1}", photoControl, exception);
                        }
                    }
                }
            }

            return control;
        }


        #endregion

        #region protected methods

        protected void AddMasterPageInformation()
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

                // navigation
                // TODO ================== GetNavigation data for media.   =====================
                INavigationTag navigationTag = Media as INavigationTag;
                if (navigationTag != null &&
                        navigationTag.Name != null)
                {
                    masterPage.SetNavigation(navigationTag.Name);
                }

                //breadcrumb
                if (masterPage.PageStyle.IsBreadcrumbSupported)
                {
                    IBreadcrumbControl breadcrumb = this.Master.FindControl(masterPage.PageStyle.BreadcrumbControlName) as IBreadcrumbControl;
                    BuildBreadcrumbData(breadcrumb, UrlKey);
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
                        BuildSideNavigationData(sidenavigation, UrlKey, masterPage.PageStyle.SideNavigationRoots);
                    }
                }

            }
        }

        public static Control GetPhotoThumbnailControl(MasterPage masterPage)
        {
            Control control = null;

            RatnaMasterPage master = masterPage as RatnaMasterPage;
            if (master != null)
            {
                IPageStyle pageStyle = master.PageStyle;

                if (pageStyle != null)
                {
                    // check for photo thumbnail control
                    string photoThumbnailControl = pageStyle.PhotoThumbnailControl;

                    if (!string.IsNullOrEmpty(photoThumbnailControl))
                    {
                        // check the file exists
                        try
                        {
                            string controlPath = string.Format("{0}/{1}", master.AppRelativeTemplateSourceDirectory, photoThumbnailControl);

                            control = master.LoadControl(master.ResolveUrl(photoThumbnailControl));
                        }
                        catch (Exception exception)
                        {
                            // unable to load the control.
                            logger.Log(LogLevel.Warn, "Unable to load the control at : [{0}], error message : {1}", photoThumbnailControl, exception);
                        }
                    }
                }
            }

            return control;
        }

        #endregion

    }
}
