<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Templates.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.Settings.Templates" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
<Ratna:ConfirmDialog id="confirmDialog" runat="server" />

<div class="box" runat="server" id="templatesListDiv">
    <span id="headerspan" runat="server">
        <h2><%= ResourceManager.GetLiteral("Admin.Templates.List.Header")%></h2>
        <div class="utils">
            <a href="#" id="addnew" runat="server">
                <%= ResourceManager.GetLiteral("Admin.Templates.List.AddNewTemplate")%></a>
        </div>
    </span>
    <table>
        <thead>
            <tr>
                <th>
                    <!-- empty -->
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Templates.List.Active")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Templates.List.Name")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Templates.List.TemplatePath")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Templates.List.UrlPath")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Templates.List.Delete")%>
                </th> 
            </tr>
        </thead>
        <tbody>
            <asp:Repeater runat="server" ID="repeater">
                <ItemTemplate>
                    <tr>
                        <td>
                            <a id="edittemplateanchor" href='<%# Constants.Urls.Settings.EditTemplateUrlWithKey + Eval("Id") %>'
                                runat="server">
                                <img id="editImage" title="<%$ Resource:Admin.Templates.List.Edit%>" src="/images/edit-pencil.png"
                                    class="imageButton16" runat="server" /></a>
                        </td>
                        <td><asp:Label ID="activeLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IsActivated") %>' /></td>
                        <td><asp:Label ID="nameLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>' /></td>
                        <td><asp:Label ID="templatePathLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "TemplatePath") %>' /></td>
                        <td><asp:Label ID="urlPathLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "UrlPath") %>' /></td>
                        <td>
                            <input type="hidden" id="deletetemplateurl" value="<%# Eval("UrlPath") %>" />
                            <input type="hidden" id="deletetemplateuid" value="<%# Eval("UId") %>" />
                            <img alt="delete" title="<%$ Resource:Admin.Templates.List.Delete%>" class="imageButton16 deleteTemplateImageButton"
                                id="deleteForTemplate" src="/images/delete.png" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr id="none" runat="server">
                <td> 
                    <%= ResourceManager.GetLiteral("Admin.Templates.List.NoTemplatesFound")%>
                </td>
            </tr>
        </tbody>
    </table>
</div>
