<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlayerManagement.ascx.cs" Inherits="Trackr.Source.Wizards.PlayerManagement" %>

<ui:AlertBox runat="server" ID="AlertBox" />

<asp:Wizard runat="server" ID="PlayerWizard" DisplaySideBar="true" OnNextButtonClick="PlayerWizard_NextButtonClick">
    <StepNavigationTemplate>
        <div class="text-right">
            <asp:Button runat="server" CommandName="MovePrevious" Text="Previous" CssClass="btn btn-default" CausesValidation="false" />
            <asp:Button runat="server" CommandName="MoveNext" Text="Save & Continue" CssClass="btn btn-default" />
        </div>
    </StepNavigationTemplate>
    <StartNavigationTemplate>
        <div class="text-right" runat="server">
            <asp:Button runat="server" ID="btnStart" CommandName="MoveNext" Text='Save & Continue' CssClass="btn btn-default" />
        </div>
    </StartNavigationTemplate>
    <FinishNavigationTemplate>
        <div class="text-right">
            <asp:Button runat="server" CommandName="MovePrevious" Text="Previous" CssClass="btn btn-default" CausesValidation="false" />
            <asp:Button runat="server" CommandName="MoveComplete" Text="Save & Finish" CssClass="btn btn-default" />
        </div>
    </FinishNavigationTemplate>


    <SideBarTemplate>
        <asp:ListView runat="server" ID="sideBarList">
            <LayoutTemplate>
                <ol class="breadcrumb">
                    <asp:PlaceHolder runat="server" ID="ItemPlaceHolder" />
                </ol>
            </LayoutTemplate>

            <ItemTemplate>
                <li><asp:LinkButton runat="server" ID="sideBarButton" CausesValidation="false"></asp:LinkButton></li>
            </ItemTemplate>
            <SelectedItemTemplate>
                <li class="active"><asp:LinkButton runat="server" ID="sideBarButton" CausesValidation="false"></asp:LinkButton></li>
            </SelectedItemTemplate>
        </asp:ListView>
    </SideBarTemplate>


    <LayoutTemplate>
        <div>
            <asp:PlaceHolder runat="server" ID="sideBarPlaceholder"></asp:PlaceHolder>
        </div>

        <div>
            <asp:PlaceHolder runat="server" ID="wizardStepPlaceholder"></asp:PlaceHolder>
        </div>

        <div>
            <asp:PlaceHolder runat="server" ID="navigationPlaceholder"></asp:PlaceHolder>
        </div>
    </LayoutTemplate>
    

    <WizardSteps>
        <asp:WizardStep runat="server" ID="Step1_Info" StepType="Start" Title="Player Information">
            <div class="form-horizontal">
                <div class="form-group">
                    <label for="<%=txtFirstName.ClientID %>" class="col-sm-4 control-label">First name</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtFirstName" CssClass="form-control" MaxLength="30" />
                        <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorFirstNameRequired" ControlToValidate="txtFirstName" CssClass="text-danger" ErrorMessage="A first name is required." />
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%=txtMiddleInitial.ClientID %>" class="col-sm-4 control-label">Middle initial</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtMiddleInitial" MaxLength="1" CssClass="form-control" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%=txtLastName.ClientID %>" class="col-sm-4 control-label">Last name</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtLastName" CssClass="form-control" MaxLength="30" />
                        <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorLastNameRequired" ControlToValidate="txtLastName" CssClass="text-danger" ErrorMessage="A last name is required." />
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%=txtDateOfBirth.ClientID %>" class="col-sm-4 control-label">Date of birth</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtDateOfBirth" CssClass="form-control" MaxLength="30" TextMode="Date" />
                        <asp:CustomValidator Display="Dynamic" runat="server" ID="validatorDOBParses" ControlToValidate="txtDateOfBirth" CssClass="text-danger" ErrorMessage="The date of birth entered was not in the format: MM/DD/YYYY" OnServerValidate="validatorDOBParses_ServerValidate" />
                        <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorDOBRequired" ControlToValidate="txtDateOfBirth" CssClass="text-danger" ErrorMessage="Player's date of birth is required." />
                    </div>
                </div>
            </div>
        </asp:WizardStep>

        <asp:WizardStep runat="server" ID="Step2_Picture" StepType="Step" Title="Pass Photo">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="col-sm-12"><strong>Select a photo for the player's pass.</strong></div>
                </div>

                <div class="form-group">
                    <div class="col-sm-6">
                        <asp:FileUpload runat="server" ID="uploadPlayerPass" AllowMultiple="false" />
                    </div>
                    <div class="col-sm-6 text-right">
                        <asp:LinkButton runat="server" ID="lnkUpload" OnClick="btnUpload_Click">
                            <i class="glyphicon glyphicon-upload"></i> Preview Photo
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="form-group" runat="server" id="divPreview" visible="false">
                    <div class="col-sm-12 text-center">Your preview is below. If you are satisfied with the photo, click "Save & Continue".</div>
                    <div class="col-sm-12 text-center">
                        <asp:Image runat="server" ID="imgUploadPreview" />
                    </div>
                </div>
            </div>
        </asp:WizardStep>

        <asp:WizardStep runat="server" ID="Step3_Teams" StepType="Finish" Title="Teams">
            Assignment support coming soon.
        </asp:WizardStep>

        <asp:WizardStep StepType="Complete" Title="Complete">
            Successfully finished editing player. <asp:LinkButton runat="server" ID="lnkEditAgain" OnClick="lnkEditAgain_Click">Continue editing this player</asp:LinkButton> or <a href="Default.aspx">view all players</a>.
        </asp:WizardStep>

    </WizardSteps>
</asp:Wizard>