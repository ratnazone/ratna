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
/// this javascript code is to support forms list
///

$(document).ready(function () {

    // add delete to every delete button
    $(".deleteFormImageButton").click(function () {
        OnDeleteFormButtonClick($(this));
    });

});

function OnDeleteFormButtonClick(deleteButton) {
    if (deleteButton != null) {
        var tr = deleteButton.parent().parent();
        var formNameInput = tr.find("#formname")
        var formname = formNameInput.val();

        ConfirmDialog.confirm(
            L_DeleteFormConfirmation.format(formname), function () {
                OnDeleteFormConfirmed(formname);
            }
        );
    }
}

function OnDeleteFormConfirmed(formname) {
    var data = "formname={0}".formatEscape(formname);
    PostManager.post(Constants.FormsManageService.DeleteFormUrl, data, OnDeleteFormCompleted, formname);
}

function OnDeleteFormCompleted(success, data, formname) {
    var response = jQuery.parseJSON(data);
    if (response.success) {

        var formNameInputField = $('input[value="' + formname + '"]');

        //get the parent's parent
        var tr = formNameInputField.parent().parent();

        // remove the tr
        tr.remove();

        ShowDeleteFormStatus(L_DeleteFormSuccess, true /* autohide */);
    }
    else {
        ShowDeleteFormStatus(response.error, false /* autohide */);
    }
}

function ShowDeleteFormStatus(message, autohide) {
    Notification.Show("#commonnotification", message, autohide)
}

