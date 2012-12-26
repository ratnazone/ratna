<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserMembership.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.Users.UserMembership" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Admin" TagName="UserMembershipRow" Src="~/r-admin/controls/Users/UserMembershipRow.ascx" %>
<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
<div>
    <h2>
        <%=ResourceManager.GetLiteral("Admin.Users.Membership.Title") %></h2>
    <div class="utils" id="utils" runat="server" visible="false">
        <a href="" id="moreAnchor">
            <%= ResourceManager.GetLiteral("Admin.Common.More")%></a>
    </div>
    <table>
        <thead>
            <tr>
                <th>
                    <%=ResourceManager.GetLiteral("Admin.Users.Membership.Group") %>
                </th>
                <th>
                    <%=ResourceManager.GetLiteral("Admin.Users.Membership.Remove") %>
                </th>
            </tr>
        </thead>
        <tbody>
            <!-- groups -->
            <Admin:UserMembershipRow id="memeberShipRow" runat="server" />
            <!-- allow to add more groups -->
            <tr id="searchGroupTR">
                <td colspan="2">
                    <label>
                        <%= ResourceManager.GetLiteral("Admin.Users.Membership.Group.AddNew")%>
                        <input type="text" style="width: 240px" id="searchUserGroupText" />
                        <img id="searchgroupforaddbtn" alt="<%$ Resource:Admin.Users.Membership.Group.Search%>"
                            class="imageButton16 searchgroupbtn" title="<%$ Resource:Admin.Users.Membership.Group.Search%>"
                            src="~/images/search.png" runat="server" />
                    </label>
                </td>
            </tr>
        </tbody>
    </table>
</div>
