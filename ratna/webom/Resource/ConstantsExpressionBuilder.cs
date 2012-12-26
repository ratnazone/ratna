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
using System;


namespace Jardalu.Ratna.Web.Resource
{
    #region using

    using System;
    using System.Web.UI;
    using System.Web.Compilation;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Reflection;

    #endregion

    public class ConstantsExpressionBuilder : ExpressionBuilder
    {

        private static Dictionary<string, object> keyValues = new Dictionary<string, object>();

        static ConstantsExpressionBuilder()
        {
            FlattenConstantsValues();
        }

        public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            CodeMethodInvokeExpression ex = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(ConstantsExpressionBuilder)),
                "GetVal", new CodePrimitiveExpression(entry.Expression.ToString().Trim()));
            return ex;

        }

        public static object GetVal(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            // try to get the key value from constants class
            object value = key;

            if (keyValues.ContainsKey(key))
            {               
                value = keyValues[key];
            }

            return value;
        }

        #region private methods

        private static void FlattenConstantsValues()
        {
            Type type = typeof(Constants);
            string prefix = string.Empty;

            AddConstantsValue(prefix, type);
        }

        private static void AddConstantsValue(string prefix, Type type)
        {
            Type[] nested = type.GetNestedTypes();

            foreach (Type nType in nested)
            {
                string nPrefix = nType.Name;
                if (!string.IsNullOrEmpty(prefix))
                {
                    nPrefix = string.Format("{0}.{1}", prefix, nType.Name);
                }

                AddConstantsValue(nPrefix, nType);
            }


            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                                                          BindingFlags.Static);

            foreach (FieldInfo field in fieldInfos)
            {
                object value = field.GetValue(null);
                string key = field.Name;

                if (!string.IsNullOrEmpty(prefix))
                {
                    key = string.Format("{0}.{1}", prefix, field.Name);
                }

                keyValues.Add(key, value);
            }

        }

        #endregion

    }
}
