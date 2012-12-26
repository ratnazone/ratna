<%@ Page Title="" Language="C#" MasterPageFile="~/templates/system/system.master" AutoEventWireup="true" 
    CodeBehind="mediacollection.aspx.cs" Inherits="Jardalu.Ratna.Web.Pages.mediacollection" %>

<%@ Register TagPrefix="Ratna" TagName="SimplePager" Src="~/Controls/Pagers/SimplePager.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="contentPlaceHolder" ContentPlaceHolderID="contentPlaceHolder" runat="server">

<div id="mediadiv" runat="server">
    
    <div id="noMediaFoundDiv" runat="server" visible="false">
         <%= ResourceManager.GetLiteral("Admin.Common.NotFound")%>
    </div>

    <h2 class="mediacollection_header" ><asp:Literal runat="server" id="TitleHeader"/></h2>

    <asp:Repeater runat="server" ID="imageRepeater">
        <ItemTemplate>
            <img style="width:120px;height:120px" src='<%# Container.DataItem %>' />
        </ItemTemplate>
    </asp:Repeater>

</div>

<div style="width: 100%">
    <!-- pager -->
    <Ratna:SimplePager id="Pager" runat="server" numberofpages="10" pagesize="16" />
</div>

</asp:Content>
