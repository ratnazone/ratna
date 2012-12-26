<%@ Page Title="" Language="C#" MasterPageFile="~/r-admin/template/admin.Master"
    AutoEventWireup="true" CodeBehind="versions.aspx.cs" Inherits="Jardalu.Ratna.Web.Admin.articles.versions" %>

<%@ Register TagPrefix="Ratna" TagName="PageAccess" Src="~/Controls/Common/PageAccess.ascx" %>
<%@ Register TagPrefix="Common" TagName="SavingNotification" Src="~/Controls/Common/SavingNotification.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>
<%@ Register TagPrefix="Admin" TagName="Menu" Src="~/r-admin/controls/Menu.ascx" %>

<%@ Register TagPrefix="Admin" TagName="ArticleVersionsList" Src="~/r-admin/controls/Articles/ArticleVersionsList.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <Ratna:PageAccess id="pageAccess" runat="server" IsRestricted="true" AllowGroups="Administrator|Editor|Author" />
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/post.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/savingnotification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/notification.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/scripts/acls.js")%>'></script>
    <script type="text/javascript" src='<%=ResolveUrl("~/external/form/form.jquery.js")%>'></script>
    <script language="javascript" type="text/javascript">

        var L_NoUserOrGroupFound = '<%=ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.NoUserOrGroupFound") %>';
        var L_UserOrGroupFound = '<%=ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.UserOrGroupFound") %>';
        var L_EnterSearchCriteria = '<%=ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.EnterSearchCriteria") %>';
        var L_NoPermissionsChecked = '<%=ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.NoPermissionsChecked") %>';
        var L_PermissionsDeleted = '<%=ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.PermissionsDeleted") %>';
        var L_User = '<%=ResourceManager.GetLiteral("Admin.Common.User") %>';
        var L_Group = '<%=ResourceManager.GetLiteral("Admin.Common.Group") %>';
        var ResourceId = '<%=ResourceId %>';

        var newlyAddedTrIdPrefix = "addedTrId_";
        var addedTRCount = 0;

    </script>
    <script type="text/javascript" src='<%=ResolveUrl("~/r-admin/scripts/articles/article.versions.js")%>'></script>
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div id="content" class="container_16 clearfix" runat="server">

        <Common:Notification id="notification" runat="server" />

        <div class="grid_16">
            <h1>
                <%= ResourceManager.GetLiteral("Admin.Articles.Versions")%></h1>
        </div>

        <div class="grid_3">
            <Admin:Menu id="menu" runat="server" />
        </div>

        <div class="grid_13">

        <div class="grid_13">
            <p>
            <div class="grid_3">
                <strong>
                    <%= ResourceManager.GetLiteral("Admin.Articles.Versions.Url")%>
                </strong>
            </div>
            <div class="grid_9">
                <span id="urlspan" runat="server"></span>
            </div>
            </p>
        </div>
        <div class="grid_13">
            <p>
            <div class="grid_3">
                <strong>
                    <%= ResourceManager.GetLiteral("Admin.Articles.Versions.Owner")%>
                </strong>
            </div>
            <div class="grid_9">
                <span id="ownerspan" runat="server"></span>
            </div>
            </p>
        </div>
        <div class="grid_13">
            <p>
            <div class="grid_3">
                <strong>
                    <%= ResourceManager.GetLiteral("Admin.Articles.Versions.Version")%>
                </strong>
            </div>
            <div class="grid_9">
                <span id="versionspan" runat="server"></span>
            </div>
            </p>
        </div>
        <div class="grid_13">
            <p>
            <div class="grid_3">
                <strong>
                    <%= ResourceManager.GetLiteral("Admin.Articles.Versions.CreatedDate")%>
                </strong>
            </div>
            <div class="grid_9">
                <span id="createdDateSpan" runat="server"></span>
            </div>
            </p>
        </div>
        <div class="grid_13">
            <p>
            <div class="grid_3">
                <strong>
                    <%= ResourceManager.GetLiteral("Admin.Articles.Versions.LastModified")%>
                </strong>
            </div>
            <div class="grid_9">
                <span id="lastModifiedSpan" runat="server"></span>
            </div>
            </p>
        </div>

        <!-- permissions -->
        <div>
            <h3 class="grid_13">
                <%= ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions")%></h3>
            <table>
                    <thead>
                        <tr>
                            <th><%= ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.Delete")%></th>
                            <th><%= ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.Principal")%></th>
                            <th><%= ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions")%></th>
                        </tr>
                    </thead>
                <tbody>
                    <asp:Repeater runat="server" ID="repeater">
                        <ItemTemplate>
                            <tr id="aclstr_<%# Eval("Id") %>">
                                <td>
                                    <input type="hidden" id="principalId_<%# Eval("Id") %>" value="<%# Eval("Id") %>" />
                                    <img title="<%$ Resource:Admin.Articles.Versions.Permissions.Delete%>"
                                         alt="<%$ Resource:Admin.Articles.Versions.Permissions.Delete%>" 
                                         src="/images/delete.png"
                                         class="imageButton16 deletePermissionButton" 
                                         id="deletePermissionButton" 
                                         runat="server" />
                                </td>
                                <td>
                                    <label style="display: inline">
                                        <%# Eval("Name") %></label>
                                </td>
                                <td>
                                    <label style="display: inline">
                                        <%# Eval("Acl") %></label>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr>
                        <td colspan="3">
                            <label>
                                <%= ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.SearchUserGroupToAdd")%>
                                <input type="text" style="width: 240px" id="searchUserGroupText" />
                                <img id="searchuserbutton" alt="<%$ Resource:Admin.Articles.Versions.Permissions.SearchUser%>"
                                    class="imageButton16" title="<%$ Resource:Admin.Articles.Versions.Permissions.SearchUser%>"
                                    src="~/images/search.png" runat="server" />
                            </label>
                        </td>
                    </tr>
                    <tr id="addnewpermission" style="display:none">
                        <td>
                            <label id="principalName" style="display: inline">
                            </label>
                            <span id="principalType" style="display: inline"></span>
                            <input type="hidden" id="principalId" value="" />
                        </td>
                        <td>
                            <label class="checkboxlabel">
                                <input type="checkbox" id="read" class="checkboxinput" /><%= ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.Acls.Read")%></label>
                            <label class="checkboxlabel">
                                <input type="checkbox" id="write" class="checkboxinput" /><%= ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.Acls.Write")%></label>
                            <label class="checkboxlabel">
                                <input type="checkbox" id="delete" class="checkboxinput" /><%= ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.Acls.Delete")%></label>
                            <label class="checkboxlabel">
                                <input type="checkbox" id="grant" class="checkboxinput" /><%= ResourceManager.GetLiteral("Admin.Articles.Versions.Permissions.Acls.Grant")%></label>
                        </td>
                        <td>
                            <img id="savePermissionsImage" style="width: 16px; height: 16px; cursor: pointer"
                                alt="<%$ Resource:Admin.Articles.Versions.Permissions.Save%>" 
                                title="<%$ Resource:Admin.Articles.Versions.Permissions.Save%>"
                                src="/images/save.png" runat="server" />
                            <img id="cancelSaveImage" style="width: 16px; height: 16px; cursor: pointer" 
                                alt="<%$ Resource:Admin.Articles.Versions.Permissions.Cancel%>"
                                title="<%$ Resource:Admin.Articles.Versions.Permissions.Cancel%>" src="/images/cancel.png"
                                runat="server" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- versions -->
        <Admin:ArticleVersionsList id="articleVersionsList" runat="server" />

        </div>

    </div>
    <div id="errorDiv" class="container_16 clearfix" runat="server">
        <label>
            <%=  ResourceManager.GetLiteral("Admin.Articles.Edit.NoArticle")%></label>
    </div>
</asp:Content>
