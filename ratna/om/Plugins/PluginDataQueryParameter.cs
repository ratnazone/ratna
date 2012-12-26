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
namespace Jardalu.Ratna.Plugins
{
    #region using

    using System;

    #endregion

    public class PluginDataQueryParameter
    {
        #region private fields

        private string propertyName;
        private string propertyValue;

        #endregion

        #region ctor

        public PluginDataQueryParameter()
        {
        }

        public PluginDataQueryParameter(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            this.propertyName = name;
            this.propertyValue = value;
        }

        #endregion

        #region public properties

        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                this.propertyName = value;
            }
        }

        public string PropertyValue
        {
            get
            {
                return this.propertyValue;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                this.propertyValue = value;
            }
        }

        #endregion

    }

}
