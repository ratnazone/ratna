<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="articlesummary.ascx.cs"
    Inherits="Jardalu.Ratna.Web.templates.system.articlesummary" %>
<div style="border:1px solid silver;padding:10px;margin-bottom:10px;">
    <h3 style="margin-bottom:10px;"><a href="<%=Article.UrlKey %>"><%= Article.Title %></a>
    <span style="font-size:10px;margin-left:10px;">(Posted : <%=Article.PublishedDate.ToString("yyyy/MM/dd") %>)</span>
    <span style="font-size:10px;margin-left:10px;">(Comments : <%=CommentCount %>)</span>
    </h3>
    <div>
        <img id="blogimg" runat="server" visible="false" style="width:80px;height:80px;float:left;margin-right:10px;" />
        <%= BlogArticle.Summary %>&nbsp;<a href="<%=Article.UrlKey %>">Read More &raquo;</a>
        <div style="clear:both">&nbsp;</div>
    </div>
</div>
