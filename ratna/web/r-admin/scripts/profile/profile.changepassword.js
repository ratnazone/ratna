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
// used for change password page

$.validator.setDefaults({
    submitHandler: function (form) {
        var success = OnSaveClick();
        if (success) {
            form.submit();
        }

        return success;
    }
});

$(document).ready(function () {
    $("#changepasswordform").validate();
});


function OnSaveClick() {
    var success = false;

    try {
        var oldpassword = $("#oldPassword").val();
        var newpassword = $("#newPassword").val();
        var retypenewpassword = $("#retypeNewPassword").val();

        var oldpasswordBytes = UTF8.encode(oldpassword);
        var newpasswordBytes = UTF8.encode(newpassword);

        //create the sha256 bytes
        var oldpasswordHash = Crypto.SHA256(oldpasswordBytes, { asString: true });
        var oldhash64 = Base64.encode(oldpasswordHash);

        var newpasswordHash = Crypto.SHA256(newpasswordBytes, { asString: true });
        var newhash64 = Base64.encode(newpasswordHash);

        $("#oldpasswordhash").val(oldhash64);
        $("#newpasswordhash").val(newhash64);

        //validation has worked
        success = true;
    }
    finally {
        return success;
    }
}
