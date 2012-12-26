<%@ Page MasterPageFile="~/templates/system/system.master" Language="C#" AutoEventWireup="true"
    Inherits="Jardalu.Ratna.Web.Pages._static" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <asp:Literal ID="pageheadLiteral" runat="server" Text="" />
</asp:Content>


<asp:Content ID="contentPlaceHolder" ContentPlaceHolderID="contentPlaceHolder" runat="server">  
    <div id="noArticleFoundDiv" runat="server">
        <!-- error case -->
        Nothing found.
    </div>
    <div id="articlesDiv" runat="server">
        <!-- article control -->
    </div>
</asp:Content>