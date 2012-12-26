<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="autopathscontrol.ascx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.controls.Settings.autopathscontrol" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
<Ratna:ConfirmDialog id="confirmDialog" runat="server" />

<div class="box" runat="server" id="autoPathsListDiv">
    <span id="headerspan" runat="server">
        <h2><%= ResourceManager.GetLiteral("Admin.Settings.AutoPaths.Title")%></h2>
        <div class="utils">
            <a href="#" id="addnew" runat="server">
                <%= ResourceManager.GetLiteral("Admin.Settings.AutoPaths.AddNew")%></a>
        </div>
    </span>
    <table>
        <thead>
            <tr>
                <th style="width:25px">
                    <%= ResourceManager.GetLiteral("Admin.Common.Edit")%>
                </th>
                <th style="width:25px">
                    <%= ResourceManager.GetLiteral("Admin.Common.Preview")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Common.Title")%>
                </th>
                <th>
                    <%= ResourceManager.GetLiteral("Admin.Common.Url")%>
                </th>
                <th style="width:25px">
                    <%= ResourceManager.GetLiteral("Admin.Common.Delete")%>
                </th> 
            </tr>
        </thead>
        <tbody>
            <asp:Repeater runat="server" ID="repeater">
                <ItemTemplate>
                    <tr>
                        <td style="width:25px">
                            <a id="editautopathanchor" href='<%# Constants.Urls.Settings.EditAutoPathUrlWithKey + Eval("Id") %>'
                                        runat="server">
                                <img id="editImage" title="<%$ Resource:Admin.Common.Edit%>" src="/images/edit-pencil.png"
                                    class="imageButton16" runat="server" /></a>
                        </td>
                        <td style="width:25px">
                            <a id="previewautoPathAnchor" href='<%# Eval("Path") %>' target="_blank"
                                        runat="server">
                                <img id="previewImage" title="<%$ Resource:Admin.Common.Preview%>" src="/images/preview.png"
                                    class="imageButton16" runat="server" /></a>
                        </td>
                        <td><asp:Label ID="titleLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>' /></td>
                        <td><asp:Label ID="urlPathLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Path") %>' /></td>
                        <td style="width:25px">
                            <input type="hidden" id="deleteautopathurl" value="<%# Eval("Path") %>" />
                            <img alt="delete" title="<%$ Resource:Admin.Common.Delete%>" class="imageButton16 deleteAutoPathImageButton"
                                id="deleteForAutoPath" src="/images/delete.png" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr id="none" runat="server">
                <td colspan="5"> 
                    <%= ResourceManager.GetLiteral("Admin.Settings.AutoPaths.NoAutoPathFound")%>
                </td>
            </tr>
        </tbody>
    </table>
</div>