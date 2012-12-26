<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArticlesList.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.Articles.ArticlesList" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>
<%@ Register TagPrefix="Admin" TagName="ArticleListRow" Src="~/r-admin/controls/Articles/ArticleListRow.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
<Ratna:ConfirmDialog id="confirmDialog" runat="server" />

<div class="box" runat="server" id="articleListDiv">
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
                <% if (this.Parameters.AllowMultiSelect) { %> 
                <th class="checkboxtd"><input type="checkbox" name="selectall" id="selectall" class="checkboxinput" /></th>
                <% } %> 
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Articles.List.Edit")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Articles.List.Preview")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Articles.List.Url")%>
                </th>  
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Articles.List.Title")%>
                </th>    
                <% if (this.Parameters.ShowPublish) { %>        
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Articles.List.Publish")%>
                </th> 
                <% } %> 
                <% if (this.Parameters.ShowDelete) { %>        
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Articles.List.Delete")%>
                </th> 
                <% } %>                    
            </tr>
        </thead>
        <%
            }
        %>
        <tbody>

            <Admin:ArticleListRow id="articleListRow" runat="server" />

            <% if (this.Parameters.AllowMultiSelect) { %> 
                <tr id="actioncontrols" runat="server">
                <td colspan="5">
                    <input type="hidden" id="responseform" name="responseform" value="0" runat="server" />
                    <input type="button" id="deleteArticlesButton" value='<%= ResourceManager.GetLiteral("Admin.Common.Delete")%>' />
                    <% if (this.Parameters.ShowPublish)
                       { %>    
                     <input type="button" id="publishArticlesButton" value='<%= ResourceManager.GetLiteral("Admin.Articles.List.Publish")%>' />
                    <%} %>
                </td>
                </tr>
            <% } %> 
            
            <tr id="none" runat="server">
                <td colspan="5"> 
                    <%= this.Parameters.NothingFoundText%>
                </td>
            </tr>
        </tbody>
    </table>
</div>
