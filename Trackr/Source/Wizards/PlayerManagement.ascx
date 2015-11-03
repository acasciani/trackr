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

        <asp:WizardStep runat="server" ID="Step2_Picture" StepType="Step" Title="Player Passes">
            <asp:GridView runat="server" ID="gvPlayerPasses" AutoGenerateColumns="false" SelectMethod="gvPlayerPasses_GetData" EmptyDataText="This player does not have any player passes." CssClass="table table-striped table-hover" 
                OnRowEditing="gvPlayerPasses_RowEditing" OnRowCancelingEdit="gvPlayerPasses_RowCancelingEdit">
                <Columns>
                    <asp:BoundField DataField="Expiration" HeaderText="Expiration" DataFormatString="{0:MM/dd/yyyy}" ReadOnly="true" />
                    <asp:BoundField DataField="PassNumber" HeaderText="Pass Number" ReadOnly="true" />

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkRemovePlayerPass" Visible='<%# (bool)Eval("Editable") %>' ToolTip="Remove player pass" CssClass="glyphicon glyphicon-trash"></asp:LinkButton>&nbsp;&nbsp;
                            <asp:LinkButton runat="server" ID="lnkEditPlayerPass" Visible='<%# (bool)Eval("Editable") %>' CommandName="Edit" ToolTip="Edit player pass" CssClass="glyphicon glyphicon-edit"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="lnkViewPlayerPass" Visible='<%# !(bool)Eval("Editable") %>' CommandArgument='<%# Eval("PlayerPassID") %>' ToolTip="View player pass" CssClass="glyphicon glyphicon-eye-open"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Panel runat="server" ID="pnlAddEditPass" Visible="false">
                <div class="well">
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="form-group row">
                                <label class="col-sm-12 control-label" for="<%=txtPassExpires.ClientID %>">Pass Expires On</label>
                                <div class="col-sm-12">
                                    <asp:TextBox runat="server" ID="txtPassExpires" TextMode="Date" CssClass="form-control" MaxLength="30" />
                                    <asp:CustomValidator runat="server" ID="validatorPlayerPassExpiresValid" CssClass="text-danger" ErrorMessage="The expiration date entered was not in the format: MM/DD/YYYY" OnServerValidate="validatorDOBParses_ServerValidate" Display="Dynamic" />
                                </div>
                            </div>

                            <div class="form-group row">
                                <label class="col-sm-12 control-label" for="<%=uploadPlayerPass.ClientID %>">Player Pass Photo</label>
                                <div class="col-sm-12">
                                    <asp:FileUpload runat="server" ID="uploadPlayerPass" AllowMultiple="false" />
                                </div>
                            </div>

                            <div class="form-group row">
                                <div class="col-sm-12">
                                    <p>Once you select the player's photo, click <strong>Preview Photo</strong>. Once you are satisfied with the preview, click <strong>Save Player Pass</strong>.</p>                                    
                                </div>
                                
                                <div class="col-sm-12">
                                    <asp:LinkButton runat="server" ID="lnkUpload" OnClick="btnUpload_Click" CausesValidation="false"><i class="glyphicon glyphicon-upload"></i> Preview Photo</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:LinkButton runat="server" ID="lnkReloadImage" OnClick="lnkReloadImage_Click" CausesValidation="false"><i class="glyphicon glyphicon-refresh"></i> Reload Existing</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:LinkButton runat="server" ID="lnkSavePlayerPass" Text="Save Pass" CausesValidation="true"><i class="glyphicon glyphicon-save"></i> Save Player Pass</asp:LinkButton>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-group row" runat="server" id="divPreview" visible="false">
                                <div class="col-sm-12 text-right"><strong>Preview:</strong></div>
                                <div class="col-sm-12 text-right">
                                    <asp:Image runat="server" ID="imgUploadPreview" CssClass="img-responsive" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:WizardStep>

        <asp:WizardStep runat="server" ID="Step3_Teams" StepType="Finish" Title="Teams">
            <asp:GridView runat="server" ID="gvTeamAssignments" AutoGenerateColumns="false" SelectMethod="gvTeamAssignments_GetData" EmptyDataText="This player is not assigned to any teams.">
                <Columns>
                    <asp:BoundField DataField="Season" HeaderText="Season" />
                    
                    <asp:TemplateField HeaderText="Team">
                        <ItemTemplate>
                            <%# Eval("TeamName") %> (<%# Eval("ProgramName") %>)
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Is Secondary">
                        <ItemTemplate>
                            <%# (bool)Eval("IsSecondary") ? "Yes" : "" %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkRemoveFromTeam" Visible='<%# Eval("dasf") %>' ToolTip="Remove from team">D</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Panel runat="server" ID="pnlTeamAssignment" Visible="false">
                <hr />

            </asp:Panel>
        </asp:WizardStep>

        <asp:WizardStep StepType="Complete" Title="Complete">
            Successfully finished editing player. <asp:LinkButton runat="server" ID="lnkEditAgain" OnClick="lnkEditAgain_Click">Continue editing this player</asp:LinkButton> or <a href="Default.aspx">view all players</a>.
        </asp:WizardStep>

    </WizardSteps>
</asp:Wizard>