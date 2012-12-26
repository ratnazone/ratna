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
namespace Jardalu.Ratna.Web.UI.Utilities
{
    #region using

    using System;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Routing;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using Jardalu.Ratna.Core;
    using Jardalu.Ratna.Core.Articles;

    #endregion

    public static class SummaryUtility
    {

        public const int MaxLength = 400;

        #region public methods

        /// <summary>
        /// Extracts summary from content.
        /// </summary>
        public static string GetSummary(string content)
        {
            string summary = content;

            if (!string.IsNullOrEmpty(content))
            {
                int indexStart = 0;
                int indexEnd = content.Length > MaxLength ? MaxLength : content.Length;

                //search div, p or span
                if (!SearchStarterEnder(content, "<div", "</div>", out indexStart, out indexEnd) ||
                    !SearchStarterEnder(content, "<p", "</p>", out indexStart, out indexEnd) ||
                    !SearchStarterEnder(content, "<span", "</span>", out indexStart, out indexEnd) )
                {
                    //reset to default
                    indexStart = 0;
                    indexEnd = content.Length > MaxLength ? MaxLength : content.Length;
                }

                summary = content.Substring(indexStart, indexEnd - indexStart);
            }

            return summary;
        }
        
        #endregion

        #region private methods

        private static bool SearchStarterEnder(string content, string starter, string ender, out int start, out int end)
        {
            bool found = false;

            start = content.IndexOf(starter, StringComparison.OrdinalIgnoreCase);
            end = content.Length;
            if (start > 0)
            {
                end = content.IndexOf(ender, start + starter.Length, StringComparison.OrdinalIgnoreCase);
                if (end < 0)
                {
                    end = content.Length;
                }
                else
                {
                    end = end + ender.Length;
                }
            }
            

            return found;
        }

        #endregion

    }

}
