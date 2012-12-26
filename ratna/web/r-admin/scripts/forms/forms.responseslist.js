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
/// this javascript code is to support responses list
///

$(document).ready(function () {

    $("#selectall").change(function () {
        OnSelectAllCheckboxChange();
    });

    $("#deleteResponsesButton").click(function () {
        OnDeleteResponsesButtonClick($(this));
    });

});

function OnSelectAllCheckboxChange() {
    var checked = $("#selectall").is(":checked");

    $("tbody").find(":checkbox").attr("checked", checked);
}

function OnDeleteResponsesButtonClick(deleteButton) {
    if (deleteButton != null) {
        // get all the checked buttons
        var selecteduids = "";

        var selectedchboxes = $("tbody").find(":checked");

        if (selectedchboxes == null || selectedchboxes.length == 0) {
            //show error to select

            Notification.Show("#commonnotification", L_SelectResponseToDelete , false /* autohide */)

            return;
        }

        // select all the checked checkboxes
        for (var i = 0; i < selectedchboxes.length; i++) {
            var chkbox = selectedchboxes[i];
            var uid = $(chkbox).parent().find("#uid").val();
            if (selecteduids == "") {
                selecteduids += uid ;
            }
            else {
                selecteduids += ',' + uid ;
            }
        }

        var formname = $("#formname").val();

        // confirm selected responses
        ConfirmDialog.confirm(
            L_DeleteFormResponsesConfirmation, function () {
                OnDeleteFormResponsesConfirmed(formname, selecteduids);
            }
        );
    }
}

function OnDeleteFormResponsesConfirmed(formname, selecteduids) {
    var data = "formname={0}&uids={1}".format(formname,selecteduids);
    PostManager.post(Constants.FormsManageService.DeleteResponsesUrl, data, OnDeleteFormResponsesCompleted, selecteduids);
}

function OnDeleteFormResponsesCompleted(success, data, selecteduids) {
    var response = jQuery.parseJSON(data);
    if (response.success) {

        if (selecteduids != null) {
            var uids = selecteduids.split(",");

            for (var i = 0; i < uids.length; i++) {
                var uidInputField = $('input[value="' + uids[i] + '"]');

                if (uidInputField != null) {
                    //get the parent's parent
                    var tr = uidInputField.parent().parent();

                    // remove the tr
                    tr.remove();
                }           
            }

        }

        ShowDeleteFormStatus(L_DeleteFormResponsesSuccess, true /* autohide */);
    }
    else {
        ShowDeleteFormStatus(response.error, false /* autohide */);
    }
}

function ShowDeleteFormStatus(message, autohide) {
    Notification.Show("#commonnotification", message, autohide)
}

