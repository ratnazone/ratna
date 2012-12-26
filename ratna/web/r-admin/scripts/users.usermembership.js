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
/// functions to support user membership control
///

var userMembershipControl = {

    OnSearchGroupClick: function () {
        var query = $("#searchUserGroupText").val();

        if ((query == null) || (query.trim() == "")) {
            ShowGroupSearchStatus(L_EnterSearchCriteria);
        }
        else {
            // get the snippet with the search
            var snippetManager = new SnippetManager();
            snippetManager.enable("UserMembershipSearchRow");
            snippetManager.add("query");

            // search
            var data = "query={0}".formatEscape(query);
            data = "{0}{1}".format(data, snippetManager.getQuerySearch());
            PostManager.post(Constants.GroupsService.SearchGroupUrl, data, userMembershipControl.OnUsersGroupSearchComplete);
        }
    },

    OnUsersGroupSearchComplete: function (success, data) {
        var response = jQuery.parseJSON(data);
        if (response.success && response.isGroup) {
            userMembershipControl.ShowPrincipalPermissionsAdd(response.name, response.principalId, response.html);
            userMembershipControl.ShowGroupSearchStatus(L_UserOrGroupFound);
        }
        else {
            userMembershipControl.ShowGroupSearchStatus(L_NoUserOrGroupFound);
        }
    },

    ShowPrincipalPermissionsAdd: function (name, principalId, html) {
        // append the html after searchGroupTR
        var searchGroupTR = $("#searchGroupTR");
        if (searchGroupTR != null) {
            $(html).insertAfter(searchGroupTR);
        }
    },

    ShowGroupSearchStatus: function (message) {
        alert(message);
    }

};


$(document).ready(function () {
    ControlUserMembershipSetup();
});

function ControlUserMembershipSetup() {
    $(".searchgroupbtn").click(function () {
        userMembershipControl.OnSearchGroupClick();
    });
}


