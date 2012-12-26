<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserMembershipSearchRow.ascx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.controls.Users.UserMembershipSearchRow" %>

<tr>
    <td>
        <input type="hidden" id="groupid" value="" runat="server"/>
        <label id="groupname" runat="server"></label>
    </td>
    <td>
        <img src="~/images/save.png" class="imageButton16" runat="server" alt="<%$ Resource:Admin.Common.Save%>" title="<%$ Resource:Admin.Common.Save%>"/>
        <img src="~/images/cancel.png" class="imageButton16" runat="server" alt="<%$ Resource:Admin.Common.Cancel%>" title="<%$ Resource:Admin.Common.Cancel%>"/>
    </td>
</tr>