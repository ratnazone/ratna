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
namespace Jardalu.Ratna.Utilities
{

    #region Using

    using System;
    using System.Collections.Generic;

    using Jardalu.Ratna.Database;

    #endregion


    public static class NavigationDataUtility
    {

        /// <summary>
        /// Returns the title and the path for all the path that are possible with the urlKey 
        /// 
        /// For example, if the urlKey is "/news/sports/cricket/india_wins_worldcup"
        /// this utility may returns the list of following pair
        /// 
        /// ("News" , "/news")
        /// ("Sports" , "/news/sports")
        /// ("Cricket", "/news/sports/cricket")
        /// ("India Wins Worldcup", "/news/sports/cricket/india_wins_worldcup")
        /// 
        /// </summary>
        /// <param name="defaultUrl">The default url used by the client.</param>
        /// <param name="urlKey">The url for which the navigation data is needed</param>
        /// <returns>Pair of title and relative url</returns>
        public static IList<Tuple<string, string>> GetNavigtionData(string defaultUrl, string urlKey)
        {
            #region arguments

            if (string.IsNullOrEmpty(defaultUrl))
            {
                throw new ArgumentNullException("defaultUrl");
            }

            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            #endregion

            return UtilDbInteractor.Instance.GetNavigationData(defaultUrl, urlKey);
        }


        /// <summary>
        /// Generic case.
        /// </summary>
        /// <param name="defaultUrl"></param>
        /// <param name="urlKeys"></param>
        /// <returns></returns>
        public static IList<Tuple<string, string>> GetNavigtionData(string defaultUrl, IList<string> urlKeys)
        {
            #region arguments

            if (string.IsNullOrEmpty(defaultUrl))
            {
                throw new ArgumentNullException("defaultUrl");
            }

            if (urlKeys == null)
            {
                throw new ArgumentNullException("urlKeys");
            }

            #endregion

            return UtilDbInteractor.Instance.GetNavigationData(defaultUrl, urlKeys);
        }


        /// <summary>
        /// Returns all the child urls for the urlkey.
        /// 
        /// Example, input of "/news/sports/cricket"
        /// may return the following List
        /// 
        /// ("India Wins WorldCup", "/news/sports/cricket/india_wins_worldcup")
        /// ("Tendulkar scores 100th 100", "/news/sports/cricket/tendulkar_100th_100")
        /// </summary>
        /// <param name="defaultUrl">The default URL used by the client</param>
        /// <param name="urlKey">UrlKey for which the child paths are to be searched</param>
        /// <returns></returns>
        public static IList<Tuple<string, string>> GetChildNavigationData(string defaultUrl, string urlKey)
        {
            #region arguments

            if (string.IsNullOrEmpty(defaultUrl))
            {
                throw new ArgumentNullException("defaultUrl");
            }

            if (string.IsNullOrEmpty(urlKey))
            {
                throw new ArgumentNullException("urlKey");
            }

            #endregion

            return UtilDbInteractor.Instance.GetChildNavigationData(defaultUrl, urlKey);
        }

    }
}
