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
namespace Jardalu.Ratna.Web.Service
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Script.Serialization;

    #endregion

    public class ServiceOutput
    {
        #region private fields

        private bool success;
        private Dictionary<string, object> outputValues = new Dictionary<string, object>();

        private const string ObjectFormatString = "\"{0}\":{1}";
        private const string StringFormatString = "\"{0}\":\"{1}\"";

        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        #endregion

        #region ctor

        public ServiceOutput()
        {
        }

        #endregion

        #region public properties

        public bool Success
        {
            get
            {
                return this.success;
            }
            set
            {
                this.success = value;
            }
        }

        #endregion

        #region public methods

        public void AddOutput(string property, object value)
        {
            #region argument checking

            if (string.IsNullOrEmpty(property))
            {
                throw new ArgumentNullException("property");
            }

            if (property == Constants.Json.Success)
            {
                // success name not allowed.
                throw new ArgumentException("property");
            }

            #endregion

            outputValues[property] = value;
        }

        public string GetJson()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append('{');
            builder.AppendFormat(ObjectFormatString, Constants.Json.Success, this.Success.ToString().ToLower());

            foreach(string key in this.outputValues.Keys)
            {
                object value = this.outputValues[key];
                builder.Append(',');
                if (value.GetType() == typeof(int))
                {
                    builder.AppendFormat(ObjectFormatString, key, value);
                }
                else if (value.GetType() == typeof(bool))
                {
                    builder.AppendFormat(ObjectFormatString, key, value.ToString().ToLower());
                }
                else if (value.GetType() == typeof(string))
                {
                    builder.AppendFormat(StringFormatString, key, value);
                }
                else
                {
                    // use serializer first.
                    string serializedValue = serializer.Serialize(value);

                    builder.AppendFormat(ObjectFormatString, key, serializedValue);
                }
            }
            builder.Append('}');

            return builder.ToString();
        }

        #endregion

        #region private methods

        private bool IsStringFormatObject(object value)
        {
            bool isStringFormat = false;

            if (value != null)
            {
                isStringFormat = true;
                Type type = value.GetType();

                if (
                        type == typeof(int) ||
                        type == typeof(bool)
                    )
                {
                    isStringFormat = false;
                }
            }

            return isStringFormat;
        }

        #endregion

    }
}
