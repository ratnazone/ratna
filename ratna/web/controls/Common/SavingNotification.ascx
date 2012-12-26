<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SavingNotification.ascx.cs"
    Inherits="Jardalu.Ratna.Web.Controls.Common.SavingNotification" %>

<div id="savingnotification" class="grid_15" runat="server">
    <div id="progressMode" style="display: none">
        <%= ResourceManager.GetLiteral("Admin.Profile.Edit.Saving")%>
    </div>
    <div id="errorMode" style="display: none">
        <%= ResourceManager.GetLiteral("Admin.Profile.Edit.SavingFailed")%>
        <span id="errorModeMessage"></span>
        <span style="float:right"><img id="savingnotificationclosebutton" src="/images/close-button.png" 
            alt='<%= ResourceManager.GetLiteral("Admin.Common.Close")%>' 
            title='<%= ResourceManager.GetLiteral("Admin.Common.Close")%>'/></span>
    </div>
    <div id="successMode" style="display: none">
        <%= ResourceManager.GetLiteral("Admin.Profile.Edit.Saved")%>
        <span id="successModeMessage"></span>
    </div>
</div>
