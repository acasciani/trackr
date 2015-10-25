<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.ascx.cs" Inherits="Trackr.Source.Wizards.UserManagement" %>


<asp:CreateUserWizard runat="server">
    <WizardSteps>
        <asp:CreateUserWizardStep runat="server" ID="Step1_Create" />
        <asp:WizardStep runat="server" ID="Step1_Edit">
            <asp:TextBox runat="server" ID="Username" placeholder="Email" TextMode="Email" />
            <asp:TextBox runat="server" ID="Password" placeholder="Password" TextMode="Password" />
            <asp:TextBox runat="server" ID="PasswordConfirm" placeholder="Confirm password" TextMode="Password" />
        </asp:WizardStep>


        <asp:WizardStep runat="server" ID="Step2_Personal">
            <asp:TextBox runat="server" ID="FirstName" placeholder="First name" />
        </asp:WizardStep>

    </WizardSteps>
</asp:CreateUserWizard>