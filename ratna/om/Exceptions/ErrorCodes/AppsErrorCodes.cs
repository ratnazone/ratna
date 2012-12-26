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
namespace Jardalu.Ratna.Exceptions.ErrorCodes
{

    #region using

    using System;

    #endregion

    public static class AppsErrorCodes
    {
        public const long PublisherNameMismatch = 1001;
        public const long NameCannotBeUpdated = 1002;

        public const long PropertyNotSupplied = 1003;
        public const long PropertyValueDoesnotMatchWithFieldType = 1004;
        public const long PropertyNotFound = 1005;
        public const long RequiredFieldValueMissing = 1006;
    }
}
