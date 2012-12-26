<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserMembershipRow.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.Users.UserMembershipRow" %>
<asp:Repeater runat="server" ID="repeater">
    <ItemTemplate>
        <tr>     
            <td style="display: none">
                <input type="hidden" id="groupid" value="<%# Eval("Id") %>" />
            </td>       
            <td><label><%# Eval("Name") %></label></td>
            <td>
                <img src="~/images/delete.png" class="imageButton16 removeusergroupbtn" alt="" runat="server"/>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>