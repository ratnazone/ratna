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
///
/// functions to support script for userlist
/// these are page level scripts
///

$(document).ready(function () {
    Setup();
});

function Setup() {
    $(".activateuserbtn").click(function () {
        OnActivateUserClick($(this));
    });

    $(".deleteuserbtn").click(function () {
        OnDeleteUserClick($(this));
    });
}

function OnActivateUserClick(activateButton) {
    if (activateButton != null) {
        // activate the user

        //get the parent
        var tr = activateButton.parent().parent();

        //find user alias
        var useraliasInput = tr.find("#useralias");
        var alias = useraliasInput.val();

        var data = "alias={0}".format(alias);
        PostManager.post(Constants.UsersService.ActivateUserUrl, data, OnActivateUserComplete);
    }
}

function OnActivateUserComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";


    if (!response.success) {
        message = L_UserActivationFailure;
    }
    else {
        message = L_UserActivationSuccess;
    }

    ShowActivationResult(response.success, message);
}

function ShowActivationResult(success, message) {
    Notification.Show("#activationnotification", message, success);
}

function OnDeleteUserClick(deleteButton) {
    if (deleteButton != null) {
        // activate the user

        //get the parent
        var tr = deleteButton.parent().parent();

        //find user alias
        var useraliasInput = tr.find("#useralias");
        var alias = useraliasInput.val();

        var data = "alias={0}".format(alias);
        PostManager.post(Constants.UsersService.DeleteUserUrl, data, OnDeleteUserComplete);
    }
}

function OnDeleteUserComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";


    if (!response.success) {
        message = L_UserDeletionFailure;
    }
    else {
        message = L_UserDeletionSuccess;
    }

    ShowDeletionResult(response.success, message);
}

function ShowDeletionResult(success, message) {
    Notification.Show("#deletionnotification", message, success);
}
