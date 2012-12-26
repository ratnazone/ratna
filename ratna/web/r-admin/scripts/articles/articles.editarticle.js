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
/// this javascript code is to support articles' edit page
///

$(document).ready(function () {
    Setup();
});

function Setup() {

    InitializeUrlKey();

    $("#cancelbutton").click(function () {
        OnCancelClick();
    });

    $("#savebutton").click(function () {
        OnSaveClick();
    });

    $("#publishbutton").click(function () {
        OnPublishClick();
    });

    $("#validatebutton").click(function () {
        OnValidateClick();
    });

    $("#urlkey").keyup(function () {
        Notification.Hide("#urlnotification");
    });

}

function IsPageView() {
    var view = $("#articleview").val();
    var isPageView = false;

    if (view == "page") {
        isPageView = true;
    }

    return isPageView;
}

function InitializeUrlKey() {
    var urlKey = $("#urlkey").val();

    if (!IsPageView() && (urlKey == null || urlKey.trim() == "")) {
        var date = new Date();
        var sampleUrl = "/{0}/{1}/{2}/<add meaningful url>".format(date.getFullYear(), (date.getMonth() + 1), date.getDate());
        $("#urlkey").val(sampleUrl);
    }
}

function OnSaveClick() {

    var urlkeyval = $("#urlkey").val();
    var title = $("#articletitle").val();
    var body = Get_ArticleBodyContents();
    var view = $("#articleview").val();
    var isPageView = false;

    if (view == "page") {
        isPageView = true;
    }

    var data = null;
    var posturl = null;

    if (!isPageView) {
        data = "urlKey={0}&title={1}&body={2}".formatEscape(urlkeyval, title, body);
        posturl = Constants.ArticleService.SaveUrl;
    }
    else {
        data = "urlKey={0}&title={1}&body={2}".formatEscape(urlkeyval, title, body);
        posturl = Constants.ArticleService.PageSaveUrl;
    }


    SavingNotification.Show();
    PostManager.post(posturl, data, OnSaveComplete);
}

function OnValidateClick() {
    var urlkeyval = $("#urlkey").val();
    var isUrlValid = false;

    // make sure url starts with '/'
    // it can not be the root url either.
    if (urlkeyval != null &&
                urlkeyval.length > 1 &&
                urlkeyval[0] == '/') {
        isUrlValid = true;
    }

    if (isUrlValid) {
        var data = "urlKey={0}".formatEscape(urlkeyval);
        PostManager.post(Constants.ArticleService.ValidateKeyUrl, data, OnValidationComplete);
    }
    else {
        // invalid url.
        ShowUrlValidity(false);
    }
}

function OnValidationComplete(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        ShowUrlValidity(true);
    }
    else {
        ShowUrlValidity(false);
    }
}

function ShowUrlValidity(success) {
    if (success) {
        Notification.Show("#urlnotification", L_ValidateUrlSuccess, true);
    }
    else {
        Notification.Show("#urlnotification", L_ValidateUrlError, false);
    }
}

///
/// OnSaveComplete - called when the save is completed for the article
///
function OnSaveComplete(success, data) {
    var response = jQuery.parseJSON(data);
    var message = "";

    if (!response.success) {
        message = response.error;
    }
    else {
        // save has been successful.
        // make sure that the validate button is disabled as well as urlkey is disabled.
        $("#validatebutton").attr("disabled", "disabled");
        $("#urlkey").attr("disabled", "disabled");

        //enable the add image button
        $("#photouploader").removeAttr("disabled");
        $("#photouploader").attr("title", L_AddImageTitle);

        var urlkey = $("#urlkey").val();

        FixMenu(urlkey);

        //fix the breadcrumb
        BreadCrumb.SetLastAnchor(Constants.Articles.EditUrlWithKey + urlkey + "&view=" + $("#articleview").val(), $("#articletitle").val());
    }

    SavingNotification.OnSavingComplete(response.success, message);
}

function FixMenu(urlkey) {
    if (urlkey != null ) {
        // grab all the menu items. If the menu items's url does not have the url added, add it.
        $("#menu a").each(function () {
            var href = $(this).attr("href");
            if (href.endsWith('url=')) {
                $(this).attr("href", href + urlkey);
            }
        });
    }
}

function OnCancelClick() {
    window.location = CancelUrl;
}

function OnPublishClick() {
    var urlKey = $("#urlkey").val();

    ConfirmDialog.confirm(
            L_PublishArticleConfirmation.format(urlKey), function () {
                OnPublishArticleConfirmed(urlKey);
            }
        );

}

function OnPublishArticleConfirmed(urlKey) {
    var data = "urlkey={0}".formatEscape(urlKey);

    PostManager.post(Constants.ArticleService.PublishUrl, data, OnPublishArticleCompleted);
}

function OnPublishArticleCompleted(success, data) {
    var response = jQuery.parseJSON(data);
    if (response.success) {
        Notification.Show("#urlnotification", L_PublishArticleSuccess, true);
    }
    else {
        Notification.Show("#urlnotification", response.error, false);
    }
}
