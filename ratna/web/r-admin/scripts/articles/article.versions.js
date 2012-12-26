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
/// this javascript code is to support articles' options page
///


$(document).ready(function () {
    Setup();
});

function ReloadPage() {
    location.reload(true); 
} 

function Setup() {

    $("#searchuserbutton").click(function () {
        OnSearchUsersGroupClick();
    });

    $(".deletePermissionButton").click(function () {
        OnDeletePermissionClick($(this));
    });

    $(".revertArticleVersionImageButton").click(function () {
        OnRevertArticleVersionImageClick($(this));
    });

    $(".deleteArticleVersionImageButton").click(function () {
        OnDeleteArticleVersionImageClick($(this));
    });
};

// ---------------------------------------------------------------------------------------
// Revert article versions  - start
// ---------------------------------------------------------------------------------------
function OnRevertArticleVersionImageClick(clickedElement) {
    if (clickedElement != null) {
        // get the previous element to find the principal id.
        var versionElement = clickedElement.prev();
        var version = versionElement.val();
        var urlKeyElement = versionElement.prev();
        var urlKey = urlKeyElement.val();

        ConfirmDialog.confirm(
            L_RevertArticleVersionConfirmation.format(version), function () {
                OnRevertArticleVersionConfirmed(urlKey, version);
            }
        );
    }
}

function OnRevertArticleVersionConfirmed(urlKey, version) {
    // delete the article with the version
    var data = "urlKey={0}&version={1}".formatEscape(urlKey, version);
    PostManager.post(Constants.ArticleService.RevertUrl, data, OnRevertArticleVersionComplete);
}

function OnRevertArticleVersionComplete(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        ReloadPage();
    }
    else {
        Notification.Show("#notification", response.error, false);
    }
}

// ---------------------------------------------------------------------------------------
// Revert article versions  - end
// ---------------------------------------------------------------------------------------

function OnDeleteArticleVersionImageClick(clickedElement) {
    if (clickedElement != null) {
        // get the previous element to find the principal id.
        var versionElement = clickedElement.prev();
        var version = versionElement.val();
        var urlKeyElement = versionElement.prev();
        var urlKey = urlKeyElement.val();

        var parentTR = clickedElement.parent().parent();
        var parentTRId = parentTR.attr("id");

        ConfirmDialog.confirm(
            L_DeleteArticleVersionConfirmation.format(version), function () {
                OnDeleteArticleVersionConfirmed(urlKey, version, parentTRId);
            }
        );
    }
}

function OnDeleteArticleVersionConfirmed(urlKey, version, parentTRId) {
    // delete the article with the version
    var data = "urlKey={0}&version={1}".formatEscape(urlKey, version);
    PostManager.post(Constants.ArticleService.DeleteVersionUrl, data, OnDeleteArticleVersionComplete, parentTRId);
}

function OnDeleteArticleVersionComplete(success, data, parentTRId) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        Notification.Show("#notification", L_DeleteArticleVersionSuccess, true);
        $("#" + parentTRId).remove();
    }
    else {
        Notification.Show("#notification", response.error, false);
    }
}

function OnDeletePermissionClick(clickedElement) {

    if (clickedElement != null) {

        // get the previous element to find the principal id.
        var inputElement = clickedElement.prev();
        var principalId = inputElement.val();

        // delete the permissions
        var data = "resourceId={0}&principalId={1}".formatEscape(ResourceId, principalId);
        PostManager.post(Constants.PermissionsService.DeleteAclsUrl, data, OnDeletePermissionComplete);
    }

}

function OnDeletePermissionComplete(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        var principalId = response.principalId;

        //get the TR and delete it
        var trID = "aclstr_" + principalId;
        $("#" + trID).remove();

        // remove the TR
        ShowPermissionDeleteStatus(L_PermissionsDeleted);
    }
    else {
        ShowPermissionDeleteStatus(response.error);
    }
}

function OnSearchUsersGroupClick() {
    var query = $("#searchUserGroupText").val();
    if ((query == null) || (query.trim() == "")) {
        ShowUserSearchStatus(L_EnterSearchCriteria);
    }
    else {
        // search
        var data = "query={0}".formatEscape(query);
        PostManager.post(Constants.PermissionsService.SearchUserOrGroupUrl, data, OnUsersGroupSearchComplete);
    }
}

function OnUsersGroupSearchComplete(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        ShowPrincipalPermissionsAdd(response.name, response.principalId, response.isGroup);
        ShowUserSearchStatus(L_UserOrGroupFound);
    }
    else {
        ShowUserSearchStatus(L_NoUserOrGroupFound);
    }
}

