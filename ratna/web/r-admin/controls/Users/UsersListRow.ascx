<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UsersListRow.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.Users.UsersListRow" %>
<asp:Repeater runat="server" ID="repeater">
    <ItemTemplate>
        <tr>
            <td style="display: none">
                <input type="hidden" id="useralias" value="<%# Eval("Alias") %>" />
            </td>
            <td>
                <a href='<%# Constants.Urls.Users.ViewUserUrlWithKey + Eval("Alias") %>' 
                    runat="server"><asp:Label ID="alias" runat="server" Text='<%# Eval("Alias") %>' /></a>
            </td>
            <td>
                <asp:Label ID="name" runat="server" Text='<%# Eval("DisplayName") %>' />
            </td>
            <%
                            if (!this.ConciseView)
                            { 
            %>
            <td>
                <asp:Label ID="email" runat="server" Text='<%# Eval("Email") %>' />
            </td>
            <% 
                            } 
            %>
            <% 
                            if (this.IsActive && this.ExpandedView)
                            { 
            %>
            <td>
                <asp:Label ID="lastlogin" runat="server" Text='<%# Eval("LastLoginTime") %>' />
            </td>
            <% 
                            } 
            %>
            <% 
                            if (!this.IsActive && this.ExpandedView)
                            { 
            %>
            <td>
                <asp:Label ID="createdDate" runat="server" Text='<%# Eval("CreatedTime") %>' />
            </td>
            <td>
                <img src="~/images/activate.png" id="activateuser" alt="" runat="server" class="imageButton16 activateuserbtn" />
            </td>
            <% 
                            } 
            %>
            <% 
                            if (!this.IsDeleted && this.ExpandedView)
                            { 
            %>
            <td>
                <img src="~/images/delete.png" id="deleteuser" alt="" runat="server" class="imageButton16 deleteuserbtn" />
            </td>
            <%
                        } 
            %>
        </tr>
    </ItemTemplate>
</asp:Repeater>
