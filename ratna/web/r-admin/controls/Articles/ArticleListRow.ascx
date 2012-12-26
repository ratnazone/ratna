<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArticleListRow.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.Articles.ArticleListRow" %>
<asp:Repeater runat="server" id="repeater" OnItemDataBound="RepeaterItemEventHandler">
    <ItemTemplate>
        <tr>
            <td class="checkboxtd" visible="<%# Parameters.AllowMultiSelect %>" runat="server">
                <input type="checkbox" name="multiactioncheckbox" id="multiactioncheckbox" class="checkboxinput" />
            </td>
            <td style="width: 20px;">
                <a id="editurlanchor" href='<%# Constants.Urls.Articles.EditUrlWithKey + Eval("UrlKey") %>'
                    runat="server">
                    <img id="Img1" title="<%$ Resource:Admin.Articles.List.Edit%>" alt="<%$ Resource:Admin.Articles.List.Edit%>" 
                        src="/images/edit-pencil.png"
                        class="imageButton16" runat="server" /></a>
            </td>
            <td style="width: 20px;">
                <a id="previewanchor" href='<%# "~/" + Eval("UrlKey") + PreviewAppender %>' runat="server"
                    target="_blank">
                    <img id="Img2" title="<%$ Resource:Admin.Articles.List.Preview%>" alt="<%$ Resource:Admin.Articles.List.Preview%>"
                        src="/images/preview.png"
                        class="imageButton16" runat="server" /></a>
            </td>
            <td>
                <asp:Label ID="urlkeyLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "UrlKey") %>' />
            </td>
            <td>
                <a id="urlanchor" href='<%# DataBinder.Eval(Container.DataItem, "UrlKey") %>' runat="server">
                    <asp:Label ID="titleLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>' /></a>
            </td>
            <td id="Td1" runat="server" visible="<%# Parameters.ShowPublish %>">
                <input type="hidden" id="publisharticleid" value="<%# Eval("UrlKey") %>" />
                <img alt="publish" title="<%$ Resource:Admin.Articles.List.Publish%>" class="imageButton16 publishArticleImageButton"
                    id="publishForArticle" src="/images/publish.png" runat="server" />
            </td>
            <td id="Td2" runat="server" visible="<%# Parameters.ShowDelete %>">
                <input type="hidden" id="deletearticleid" value="<%# Eval("UrlKey") %>" />
                <img alt="delete" title="<%$ Resource:Admin.Articles.List.Delete%>" class="imageButton16 deleteArticleImageButton"
                    id="deleteForArticle" src="/images/delete.png" runat="server" />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
