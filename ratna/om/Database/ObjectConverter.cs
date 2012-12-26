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
namespace Jardalu.Ratna.Database
{

    #region using

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Reflection;
    using System.Threading;

    #endregion

    /// <summary>
    /// Original Code Credit - http://galratner.com/blogs/net/archive/2009/11/08/move-a-datareader-to-an-object-with-reflection-revisited.aspx
    /// </summary>
    internal class ObjectConverter
    {
        #region Private Fields

        private static IDictionary<string, PropertyInfo[]> propertiesCache = new Dictionary<string, PropertyInfo[]>();
        private static ReaderWriterLockSlim propertiesCacheLock = new ReaderWriterLockSlim();

        #endregion

        #region Private Methods

        private static PropertyInfo[] GetCachedProperties(Type type)
        {
            PropertyInfo[] props = new PropertyInfo[0];

            if (propertiesCacheLock.TryEnterUpgradeableReadLock(100))
            {
                try
                {
                    if (!propertiesCache.TryGetValue(type.FullName, out props))
                    {
                        props = type.GetProperties();

                        if (propertiesCacheLock.TryEnterWriteLock(100))
                        {
                            try
                            {
                                propertiesCache.Add(type.FullName, props);
                            }
                            finally
                            {
                                propertiesCacheLock.ExitWriteLock();
                            }
                        }
                    }
                }
                finally
                {
                    propertiesCacheLock.ExitUpgradeableReadLock();
                }

                return props;
            }
            else
            {
                return type.GetProperties();
            }
        }

        private static PropertyInfo[] GetCachedProperties<T>()
        {
            return GetCachedProperties(typeof(T));
        }

        private static List<string> GetColumnList(SqlDataReader reader)
        {
            List<string> columnList = new List<string>();

            System.Data.DataTable readerSchema = reader.GetSchemaTable();

            for (int i = 0; i < readerSchema.Rows.Count; i++)
            {
                columnList.Add(readerSchema.Rows[i]["ColumnName"].ToString());
            }

            return columnList;

        }

        #endregion

        public static T GetAs<T>(SqlDataReader reader)
        {

            T newObjectToReturn = Activator.CreateInstance<T>();
            PropertyInfo[] props = GetCachedProperties<T>();
            List<string> columnList = GetColumnList(reader);

            for (int i = 0; i < props.Length; i++)
            {
                if (columnList.Contains(props[i].Name) && reader[props[i].Name] != DBNull.Value && props[i].GetSetMethod() != null)
                {
                    object value = reader[props[i].Name];

                    //auto conversion for enums
                    if (props[i].PropertyType.BaseType == typeof(Enum))
                    {
                        value = Enum.ToObject(props[i].PropertyType, value);
                    }

                    typeof(T).InvokeMember(props[i].Name, BindingFlags.SetProperty, null, newObjectToReturn, new Object[] { value });

                }
            }

            return newObjectToReturn;

        }

        public static object GetAs(Type type, SqlDataReader reader)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            object newObjectToReturn = Activator.CreateInstance(type);
            PropertyInfo[] props = GetCachedProperties(type);
            List<string> columnList = GetColumnList(reader);

            for (int i = 0; i < props.Length; i++)
            {
                if (columnList.Contains(props[i].Name) && reader[props[i].Name] != DBNull.Value && props[i].GetSetMethod() != null)
                {
                    type.InvokeMember(props[i].Name, BindingFlags.SetProperty, null, newObjectToReturn, new Object[] { reader[props[i].Name] });
                }
            }

            return newObjectToReturn;
        }


    }

}
