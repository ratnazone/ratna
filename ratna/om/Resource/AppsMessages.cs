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
namespace Jardalu.Ratna.Resource
{

    #region using

    using System;

    #endregion

    internal class AppsMessages
    {
        public const string ManadatoryFieldsMissing = "Apps_ManadatoryFieldsMissing";
        public const string PublisherNameMismatch = "Apps_PublisherNameMismatch";
        public const string NameCannotBeUpdated = "Apps_NameCannotBeUpdated";
        public const string FieldValueNotSupplied = "Apps_FieldValueNotSupplied. Field - {0}";
        public const string FieldValueDoesnotMatchWithFieldType = "Apps_FieldValueDoesnotMatchWithFieldType. Field - {0}";
        public const string FieldNotFound = "Apps_FieldNotFound. Field - {0}";
        public const string RequiredFieldValueMissing = "Apps_RequiredFieldValueMissing. Field - {0}";
        public const string FileNameMustBeAssembly = "Apps_ScopeRequiresAssemblyFile. Scope - {0}";
    }

}
