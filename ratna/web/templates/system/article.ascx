<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="article.ascx.cs" Inherits="Jardalu.Ratna.Web.templates.system.article" %>
<h2 style="margin-bottom:10px;"><%= Article.Title %></h2>
<div style="margin-bottom:10px;">Tags - <%= Article.SerializedTags %></div>
<div>
    <%= BlogArticle.Body %>
</div>
