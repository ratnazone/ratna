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
namespace Jardalu.Ratna.Web.Controls.Pagers
{
    #region using

    using System;
    using System.Web.Routing;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using Jardalu.Ratna.Web.UI;

    #endregion

    public partial class SimplePager : System.Web.UI.UserControl
    {

        #region private fields

        private const int MinGap = 3;

        private int currentPageNumber = 1;
        private bool autoHide = true;

        private const string PageLiteral = "Page ";

        private const string DefaultCssClass = "pagerLinks";
        private const string DefaultSelectedCssClass = "current";

        private string cssClass = DefaultCssClass;
        private string selectedCssClass = DefaultSelectedCssClass;

        #endregion

        #region public properties

        public string PageLinkFormat
        {
            get;
            set;
        }

        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }

        public string SelectedCssClass
        {
            get { return this.selectedCssClass; }
            set { this.selectedCssClass = value; }
        }

        public int TotalPages
        {
            get;
            set;
        }

        public int NumberOfPages
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public bool AutoHide
        {
            get { return autoHide; }
            set { autoHide = value; }
        }

        public int CurrentPageNumber
        {
            get
            {
                return currentPageNumber;
            }
            set
            {
                currentPageNumber = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

            SetCssClass(this.firstPageAnchor);
            SetCssClass(this.lastPageAnchor);

            this.GenerateLinks();


        }


        private void SetCssClass(HtmlControl control)
        {
            SetCssClass(control, false);
        }

        private void SetCssClass(HtmlControl control, bool isSelected)
        {
            if (control == null)
            {
                return;
            }


            if (!isSelected && !string.IsNullOrEmpty(CssClass))
            {
                control.Attributes["class"] = this.CssClass;
            }

            if (isSelected && !string.IsNullOrEmpty(SelectedCssClass))
            {
                control.Attributes["class"] = this.SelectedCssClass;
            }
        }

        public void GenerateLinks()
        {
            int start, end;
            CalculateStartEnd(out start, out end);

            HtmlGenericControl div = new HtmlGenericControl();

            if (start != 1)
            {
                // add the first link
                AddLinkToDiv(1, div);

                if (start != 2)
                {
                    AddLinkToDiv(2, div);

                    HtmlGenericControl control = new HtmlGenericControl("span");
                    control.InnerText = "...";
                    div.Controls.Add(control);
                }
            }

            

            for (int i = start; i <= end; i++)
            {
                if (this.CurrentPageNumber != i)
                {
                    AddLinkToDiv(i, div);
                }
                else
                {
                    HtmlGenericControl control = new HtmlGenericControl("span");
                    control.InnerText = i.ToString();
                    SetCssClass(control, true);
                    div.Controls.Add(control);
                }
            }

            // extend the end
            if (end != this.TotalPages)
            {

                if (end != this.TotalPages - 1)
                {
                    HtmlGenericControl control = new HtmlGenericControl("span");
                    control.InnerText = "...";
                    div.Controls.Add(control);

                    // add the second one also.
                    AddLinkToDiv(this.TotalPages - 1, div);
                }

                AddLinkToDiv(this.TotalPages, div);
            }

            this.linksdiv.Controls.Add(div);
        }

        private void AddLinkToDiv(int index, HtmlGenericControl div)
        {
            HtmlAnchor link = new HtmlAnchor();
            link.InnerHtml = index.ToString();
            link.Title = PageLiteral + index;
            link.HRef = GetPageLink(index);
            SetCssClass(link);
            div.Controls.Add(link);
        }

        private string GetPageLink(int page)
        {
            string pageLink = "";

            // check if PageLinkFormat is set
            if (!string.IsNullOrEmpty(PageLinkFormat))
            {
                pageLink = string.Format(PageLinkFormat, page);
            }
            else
            {
                DynamicPage dPage = Page as DynamicPage;
                if (dPage != null)
                {

                    RouteValueDictionary rvd = new RouteValueDictionary();
                    rvd.Add(Constants.PageRouteIdentifier, page);

                    if (!string.IsNullOrEmpty(dPage.Query))
                    {
                        rvd.Add(Constants.SearchRouteIdentifier, dPage.Query);
                    }

                    pageLink = Page.GetRouteUrl(dPage.RouteName, rvd);
                }              
            }

            return pageLink;
        }

        private void CalculateStartEnd(out int start, out int end)
        {
            start = this.CurrentPageNumber;
            end = start + this.NumberOfPages - 1;

            if (end > this.TotalPages)
            {
                end = this.TotalPages;
            }

            //get the total count of displayed
            int totalPagesDisplay = end - start + 1;

            while ((this.TotalPages > totalPagesDisplay) &&
                (totalPagesDisplay < this.NumberOfPages))
            {
                bool either = false;

                if (start > 1)
                {
                    start--;
                    either = true;
                }

                totalPagesDisplay = end - start + 1;

                if (totalPagesDisplay < this.NumberOfPages)
                {
                    if (end < this.TotalPages)
                    {
                        end++;
                        either = true;
                    }
                }

                if (!either)
                {
                    break;
                }

                totalPagesDisplay = end - start + 1;
            }


            // readjust start and end.
            if (start != 1)
            {
                int gap = start - 1;
                if (gap < MinGap)
                {
                    start = 1;
                }
            }

            if (end != this.TotalPages)
            {
                int gap = this.TotalPages - end;
                if (gap < MinGap)
                {
                    end = this.TotalPages;
                }
            }

        }

    }

}
