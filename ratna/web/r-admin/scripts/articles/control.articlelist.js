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
/// javascript code for ArticlesList.ascx
$(document).ready(function () {
    ArticleListControlSetup();

    $("#selectall").change(function () {
        OnSelectAllCheckboxChange();
    });
});

function ArticleListControlSetup() {
    $(".deleteArticleImageButton").click(function () {
        OnDeleteArticleButtonClick($(this));
    });

    $(".publishArticleImageButton").click(function () {
        OnPublishArticleButtonClick($(this));
    });

    $("#deleteArticlesButton").click(function () {
        OnMulitpleArticlesButtonClick("delete");
    });

    $("#publishArticlesButton").click(function () {
        OnMulitpleArticlesButtonClick("publish");
    });
}

function OnSelectAllCheckboxChange() {
    var checked = $("#selectall").is(":checked");

    $("tbody").find(":checkbox").attr("checked", checked);
}

function OnMulitpleArticlesButtonClick(action) {

    // get all the checked buttons
    var selectedurls = "";

    var selectedchboxes = $("tbody").find(":checked");

    if (selectedchboxes == null || selectedchboxes.length == 0) {
        //show error to select

        var alertMessage = L_SelectArticlesToDelete;
        if (action == "publish") {
            alertMessage = L_SelectArticlesToPublish;
        }

        Notification.Show("#commonnotification", alertMessage, false /* autohide */)

        return;
    }

    // select all the checked checkboxes
    for (var i = 0; i < selectedchboxes.length; i++) {
        var chkbox = selectedchboxes[i];
        var urlkey = $(chkbox).parent().parent().find("#urlkeyLabel").html();
        if (selectedurls == "") {
            selectedurls += urlkey;
        }
        else {
            selectedurls += ',' + urlkey;
        }
    }

    var alertMessage = L_DeleteMultiArticlesConfirmation;
    if (action == "publish") {
        alertMessage = L_PublishMultiArticlesConfirmation;
    }

    // confirm selected responses
    ConfirmDialog.confirm(
            alertMessage, function () {
                OnMultipleArticlesButtonClickConfirmed(action, selectedurls);
            }
        );
}

function OnMultipleArticlesButtonClickConfirmed(action, selectedurls) {

    var postUrl = Constants.ArticleService.DeleteMultipleUrl;
    if (action == "publish") {
        postUrl = Constants.ArticleService.PublishMultipleUrl;
    }

    var data = "urlKeys={0}".format(selectedurls);
    PostManager.post(postUrl, data, OnMultipleArticlesActionCompleted, selectedurls);
}

function OnMultipleArticlesActionCompleted(success, data, selectedUrls) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        var tokens = selectedUrls.split(',');
        for (var i = 0; i < tokens.length; i++) {
            deletedUrlKey = tokens[i];

            var hiddenInputField = $('input[value="' + deletedUrlKey + '"]');

            //get the parent's parent
            var tr = hiddenInputField.parent().parent();

            // remove the tr
            tr.remove();
        }

        ShowDeleteArticleStatus(L_DeleteArticleSuccess);
    }
    else {
        ShowDeleteArticleStatus(response.error);
    }

}


function OnDeleteArticleButtonClick(deleteButton) {
    if (deleteButton != null) {
        var deleteInput = deleteButton.prev();
        var urlKey = deleteInput.val();

        ConfirmDialog.confirm(
            L_DeleteArticleConfirmation.format(urlKey), function () {
                OnDeleteArticleConfirmed(urlKey);
            }
        );
    }
}

function OnDeleteArticleConfirmed(urlKey) {
    var data = "urlkey={0}".formatEscape(urlKey);
    PostManager.post(Constants.ArticleService.DeleteUrl, data, OnDeleteArticleCompleted);
}

function OnDeleteArticleCompleted(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        var deletedUrlKey = response.urlKey;
        var hiddenInputField = $('input[value="' + deletedUrlKey + '"]');

        //get the parent's parent
        var tr = hiddenInputField.parent().parent();
        var tbody = tr.parent();

        // remove the tr
        tr.remove();

        // if there are no tr's left, show the default tr
        // of "no posts" ....
        var trsLeft = 0;
        tbody.find("tr").each(function () {
            trsLeft++;
        });

        // only one TR left (that is the none)
        if (trsLeft == 1) {
            var noneTR = tbody.find('tr[id^="none"]');
            if (noneTR != null) {
                noneTR.show();
            }
        }

        ShowNotification(L_DeleteArticleSuccess, true);
    }
    else {
        ShowNotification(response.error, false);
    }
}


function ShowNotification(message, autohide) {
    Notification.Show("#commonnotification", message, autohide)
}

function OnPublishArticleButtonClick(publishButton) {
    if (publishButton != null) {
        var publishInput = publishButton.prev();
        var urlKey = publishInput.val();

        ConfirmDialog.confirm(
            L_PublishArticleConfirmation.format(urlKey), function () {
                OnPublishArticleConfirmed(urlKey);
            }
        );
    }
}

function OnPublishArticleConfirmed(urlKey) {

    var GetSnippetForPublished = true;
    var data = null;

    if (GetSnippetForPublished) {
        // get the snippet with the search
        var snippetManager = new SnippetManager();
        snippetManager.enable("ArticleListRow");
        snippetManager.add("urlkey");
        snippetManager.add("view");

        data = "urlkey={0}&view={1}".formatEscape(urlKey, "page");
        data = "{0}&{1}".format(data, snippetManager.getQueryString());
    }
    else {
        data = "urlkey={0}".formatEscape(urlKey);
    }
    PostManager.post(Constants.ArticleService.PublishUrl, data, OnPublishArticleCompleted);
}

function OnPublishArticleCompleted(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        var publishedUrlKey = response.urlKey;
        var title = response.title;
        var hiddenInputField = $('input[value="' + publishedUrlKey + '"]');

        //get the parent's parent
        var tr = hiddenInputField.parent().parent();

        // remove the tr
        tr.remove();

        var html = response.html;

        // add the new tr to published
        AddToPublishedArticlesList(html);

        ShowNotification(L_PublishArticleSuccess, true);
    }
    else {
        ShowNotification(response.error, false);
    }
}

/// <summary>
/// Adds to the published articles list.
/// </summary>
function AddToPublishedArticlesList(html) {
    var publishedArticlesList = $("#publishedArticlesList");
    if (publishedArticlesList != null) {

        // find tbody within the list
        var tbody = publishedArticlesList.find("tbody");
        if (tbody != null) {

            // get the first tr
            var firstTR = tbody.find("tr:first");
            if (firstTR != null) {
                $(html).insertBefore(firstTR);


                // check if "none" column exists. If so, remove that.
                var noneTR = tbody.find('tr[id^="none"]');
                if (noneTR != null) {
                    noneTR.hide();
                }
            }
        }
    }
}
