<%@ Page MasterPageFile="~/templates/system/system.master" Language="C#" AutoEventWireup="true"
    Inherits="Jardalu.Ratna.Web._default" %>

<%@ Register TagPrefix="Ratna" TagName="SimplePager" Src="~/Controls/Pagers/SimplePager.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="contentPlaceHolder" ContentPlaceHolderID="contentPlaceHolder" runat="server">
    <div id="articlesdiv" runat="server">
        <div id="noArticleFoundDiv" runat="server">
            <%= ResourceManager.GetLiteral("Common.NotFound") %>
        </div>
    </div>
    <div style="width: 100%">
        <!-- pager -->
        <Ratna:SimplePager id="Pager" runat="server" numberofpages="10" pagesize="4" />
    </div>
</asp:Content>
