<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UsersList.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.Users.UsersList" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Admin" TagName="UsersListRow" Src="~/r-admin/controls/Users/UsersListRow.ascx" %>
<%@ Register TagPrefix="Common" TagName="Notification" Src="~/Controls/Common/Notification.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />

<div class="box">
    <%
            if (this.DisplayHeader)
            {
    %>
    <h2>
        <%= Title %></h2>
    <div class="utils">
        <a href="#" id="moreAnchor" runat="server">
            <%= ResourceManager.GetLiteral("Admin.Common.More")%></a>
    </div>
    <%
        } 
    %>
    <table>
        <%
            if (this.DisplayTableHeader)
            {
        %>
        <thead>
            <tr>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Users.Alias")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Users.Name")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Users.Email")%>
                </th>
                <% if (this.IsActive) { %>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Users.LastLogin")%>
                </th>
                <% } %>
                <% if (!this.IsActive) { %>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Users.CreatedOn")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Users.Activate")%>
                </th>
                <% } %>
                <% if (!this.IsDeleted) { %>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Users.Delete")%>
                </th>
                <% } %>
            </tr>
        </thead>
        <%
            }
        %>
        <tbody>
            
            <!-- notification -->
            <Common:Notification id="activationnotification" runat="server" />
            <Common:Notification id="deletionnotification" runat="server" />

            <Admin:UsersListRow id="usersListRow" runat="server" />

            <tr id="none" runat="server">
                <td>
                    <%= ResourceManager.GetLiteral("Admin.Users.NoUsers")%>
                </td>
            </tr>
        </tbody>
    </table>
</div>
