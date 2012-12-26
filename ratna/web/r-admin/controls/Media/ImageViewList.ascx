<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageViewList.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Admin.controls.Media.ImageViewList" %>
<div class="box">
    <span id="headerspan" runat="server">
        <h2>
            <asp:Literal runat="server" ID="header"></asp:Literal></h2>
        <div class="utils">
            <a href="#" id="more" runat="server">
                <%= ResourceManager.GetLiteral("Admin.Common.More")%></a>
        </div>
    </span>
    <table style="text-align:justify">
        <tr>
            <td>
                <asp:Repeater runat="server" ID="repeater">
                    <ItemTemplate>
                        <% if (ShortMode)
                           { %>
                           <a runat="server" id="editurlanchor_shortmode" href='<%# Constants.Urls.Media.Photos.EditUrlWithKey + Eval("Url") %>'>
                                    <img id="Img1" runat="server" class="imageListView" style="cursor: pointer; height:80px; max-width:80px;" src='<%# Eval("Url")%>'
                                        title="<%$ Resource:Admin.Common.Edit %>" alt='<%# Eval("Name")%>' /></a>
                        <% } else { %>
                            <div class="imageListViewContainer">
                                <a runat="server" id="editurlanchor" href='<%# Constants.Urls.Media.Photos.EditUrlWithKey + Eval("Url") %>'>
                                    <img runat="server" class="imageListView" style="cursor: pointer;" src='<%# Eval("Url")%>'
                                        title="<%$ Resource:Admin.Common.Edit %>" alt='<%# Eval("Name")%>' /></a>
                                <br />
                                <%# Eval("Name")%>
                            </div>
                       <% } %>
                    </ItemTemplate>
                </asp:Repeater>
                <div id="none" runat="server">
                    <%= this.Parameters.NothingFoundText%>
                </div>
            </td>
        </tr>
    </table>
</div>
