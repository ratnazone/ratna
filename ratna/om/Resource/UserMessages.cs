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

    public static class UserMessages
    {
        public const string AliasSizeTooBig = "User_Alias_SizeTooBig";
        public const string AliasAllowedCharacters = "User_Alias_AllowedCharactersRule";
        public const string AliasFirstCharacterRule = "User_Alias_FirstCharacterRule";
        public const string AliasAlreadyInUse = "User_Alias_AlreadyInUse";

        public const string EmailNotValid = "User_Email_NotValid";
        public const string EmailAlreadyInUse = "User_Email_AlreadyInUse";

        public const string PasswordMismatch = "User_Password_Mismatch";
        public const string ActivationCodeMismatch = "User_Activation_CodeInvalid";
    }
}
