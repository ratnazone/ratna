<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArticleVersionsList.ascx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.controls.Articles.ArticleVersionsList" %>

<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
<%@ Register TagPrefix="Ratna" TagName="ConfirmDialog" Src="~/Controls/Common/ConfirmDialog.ascx" %>

<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />
<Ratna:ConfirmDialog id="confirmDialog" runat="server" />

 <div>
    <h3 class="grid_13">
        <%= ResourceManager.GetLiteral("Admin.Articles.Versions.Versions")%></h3>
    <table>
            <thead>
                <tr>
                    <th>&nbsp;</th>
                    <th>&nbsp;</th>
                    <th><%= ResourceManager.GetLiteral("Admin.Articles.Versions.Version")%></th>
                    <th><%= ResourceManager.GetLiteral("Admin.Articles.Versions.Versions.LastModifiedDate")%></th>
                    <th><%= ResourceManager.GetLiteral("Admin.Articles.Versions.Versions.ArchivedDate")%></th>
                    <th><%= ResourceManager.GetLiteral("Admin.Common.Delete")%></th>
                </tr>
            </thead>
    <tbody>
        <asp:Repeater runat="server" ID="versionsRepeater">
            <ItemTemplate>
                <tr id="versionstr_<%# Eval("Version") %>">
                    <td style="width:25px">
                            <a id="versionurlanchor" target="_blank" href='<%# "~/" + UrlKey + VersionAppender + Eval("Version") %>' runat="server">
                            <img alt="<%$ Resource:Admin.Articles.List.Preview%>" 
                            title="<%$ Resource:Admin.Articles.List.Preview%>" 
                            class="imageButton16 previewArticleVersionImageButton"
                            id="previewImage" src="/images/preview.png" runat="server" />
                            </a>
                    </td>
                    <td style="width:25px">
                        <input type="hidden" id="revertarticleid" value="<%= UrlKey %>" />
                        <input type="hidden" id="revertarticleversion" value="<%# Eval("Version") %>" />
                        <img alt="<%$ Resource:Admin.Articles.List.RevertToVersion%>" 
                            title="<%$ Resource:Admin.Articles.List.RevertToVersion%>" 
                            class="imageButton16 revertArticleVersionImageButton"
                            id="revertImage" src="/images/revert.png" runat="server" />
                    </td>
                    <td><%# Eval("Version") %></td>
                    <td><%# Eval("LastModifiedDate") %></td>
                    <td><%# Eval("ArchivedDate") %></td>
                    <td id="deleteVersionTD" style="width:25px">
                        <input type="hidden" id="deletearticleid" value="<%= UrlKey %>" />
                        <input type="hidden" id="deletearticleversion" value="<%# Eval("Version") %>" />
                        <img    alt="<%$ Resource:Admin.Articles.DeleteVersion%>" 
                                title="<%$ Resource:Admin.Articles.DeleteVersion%>" 
                                class="imageButton16 deleteArticleVersionImageButton"
                                id="deleteForArticle" src="/images/delete.png" runat="server" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>
</div>
