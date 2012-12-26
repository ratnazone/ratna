<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmDialog.ascx.cs" Inherits="Jardalu.Ratna.Web.Controls.Common.ConfirmDialog" %>
<%@ Register TagPrefix="Ratna" TagName="ClientJavaScript" Src="~/Controls/Common/ClientJavaScript.ascx" %>
 <link rel="stylesheet" href='<%=ResolveUrl("~/external/simplemodal/confirm.css")%>' type="text/css" />
<Ratna:ClientJavaScript id="clientJavaScript" runat="server" />


<!-- modal content -->
<div id='confirm'>
	<div class='header'><span><%= ResourceManager.GetLiteral("Confirm.Heading")%></span></div>
	<div class='message'></div>
	<div class='buttons'>
		<div class='no simplemodal-close'><%= ResourceManager.GetLiteral("Confirm.No")%></div>
        <div class='yes'><%= ResourceManager.GetLiteral("Confirm.Yes")%></div>
	</div>
</div>