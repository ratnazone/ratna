<%@ Page Title="" Language="C#" MasterPageFile="~/templates/system/system.master" AutoEventWireup="true" CodeBehind="search.aspx.cs" 
    Inherits="Jardalu.Ratna.Web.pages.search" %>

<%@ Register TagPrefix="Ratna" TagName="SimplePager" Src="~/Controls/Pagers/SimplePager.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src='<%=ResolveUrl("~/external/jquery/1.6.2/jquery-1.6.2.min.js")%>'></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            Setup();
        });

        function Setup() {
            $("#__ratna_search_b").click(function () {
                ____SearchButtonClicked();
            });
        }

        function ____SearchButtonClicked() {
            var q = $("#__ratna_search_q").val();
            if (q != null && q != "") {
                window.location = "/search/" + q;
            }
        }

    </script>
</asp:Content>

<asp:Content ID="contentPlaceHolder" ContentPlaceHolderID="contentPlaceHolder" runat="server">
    <div style="margin-bottom:10px;">
        <input type="text" class="searchtext" id="__ratna_search_q" runat="server"/>
        <input type="button" class="searchbutton" id="__ratna_search_b" value="<%= ResourceManager.GetLiteral("Common.Search")%>"/>
    </div>

    <div id="nosearchfound" runat="server" visible="false">
        No search found.
    </div>

    <div id="searchcontent">

        <div id="photosdiv" runat="server">
            <h4 style="border-bottom:1px dotted silver;">Photos 
                <small>(<a runat="server" id="photosAllAnchor" href="/search/"><asp:Literal runat="server" ID="photosCount"></asp:Literal> results</a>)</small>
            </h4>
        </div>

        <div id="articlesdiv" runat="server">
            <h4 style="border-bottom:1px dotted silver;">Posts 
                <small>(<a runat="server" id="articlesAllAnchor" href="/search/"><asp:Literal runat="server" ID="articlescount"></asp:Literal> results</a>)</small>
            </h4>
        </div>
    </div>

    <div style="width: 100%">
        <!-- pager -->
        <Ratna:SimplePager id="Pager" runat="server" numberofpages="10" pagesize="4" />
    </div>

</asp:Content>
