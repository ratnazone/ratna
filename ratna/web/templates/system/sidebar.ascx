<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="sidebar.ascx.cs" Inherits="Jardalu.Ratna.Web.templates.system.sidebar" %>
<div>
    <h4 class="header">
        About</h4>
    <span>System template for Ratna is for demonstration purpose only. Please avoid using
        this template on your site.</span>
</div>

<div id="recent" class="sidemenu" runat="server">
    <h4 class="header">
        Recent Posts</h4>
    <ul>
        <asp:Repeater ID="postsrepeater" runat="server" OnItemDataBound="Posts_OnItemDataBound">
            <ItemTemplate>
                <li id="postli" runat="server"><a href="#" id="postanchor" runat="server"></a>
                    <br />
                    <span>Posted on
                        <asp:Literal ID="postedLiteral" runat="server" /></span> </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</div>

<div id="latestcomments" class="sidemenu recent-comments" runat="server">
    <h4 class="header">Latest Comments</h4>
    <ul>
        <asp:Repeater ID="commentsrepeater" runat="server" OnItemDataBound="Comments_OnItemDataBound">
            <ItemTemplate>
                <li id="postli" runat="server"><a href='#' id="commentsanchor" runat="server"></a>
                    <br />
                    <cite><span id="namespan" runat="server"></span></cite>
                </li>    
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</div>

<div id="favlinks" class="sidemenu" runat="server">
    <h4 class="header">Links</h4>
    <ul>
        <asp:Repeater ID="linksRepeater" runat="server" OnItemDataBound="Links_OnItemDataBound">
            <ItemTemplate>
                <li id="postli" runat="server">
                    <a href='#' id="linksanchor" runat="server"></a>
                </li>    
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</div>
