<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="messagerow.ascx.cs"
    Inherits="Jardalu.Ratna.Web.templates.system.messagerow" %>

<li class="comment_odd" id="commentli" runat="server">
<article>
    <header>
    <figure><img runat="server" id="avatar" src="" width="32" height="32" alt=""></figure>
    <address>
    <%= Name %>
    </address>
    
    <time runat="server" id="utctime"><asp:Literal runat="server" ID="timeformatted"></asp:Literal></time>
    </header>
    <section>
    <p>
        <%= Message %>
    </p>
    </section>
</article>
</li>
