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
namespace Jardalu.Ratna.Core
{
    using System;

    internal static class CoreConstants
    {

        public static class FieldSize
        {
            public const int UserAlias = 16;
        }

        public static class User
        {
            public const int AllUserId = -1;

            public const string SystemAlias = "system";
            public const string GuestAlias = "guest";
            public const string AdminAlias = "admin";

            public const string SystemDisplayName = "System";
            public const string GuestDisplayName = "Guest";
            public const string AdminDisplayName = "Admin";
        }

        public static class Group
        {
            public const string Visitor = "Visitor";
            public const string Author = "Author";
            public const string Editor = "Editor";
            public const string Administrator = "Administrator";
        }
    }

    internal static class PagesUrl
    {
        //single item pages
        public const string BlogArticleSingleItem = "~/Pages/article.aspx?url={UrlKey}&stage={Stage}&version={Version}";

        //collection pages
        public const string BlogArticleCollection = "~/Pages/articlecollection.aspx?urlpath={UrlPath}&page={Page}&query={Query}&title={Title}&nav={Nav}";
        public const string PhotosCollection = "~/Pages/mediacollection.aspx?urlpath={UrlPath}&page={Page}&query={Query}&title={Title}&nav={Nav}";
    }

    public static class PublicConstants
    {
        public const string StickyId = "StickyId";
        public const string Site = "___Site";
        public const string ResponseContext = "____response_content";
    }
}
