<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.ascx.cs" Inherits="Trackr.Source.Wizards.UserManagement" %>

<asp:CreateUserWizard runat="server" ID="UserWizard" LoginCreatedUser="false" OnCreatedUser="UserWizard_CreatedUser">
    <CreateUserButtonStyle CssClass="btn btn-default" />

    <StepNavigationTemplate>
        <div class="col-sm-12 text-right">
            <asp:Button runat="server" CommandName="MovePrevious" Text="Previous" CssClass="btn btn-default" />
            <asp:Button runat="server" CommandName="MoveNext" Text="Save & Continue" CssClass="btn btn-default" />
        </div>
    </StepNavigationTemplate>
    <StartNavigationTemplate>
        <div class="col-sm-12 text-right">
            <asp:Button runat="server" CommandName="MoveNext" Text="Save & Continue" CssClass="btn btn-default" />
        </div>
    </StartNavigationTemplate>
    <FinishNavigationTemplate>
        <div class="col-sm-12 text-right">
            <asp:Button runat="server" CommandName="MovePrevious" Text="Previous" CssClass="btn btn-default" />
            <asp:Button runat="server" CommandName="MoveComplete" Text="Save & Finish" CssClass="btn btn-default" />
        </div>
    </FinishNavigationTemplate>


    <LayoutTemplate>
        <div class="row">
            <div class="col-sm-12">
                <asp:PlaceHolder runat="server" ID="wizardStepPlaceholder"></asp:PlaceHolder>
            </div>
        </div>

        <div class="row">
            <asp:PlaceHolder runat="server" ID="navigationPlaceholder"></asp:PlaceHolder>
        </div>
    </LayoutTemplate>
    
    <WizardSteps>
        <asp:CreateUserWizardStep runat="server" ID="Step1_Create">
            <ContentTemplate>
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-4 control-label">Email</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" ID="UserName" placeholder="e.g. jsmith@gmail.com" TextMode="Email" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ID="validatorEmailRequired" ControlToValidate="UserName" CssClass="text-danger" ErrorMessage="A valid email is required." />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-4 control-label">Password</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                            <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="Password" ID="validatorPasswordRegex" ValidationExpression="^[\s\S]{5,8}$" runat="server" ErrorMessage="The password must be at least 8 characters long." CssClass="text-danger" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-4 control-label">Confirm Password</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" ID="txtPasswordConfirm" TextMode="Password" CssClass="form-control" />
                            <asp:CompareValidator runat="server" ControlToCompare="txtPasswordConfirm" ControlToValidate="Password" CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
                        </div>
                    </div>

                    <asp:TextBox runat="server" ID="Email" Visible="false" />
                </div>
            </ContentTemplate>
        </asp:CreateUserWizardStep>


        <asp:WizardStep runat="server" ID="Step1_Edit" OnDeactivate="Step1_Edit_Deactivate" StepType="Start">
            <div class="form-horizontal">
                <div class="form-group">
                    <label for="<%=txtEmail.ClientID %>" class="col-sm-4 control-label">Email</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtEmail" placeholder="e.g. jsmith@gmail.com" TextMode="Email" CssClass="form-control" />
                        <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorEmailRequired" ControlToValidate="txtEmail" CssClass="text-danger" ErrorMessage="A valid email is required." />
                    </div>
                </div>
            </div>
        </asp:WizardStep>


        <asp:WizardStep runat="server" ID="Step2_Personal" OnDeactivate="Step2_Personal_Deactivate">
            <div class="form-horizontal">
                <div class="form-group">
                    <label for="<%=txtFirstName.ClientID %>" class="col-sm-4 control-label">First name</label>
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtFirstName" MaxLength="30" CssClass="form-control" />
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
                        <asp:TextBox runat="server" ID="txtLastName" MaxLength="30" CssClass="form-control" />
                        <asp:RequiredFieldValidator Display="Dynamic" runat="server" ID="validatorLastNameRequired" ControlToValidate="txtLastName" CssClass="text-danger" ErrorMessage="A last name is required." />
                    </div>
                </div>
            </div>
        </asp:WizardStep>


        <asp:WizardStep runat="server" ID="Step3_RoleAssignments" OnDeactivate="Step3_RoleAssignments_Deactivate">
            <asp:GridView runat="server" ID="gvRoleAssignments" AutoGenerateColumns="false" SelectMethod="gvRoleAssignments_GetData" DeleteMethod="gvRoleAssignments_DeleteItem" DataKeyNames="ScopeAssignmentID,ScopeID,ResourceID,RoleID" CssClass="table table-striped">
                <Columns>
                    <asp:BoundField DataField="PermissionName" HeaderText="Permission" />
                    <asp:BoundField DataField="RoleName" HeaderText="Role" />
                    <asp:BoundField DataField="ScopeType" HeaderText="Scope Type" />
                    <asp:BoundField DataField="ScopeValue" HeaderText="Scoped To" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CSSClass="glyphicon glyphicon-trash" CommandName="Delete" ToolTip="Remove role assignment"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:LinkButton runat="server" ID="lnkAddRoleAssignment" OnClick="lnkAddRoleAssignment_Click" ToolTip="Add role assignment">
                <span class="glyphicon glyphicon-plus-sign"></span> Add Role Assignment
            </asp:LinkButton>

            <asp:Panel runat="server" ID="pnlAddRoleAssignment" Visible="false">
                <div class="form-horizontal">
                    <div class="form-group">
                        <label for="<%=ddlRole.ClientID %>" class="col-sm-2 control-label">Role</label>
                        <div class="col-sm-10">
                            <asp:DropDownList runat="server" ID="ddlRole" CssClass="form-control" AppendDataBoundItems="true">
                                <asp:ListItem Text="-- Select a Role --"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="<%=ddlPermission.ClientID %>" class="col-sm-2 control-label">Permission</label>
                        <div class="col-sm-10">
                            <asp:DropDownList runat="server" ID="ddlPermission" CssClass="form-control" AppendDataBoundItems="true">
                                <asp:ListItem Text="-- Select a Permission --"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="<%=chkDenyFlag.ClientID %>" class="col-sm-2 control-label">Deny</label>
                        <div class="col-sm-10">
                            <asp:CheckBox runat="server" ID="chkDenyFlag" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="<%=ddlRole.ClientID %>" class="col-sm-2 control-label">Scope</label>
                        <div class="col-sm-5">
                            <asp:DropDownList runat="server" ID="ddlScopeType" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlScopeType_SelectedIndexChanged">
                                <asp:ListItem Text="-- Select a Scope Type --"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-5">
                            <asp:DropDownList runat="server" ID="ddlScopeValue" CssClass="form-control" AppendDataBoundItems="false" />
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-sm-5 col-sm-offset-2">
                            <asp:Button Text="Add Assignment" CssClass="btn btn-default" runat="server" ID="btnAddAssignment" OnClick="btnAddAssignment_Click" />
                        </div>
                    </div>
                </div>


            </asp:Panel>
        </asp:WizardStep>

    </WizardSteps>
</asp:CreateUserWizard>