<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GalleryList.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.gallery.GalleryList" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
<Ratna:ConfirmDialog id="confirmDialog" runat="server" />

<div class="box">
    <span id="headerspan" runat="server">
        <h2><%= ResourceManager.GetLiteral("Admin.Media.Gallery")%></h2>
    </span>
    <table>
        <thead>
            <tr>
                <th style="width: 20px;"><%= ResourceManager.GetLiteral("Admin.Common.Edit")%></th>
                <th style="width: 20px;"><%= ResourceManager.GetLiteral("Admin.Common.Preview")%></th>
                <th><%= ResourceManager.GetLiteral("Admin.Common.Url")%></th>
                <th><%= ResourceManager.GetLiteral("Admin.Common.Title")%></th>
                <th style="width: 20px;"><%= ResourceManager.GetLiteral("Admin.Common.Delete")%></th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater runat="server" ID="repeater" OnItemDataBound="RepeaterItemEventHandler">
                <ItemTemplate>
                    <tr>
                        <td style="width: 20px;">
                            <a id="editurlanchor" href='<%# Constants.Urls.Media.Gallery.EditUrlWithKey + Eval("Url") %>'
                                runat="server">
                                <img id="editImage" title="<%$ Resource:Admin.Common.Edit%>" alt="<%$ Resource:Admin.Common.Edit%>" 
                                    src="/images/edit-pencil.png"
                                    class="imageButton16" runat="server" /></a>
                        </td>
                        <td style="width:20px">
                            <a id="previewgalleryAnchor" href='<%# Eval("Url") %>' target="_blank"
                                        runat="server">
                                <img id="previewImage" title="<%$ Resource:Admin.Common.Preview%>" src="/images/preview.png"
                                    class="imageButton16" runat="server" /></a>
                        </td>
                        <td style="width: 200px;">
                             <asp:Label ID="urlLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Url") %>' />
                        </td>
                        <td style="width: 200px;">
                             <asp:Label ID="titleLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>' />
                        </td>
                        <td style="width: 20px;">
                            <input type="hidden" id="deleteid" value="<%# DataBinder.Eval(Container.DataItem, "Url") %>" />
                            <img alt="delete" title="<%$ Resource:Admin.Common.Delete%>" class="imageButton16 deleteGalleryImageButton"
                                id="deleteForGallery" src="/images/delete.png" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr id="none" runat="server">
                <td colspan="3"> 
                    <%= this.Parameters.NothingFoundText%>
                </td>
            </tr>
        </tbody>
    </table>
</div>