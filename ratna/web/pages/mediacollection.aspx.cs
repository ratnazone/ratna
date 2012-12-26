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

    using Jardalu.Ratna.Core.Media;
    using Jardalu.Ratna.Store;
    using Jardalu.Ratna.Utilities;
    using Jardalu.Ratna.Web.Plugins;
    using Jardalu.Ratna.Web.UI;
    
    #endregion

    public partial class mediacollection : CollectionPage
    {        
        
        #region private fields

        private static Logger logger;

        #endregion

        #region ctor

        static mediacollection()
        {
            logger = Logger.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            base.OnPageLoad();

            string urlPath = UrlPath;
            int page = PageNumber;
            bool found = false;

            if (!string.IsNullOrEmpty(urlPath))
            {
                
                // page number starts from 1 in UI
                int start = 1 + ((page - 1) * Pager.PageSize);

                // get the media for the urlPath ( paths like /gallery/world_cup )
                Gallery gallery = GalleryPlugin.Instance.Read(urlPath);
                if (gallery != null)
                {

                    this.Title = gallery.Title;
                    this.TitleHeader.Text = gallery.Title;

                    RatnaMasterPage rmp = this.Master as RatnaMasterPage;
                    if (rmp != null && !string.IsNullOrEmpty(gallery.Navigation))
                    {
                        rmp.SetNavigation(gallery.Navigation);
                    }

                    //grab the Media for urls
                    IList<BaseMedia> mediaList = MediaStore.Instance.GetMedia(gallery.Photos);
                    if (mediaList != null && mediaList.Count > 0)
                    {
                        RenderMedia(mediaList);
                        found = true;
                    }
                }
                else
                {
                    // check if this is a collection path
                    CollectionPath path = CollectionPathPlugin.Instance.Read(urlPath);
                    if (path != null && path.CollectionType == CollectionType.Photo)
                    {
                        this.Title = path.Title;
                        this.TitleHeader.Text = path.Title;

                        int pageSize = path.PageSize;
                        if (pageSize > 0)
                        {
                            Pager.PageSize = pageSize;
                        }

                        int total;
                        IList<BaseMedia> mediaList = MediaStore.Instance.Read(MediaType.Photo, start, Pager.PageSize, out total);
                        this.TotalPages = (total / this.Pager.PageSize) + (total % this.Pager.PageSize > 0 ? 1 : 0);
                        if (mediaList != null && mediaList.Count > 0)
                        {
                            RenderMedia(mediaList);
                            found = true;
                        }
                    }
                }

            }

            if (!found)
            {
                noMediaFoundDiv.Visible = true;
            }
            else
            {
                RenderPager(this.Pager);
            }
        }

        protected void RenderMedia(IList<BaseMedia> mediaList)
        {
            //
            //imageRepeater.DataSource = photos;
            //imageRepeater.DataBind();
            //

            if (!base.RenderMedia(mediaList, this.mediadiv))
            {
                this.noMediaFoundDiv.Visible = false;
            } 
        }        

        #endregion

    }
}
