<%@ Page Language="C#" MasterPageFile="~/templates/system/system.master" AutoEventWireup="true" 
    Inherits="Jardalu.Ratna.Web.Pages.articlecollection" %>

<%@ Register TagPrefix="Ratna" TagName="SimplePager" Src="~/Controls/Pagers/SimplePager.ascx" %>

<asp:Content ID="header" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="contentPlaceHolder" ContentPlaceHolderID="contentPlaceHolder" runat="server">

<div id="articlesdiv" runat="server">
    
    <div id="noArticleFoundDiv" runat="server" visible="false">
         <%= ResourceManager.GetLiteral("Common.NotFound")%>
    </div>

</div>

<div style="width: 100%">
    <!-- pager -->
    <Ratna:SimplePager id="Pager" runat="server" numberofpages="10" pagesize="4" />
</div>

</asp:Content>
