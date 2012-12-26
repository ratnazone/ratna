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

    #region using

    using System;

    #endregion

    public class Field
    {
        #region private fields

        private FieldType fieldType;
        private bool isCollection;

        #endregion

        #region public properties

        public string Name { get; set; }

        public FieldType FieldType
        {
            get
            {
                return fieldType;
            }
            set
            {
                FieldType setter = value;

                if (IsCollection)
                {
                    // cannot have a fieldtype = Other and IsCollection set to true.
                    EnsureFieldTypeAndCollection(setter);
                }

                this.fieldType = setter;
            }
        }

        public bool IsRequired { get; set; }
        
        public bool IsCollection 
        {
            get
            {
                return isCollection;
            }
            set
            {
                if (value)
                {
                    EnsureFieldTypeAndCollection(this.FieldType);
                }

                isCollection = value;
            }
        }

        #endregion

        #region public methods

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool equals = false;

            Field field = obj as Field;

            if (field != null)
            {
                equals = (this.Name == field.Name);
            }

            return equals;
        }

        #endregion

        #region private methods

        private static void EnsureFieldTypeAndCollection(FieldType fType)
        {
            if (fType == FieldType.Other) 
            {
                throw new InvalidOperationException("Collection not allowed for FieldType");
            }
        }

        #endregion

    }
}
