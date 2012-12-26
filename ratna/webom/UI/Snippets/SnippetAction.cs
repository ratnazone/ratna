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
namespace Jardalu.Ratna.Web.UI.Snippets
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    #endregion

    public class SnippetAction
    {

        #region private fields

        private bool isEnabled;
        private string name;
        private NameValueCollection collection = new NameValueCollection();

        #endregion

        #region ctor

        public SnippetAction()
        {
        
        }

        public SnippetAction(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;
        }

        #endregion

        #region public properties

        public string Name
        {
            get { return this.name; }
            set
            {
                if (string.IsNullOrEmpty(value) || 
                    this.name != null)
                {
                    throw new ArgumentException("value");
                }

                this.name = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
            }
        }

        public NameValueCollection Properties
        {
            get
            {
                return this.collection;
            }
        }

        #endregion

        #region public methods

        public void Add(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            collection[key] = value;
        }

        #endregion

    }
}
