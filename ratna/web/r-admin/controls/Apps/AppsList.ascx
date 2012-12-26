<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppsList.ascx.cs" Inherits="Jardalu.Ratna.Web.Admin.controls.Apps.AppsList" %>

<%@ Register TagPrefix="Admin" TagName="AppListItem" Src="~/r-admin/controls/Apps/AppListItem.ascx" %>

<div class="box" runat="server" id="appsListDiv">

<span id="headerspan" runat="server">
        <h2>
        <%= Parameters.Header %>
        </h2>
    </span>

    <asp:Repeater ID="repeater" runat="server" OnItemDataBound="RepeaterItemEventHandler">
        <ItemTemplate>
            <Admin:AppListItem runat="server" id="appListItem"/>
        </ItemTemplate>
    </asp:Repeater>

    <div id="noapps" runat="server" visible="false" style="padding:5px">
       <%= Parameters.NothingFoundText %>        
    </div>

</div>
