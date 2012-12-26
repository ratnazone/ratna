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
    $(".approveCommentImageButton").click(function () {
        OnApproveCommentClick($(this));
    });

    $(".deleteCommentImageButton").click(function () {
        OnDeleteCommentClick($(this));
    });
}

function OnApproveCommentClick(approveButton) {
    if (approveButton != null) {
        // approve the comment

        //get the parent
        var tr = approveButton.parent().parent();

        //find uid
        var uidInput = tr.find("#commentuid");
        var uid = uidInput.val();

        // approval shouldn't require confirmation.
        var data = "uid={0}".format(uid);
        PostManager.post(Constants.CommentsService.ApproveCommentUrl, data, OnApproveCommentComplete);
    }
}


function OnApproveCommentComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";


    if (!response.success) {
        message = L_CommentApprovalFailure;
    }
    else {
        RemoveTRContainingUid(response.uid);
        message = L_CommentApprovalSuccess;
    }

    ShowApprovalResult(response.success, message);
}

function ShowApprovalResult(success, message) {
    Notification.Show("#approvalnotification", message, success);
}

function OnDeleteCommentClick(deleteButton) {
    if (deleteButton != null) {
        // activate the user

        //get the parent
        var tr = deleteButton.parent().parent();

        //find uid
        var uidInput = tr.find("#commentuid");
        var uid = uidInput.val();

        ConfirmDialog.confirm(
            L_DeleteCommentConfirmation, function () {
                OnDeleteCommentConfirmed(uid);
            }
        );

    }
}

function OnDeleteCommentConfirmed(uid) {
    var data = "uid={0}".format(uid);
    PostManager.post(Constants.CommentsService.DeleteCommentUrl, data, OnDeleteCommentComplete);
}

function OnDeleteCommentComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";


    if (!response.success) {
        message = L_CommentDeletionFailure;
    }
    else {
        // remove the tr that hosts it.
        RemoveTRContainingUid(response.uid);

        message = L_CommentDeletionSuccess;
    }

    ShowDeletionResult(response.success, message);
}

function ShowDeletionResult(success, message) {
    Notification.Show("#deletionnotification", message, success);
}

function RemoveTRContainingUid(commentuid) {
    var hiddenInputField = $('input[value="' + commentuid + '"]');

    //get the parent's parent
    var tr = hiddenInputField.parent().parent();

    // remove the tr
    tr.remove();
}
