<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentsList.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.Comments.CommentsList" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
<Ratna:ConfirmDialog id="confirmDialog" runat="server" />

<div class="box" runat="server" id="commentsListDiv">
    <span id="headerspan" runat="server">
        <h2>
            <asp:Literal runat="server" ID="header"></asp:Literal></h2>
        <div class="utils">
            <a href="#" id="more" runat="server">
                <%= ResourceManager.GetLiteral("Admin.Common.More")%></a>
        </div>
    </span>
    <table>
        <%
            if (this.Parameters.DisplayTableHeader)
            {
        %>
        <thead>
            <tr>
                <th></th>
                <% if (Parameters.ShowKey)
                   { %>
                <th> <%= ResourceManager.GetLiteral("Admin.Comments.List.Key")%></th>
                <% } %>
                <th> <%= ResourceManager.GetLiteral("Admin.Comments.List.User")%></th>
                <th> <%= ResourceManager.GetLiteral("Admin.Comments.List.Comment")%></th>
                 <% if (Parameters.ShowApprove)
                    { %>
                <th> <%= ResourceManager.GetLiteral("Admin.Comments.List.Approve")%></th>
                <% } %>
                <% if (Parameters.ShowDelete)
                    { %>
                <th> <%= ResourceManager.GetLiteral("Admin.Comments.List.Delete")%></th>
                <% } %>
            </tr>
        </thead>
        <% } %>
        <tbody>

            <asp:Repeater runat="server" ID="repeater" OnItemDataBound="OnItemDataBound">
                <ItemTemplate>
                    <tr>
                        <td style="width:60px">
                            <asp:Label ID="dateLabel" runat="server" Text="" />
                            <input type="hidden" id="commentuid" value="<%# Eval("UId") %>" />
                        </td>
                        <td runat="server" visible="<%# Parameters.ShowKey %>">
                            <asp:Label ID="keyLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Key") %>' />
                        </td>
                         <td>
                            <asp:Label ID="name" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>' />
                            (<asp:Literal ID="email" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Email") %>' />)
                        </td>
                         <td>
                            <asp:Label ID="commentbody" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Body") %>' />
                        </td>
                        <td runat="server" visible="<%# Parameters.ShowApprove %>" style="width:20px">
                            <img alt="publish" title="<%$ Resource:Admin.Comments.List.Approve%>" class="imageButton16 approveCommentImageButton"
                                id="approveForComment" src="/images/ok.png" runat="server" />
                        </td>
                        <td runat="server" visible="<%# Parameters.ShowDelete %>"  style="width:20px">
                            <img alt="delete" title="<%$ Resource:Admin.Comments.List.Delete%>" class="imageButton16 deleteCommentImageButton"
                                id="deleteForComment" src="/images/delete.png" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>

            <tr id="none" runat="server" visible="false">
                <td><%= Parameters.NothingFoundText %></td>
            </tr>

        </tbody>
    </table>
</div>
