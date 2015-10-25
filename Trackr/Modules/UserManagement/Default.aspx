<%@ Page Title="User Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Trackr.Modules.UserManagement.Default" %>

<%@ Register Src="~/Source/Wizards/UserManagement.ascx" TagName="UserManagement" TagPrefix="ui" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <ui:UserManagement runat="server" ID="UserManagement" />

</asp:Content>
