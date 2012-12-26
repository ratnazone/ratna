<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Notification.ascx.cs" 
    Inherits="Jardalu.Ratna.Web.Controls.Common.Notification" %>

<div id="notification" style="display: none;" runat="server">
    <div class="notification">
        <span id="message">&nbsp;</span>
        <span style="float:right"><img id="notificationclosebutton" src="/images/close-button.png" 
            alt='<%= ResourceManager.GetLiteral("Admin.Common.Close")%>' 
            title='<%= ResourceManager.GetLiteral("Admin.Common.Close")%>'/></span>
    </div>
</div>
