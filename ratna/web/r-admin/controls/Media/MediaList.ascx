<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MediaList.ascx.cs" 
    Inherits="Jardalu.Ratna.Web.Admin.controls.Media.MediaList" %>

<div class="box">
    
    <span id="headerspan" runat="server">
        <h2><asp:Literal runat="server" ID="header"></asp:Literal></h2>
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
                <%if (!Parameters.HideEdit){ %>
                <th><%= ResourceManager.GetLiteral("Admin.Media.List.Edit")%></th>
                <%} %>
                <%if (!Parameters.HidePreview){ %>
                <th><%= ResourceManager.GetLiteral("Admin.Media.List.Preview")%></th>
                <%} %>
                <th><%= ResourceManager.GetLiteral("Admin.Media.List.Name")%></th>
                <%if (!Parameters.HidePreview){ %>
                <th><%= ResourceManager.GetLiteral("Admin.Media.List.Url")%></th>
                <%} %>
                <th><%= ResourceManager.GetLiteral("Admin.Media.List.CreatedDate")%></th>
                <%if (Parameters.ShowDelete)
                  { %>
                <th><%= ResourceManager.GetLiteral("Admin.Media.List.Delete")%></th>
                <%}%>
            </tr>
        </thead>
        <%
            }
        %>
        <tbody>
            <asp:Repeater runat="server" ID="repeater" OnItemDataBound="RepeaterItemEventHandler">
                <ItemTemplate>
                    <tr>
                        <td style="width: 20px;" visible="<%# !Parameters.HideEdit %>" runat="server">
                            <a id="editurlanchor" href='<%# Constants.Urls.Media.Photos.EditUrlWithKey + Eval("Url") %>'
                                runat="server">
                                <img id="editImage" title="<%$ Resource:Admin.Media.List.Edit%>" alt="<%$ Resource:Admin.Media.List.Edit%>" 
                                    src="/images/edit-pencil.png" class="imageButton16" runat="server" /></a>
                        </td>
                        <td style="width: 20px;" visible="<%# !Parameters.HidePreview %>" runat="server">
                            <a id="previewanchor" href='<%# Eval("Url") %>' runat="server"
                                target="_blank">
                                <img id="previewImage" title="<%$ Resource:Admin.Media.List.Preview%>" alt="<%$ Resource:Admin.Media.List.Preview%>" 
                                src="/images/preview.png" class="imageButton16" runat="server" /></a>
                        </td>
                        <td style="width: 200px;">
                             <asp:Label ID="nameLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>' />
                        </td>
                        <td runat="server" visible="<%# !Parameters.HideUrl %>">
                             <asp:Label ID="urlLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Url") %>' />
                        </td>
                        <td style="width: 100px;">
                             <asp:Label ID="createdDateLabel" runat="server" Text='' />
                        </td>
                        <td runat="server" visible="<%# Parameters.ShowDelete %>" style="width: 20px;">
                            <input type="hidden" id="deleteid" value="<%# Eval("Url") %>" />
                            <img alt="delete" title="<%$ Resource:Admin.Media.List.Delete%>" class="imageButton16 deleteMediaImageButton"
                                id="deleteForMedia" src="/images/delete.png" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr id="none" runat="server">
                <td colspan="6"> 
                    <%= this.Parameters.NothingFoundText%>
                </td>
            </tr>
        </tbody>
    </table>

</div>
