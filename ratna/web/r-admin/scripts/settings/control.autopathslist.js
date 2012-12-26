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
$(document).ready(function () {
    AutoPathsControlSetup();
});

function AutoPathsControlSetup() {
    $(".deleteAutoPathImageButton").click(function () {
        OnDeleteAutoPathButtonClick($(this));
    });
}

function OnDeleteAutoPathButtonClick(deleteButton) {
    if (deleteButton != null) {
        var deleteInput = deleteButton.prev();
        var urlPath = deleteInput.val();

        ConfirmDialog.confirm(
            L_DeleteAutoPathConfirmation.format(urlPath), function () {
                OnDeleteAutoPathConfirmed(urlPath);
            }
        );
    }
}

function OnDeleteAutoPathConfirmed(path) {
    var data = "path={0}".format(path);
    PostManager.post(Constants.AutoPathsService.DeleteUrl, data, OnDeleteAutoPathCompleted);
}

function OnDeleteAutoPathCompleted(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        var deletedPath = response.path;
        var hiddenInputField = $('input[value="' + deletedPath + '"]');

        //get the parent's parent
        var tr = hiddenInputField.parent().parent();

        // remove the tr
        tr.remove();

        ShowDeleteAutoPathStatus(true, L_DeleteAutoPathSuccess);
    }
    else {
        ShowDeleteAutoPathStatus(false, response.error);
    }
}

function ShowDeleteAutoPathStatus(success, message) {
    if (success) {
        Notification.Show("#notification", message, true /*autohide*/);
    }
    else {
        Notification.Show("#notification", message, false /*autohide*/);
    }
}
