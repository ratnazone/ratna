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
/// javascript code for TemplatesList.ascx
$(document).ready(function () {
    TemplateListControlSetup();
});

function TemplateListControlSetup() {
    $(".deleteTemplateImageButton").click(function () {
        OnDeleteTemplateButtonClick($(this));
    });
}

function OnDeleteTemplateButtonClick(deleteButton) {
    if (deleteButton != null) {
        var uidInput = deleteButton.prev();
        var deleteInput = deleteButton.prev().prev();
        var urlPath = deleteInput.val();
        var uid = uidInput.val();

        ConfirmDialog.confirm(
            L_DeleteTemplateConfirmation.format(urlPath), function () {
                OnDeleteTemplateConfirmed(uid);
            }
        );
    }
}

function OnDeleteTemplateConfirmed(uid) {
    var data = "uid={0}".format(uid);
    PostManager.post(Constants.TemplatesService.DeleteUrl, data, OnDeleteTemplateCompleted);
}

function OnDeleteTemplateCompleted(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        var deletedUid = response.uid;
        var hiddenInputField = $('input[value="' + deletedUid + '"]');

        //get the parent's parent
        var tr = hiddenInputField.parent().parent();

        // remove the tr
        tr.remove();

        ShowDeleteTemplateStatus(true,L_DeleteTemplateSuccess);
    }
    else {
        ShowDeleteTemplateStatus(false, response.error);
    }
}

function ShowDeleteTemplateStatus(success, message) {
    if (success) {
        Notification.Show("#notification", message, true /*autohide*/);
    }
    else {
        Notification.Show("#notification", message, false /*autohide*/);
    }
}
