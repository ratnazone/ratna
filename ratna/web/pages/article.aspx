<%@ Page MasterPageFile="~/templates/system/system.master" Language="C#" AutoEventWireup="true"
    Inherits="Jardalu.Ratna.Web.Pages.ArticlePage" %>

<%@ Register TagPrefix="Ratna" TagName="Thread" Src="~/Controls/Common/Thread.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <title><%= Article.Title %></title>    
</asp:Content>

<asp:Content ID="contentPlaceHolder" ContentPlaceHolderID="contentPlaceHolder" runat="server">
    <div id="noArticleFoundDiv" runat="server">
        <!-- error case -->
        Nothing found.
    </div>
    <div id="main" runat="server">

        <!-- article control insert -->

        <!-- comments -->

    </div>
</asp:Content>