function ShowPrincipalPermissionsAdd(name, principalId, isGroup) {

    addedTRCount++;

    // generate the id for the new TR
    var newlyAddedTrId = newlyAddedTrIdPrefix + addedTRCount;

    // clone the add tr
    var addnewPermissionTr = $("#addnewpermission").clone();
    addnewPermissionTr.attr("id", newlyAddedTrId);

    //set the name
    var nameLabel = addnewPermissionTr.find("#principalName");
    if (nameLabel != null) {
        nameLabel.text(name);
    }

    var groupSpan = addnewPermissionTr.find("#principalType");
    if (groupSpan != null) {
        var typeValue = "( {0} )";
        if (isGroup) {
            typeValue = typeValue.format(L_Group);
        }
        else {
            typeValue = typeValue.format(L_User);
        }

        groupSpan.text(typeValue);
    }

    var principalIdInput = addnewPermissionTr.find("#principalId");
    if (principalIdInput != null) {

        var principalInputId = "principalId_" + addedTRCount;

        // change the id
        principalIdInput.attr("id", principalInputId);

        principalIdInput.val(principalId);
    }

    var cancelButton = addnewPermissionTr.find("#cancelSaveImage");
    if (cancelButton != null) {

        var cancelButtonId = "cancelButton_" + addedTRCount;

        // change the id
        cancelButton.attr("id", cancelButtonId);

        //now add functionality for onclick
        cancelButton.click(function () {
            OnCancelSavePermissionsClicked(addedTRCount);
        });
    }

    var saveButton = addnewPermissionTr.find("#savePermissionsImage");
    if (saveButton != null) {

        var saveButtonId = "saveButton_" + addedTRCount;

        // change the id
        saveButton.attr("id", saveButtonId);

        //now add functionality for onclick
        saveButton.click(function () {
            OnSavePermissionsClicked(addedTRCount);
        });
    }

    // set the IDs for read, write, delete, grant checkboxes
    var readChBox = addnewPermissionTr.find("#read");
    if (readChBox != null) {
        readChBox.attr("id", "read_" + addedTRCount);
    }

    var writeChBox = addnewPermissionTr.find("#write");
    if (writeChBox != null) {
        writeChBox.attr("id", "write_" + addedTRCount);
    }

    var deleteChBox = addnewPermissionTr.find("#delete");
    if (deleteChBox != null) {
        deleteChBox.attr("id", "delete_" + addedTRCount);
    }

    var grantChBox = addnewPermissionTr.find("#grant");
    if (grantChBox != null) {
        grantChBox.attr("id", "grant_" + addedTRCount);
    }

    //add it before the add-newpermission
    addnewPermissionTr.insertBefore("#addnewpermission");
}

function OnCancelSavePermissionsClicked(count) {

    //get the TR id
    var addedTrId = newlyAddedTrIdPrefix + count;

    // save has been cancelled, just remove the tr
    var newlyAddedTr = $("#" + addedTrId);
    if (newlyAddedTr != null) {
        newlyAddedTr.remove();
    }

}

function OnSavePermissionsClicked(count) {
    var acls = GetPermissionsChecked(count);

    if (acls == 0) {
        //nothing checked, just alert error
        ShowPermissionSavedStatus(L_NoPermissionsChecked);
    }
    else {

        var principalInput = $("#principalId_" + count);
        var principalId = principalInput.val();

        // save the permissions
        var data = "resourceId={0}&principalId={1}&acls={2}".formatEscape(ResourceId, principalId, acls);
        PostManager.post(Constants.PermissionsService.SetAclsUrl, data, OnSavePermissionsComplete);
    }
}

function GetPermissionsChecked(count) {
    var acls = 0;

    // set the IDs for read, write, delete, grant checkboxes
    var readChBox = $("#read_" + count);
    var writeChBox = $("#write_" + count);
    var deleteChBox = $("#delete_" + count);
    var grantChBox = $("#grant_" + count);

    if (readChBox != null && readChBox.is(":checked")) {
        acls += Acls.Read;
    }

    if (writeChBox != null && writeChBox.is(":checked")) {
        acls += Acls.Write;
    }

    if (deleteChBox != null && deleteChBox.is(":checked")) {
        acls += Acls.Delete;
    }

    if (grantChBox != null && grantChBox.is(":checked")) {
        acls += Acls.Grant;
    }

    return acls;
}

///
/// OnSavePermissionsComplete - called when the save/setacls is completed for the article
///
function OnSavePermissionsComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";

    if (!response.success) {
        message = response.error;
    }
    else {

        // save has been successful.
        // add a new TR to the permissions table
        // remove the TR that allows to save

        alert('done');
    }

    ShowPermissionSavedStatus(message);
}

function ShowUserSearchStatus(message) {
    alert(message);
}

function ShowPermissionSavedStatus(message) {
    alert(message);
}

function ShowPermissionDeleteStatus(message) {
    alert(message);
}
