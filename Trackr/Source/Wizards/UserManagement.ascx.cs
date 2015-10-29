using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;
using Trackr.Utils;
using System.Web.Security;

namespace Trackr.Source.Wizards
{
    public partial class UserManagement : WizardBase<int>
    {
        [Serializable]
        private class ScopeAssignmentResult
        {
            public string RoleName { get; set; }
            public string ScopeType { get; set; }
            public string ScopeValue { get; set; }
            public int? ScopeAssignmentID { get; set; }
            public int ScopeID { get; set; }
            public int ResourceID { get; set; }
            public int RoleID { get; set; }
        }

        private List<ScopeAssignmentResult> ScopeAssignmentResults
        {
            get { return ViewState["ScopeAssignments"] as List<ScopeAssignmentResult>; }
            set { ViewState["ScopeAssignments"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            if (IsNew)
            {
                UserWizard.ActiveStepIndex = 0;
                Populate_Create();
            }
            else
            {
                UserWizard.ActiveStepIndex = 1;
                Populate_Edit();
            }

            // update dropdownlists
            ddlRole.Populate(DropDownType.Role);
            ddlScopeType.Populate(DropDownType.ScopeType);
        }

        private void Populate_Create()
        {
            UserWizard.ActiveStepIndex = 0;
        }

        private void Populate_Edit()
        {
            using (WebUsersController wuc = new WebUsersController())
            {
                WebUser wu = wuc.GetScopedEntity(CurrentUser.UserID, Permissions.UserManagement.EditUser, PrimaryKey.Value);
                txtEmail.Text = wu.Email;
            }
        }

        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
        public IQueryable gvRoleAssignments_GetData()
        {
            if (ScopeAssignmentResults == null)
            {
                if (IsNew)
                {
                    ScopeAssignmentResults = new List<ScopeAssignmentResult>();
                }
                else
                {
                    using (ScopeAssignmentsController sac = new ScopeAssignmentsController())
                    {
                        FetchStrategy fetch = new FetchStrategy();
                        fetch.LoadWith<ScopeAssignment>(i => i.Role);
                        fetch.LoadWith<ScopeAssignment>(i => i.Scope);

                        ScopeAssignmentResults = sac.GetWhere(i => i.UserID == PrimaryKey.Value, fetch).Select(i => new ScopeAssignmentResult()
                        {
                            RoleName = i.Role.RoleName,
                            ScopeAssignmentID = i.ScopeAssignmentID,
                            ScopeType = i.Scope.ScopeName,
                            ScopeValue = sac.GetScopeValueDisplay(i.ScopeID, i.ResourceID)
                        }).ToList();
                    }
                }
            }

            return ScopeAssignmentResults.AsQueryable();
        }
       
        // The id parameter name should match the DataKeyNames value set on the control
        public void gvRoleAssignments_DeleteItem(int? ScopeAssignmentID, int ScopeID, int ResourceID, int RoleID)
        {
            if (ScopeAssignmentID.HasValue)
            {
                using (ScopeAssignmentsController sac = new ScopeAssignmentsController())
                {
                    HttpResponseMessage response = sac.Delete(ScopeAssignmentID.Value);
                    if (response.IsSuccessStatusCode)
                    {
                        //YES
                        ScopeAssignmentResults.RemoveAll(i => i.ScopeAssignmentID == ScopeAssignmentID.Value);
                    }
                    else
                    {
                        //no
                    }
                }
            }
            else
            {
                ScopeAssignmentResults.RemoveAll(i => i.ScopeID == ScopeID && i.ResourceID == ResourceID && i.RoleID == RoleID);
            }
        }

        protected void lnkAddRoleAssignment_Click(object sender, EventArgs e)
        {
            pnlAddRoleAssignment.Visible = true;
        }

        protected void ddlScopeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int scopeID;
            if (int.TryParse(ddlScopeType.SelectedValue, out scopeID))
            {
                ddlScopeValue.Populate_ScopeValues(scopeID, retainIndices: new int[] { 0 });
                ddlScopeValue.Focus();
            }
        }

        protected void btnAddAssignment_Click(object sender, EventArgs e)
        {
            int roleID, scopeID, resourceID;

            if (int.TryParse(ddlRole.SelectedValue, out roleID) && int.TryParse(ddlScopeType.SelectedValue, out scopeID) && int.TryParse(ddlScopeValue.SelectedValue, out resourceID))
            {
                ScopeAssignmentResults.Add(new ScopeAssignmentResult()
                {
                    ResourceID = resourceID,
                    RoleID = roleID,
                    ScopeID = scopeID,
                    RoleName = ddlRole.SelectedItem.Text,
                    ScopeType = ddlScopeType.SelectedItem.Text,
                    ScopeValue = ddlScopeValue.SelectedItem.Text
                });

                gvRoleAssignments.DataBind();
            }
        }

        protected void Step1_Edit_Deactivate(object sender, EventArgs e)
        {
            MembershipUser user = Membership.GetUser(PrimaryKey.Value);

            if (user.UserName != txtEmail.Text && Membership.GetUser(txtEmail.Text) == null)
            {
                using (WebUsersController wuc = new WebUsersController())
                {
                    WebUser webUser = wuc.Get(PrimaryKey.Value);
                    webUser.Email = txtEmail.Text;
                    wuc.Update(webUser);
                }
            }
        }

        protected void Step2_Personal_Deactivate(object sender, EventArgs e)
        {
            using (WebUserInfosController wuic = new WebUserInfosController())
            {
                WebUserInfo info = wuic.GetWhere(i => i.UserID == PrimaryKey.Value).FirstOrDefault();

                if (info == null)
                {
                    // no info, so add one
                    info = new WebUserInfo()
                    {
                        FName = txtFirstName.Text,
                        LName = txtLastName.Text,
                        MInitial = string.IsNullOrWhiteSpace(txtMiddleInitial.Text) ? (char?)null : (char)txtMiddleInitial.Text.ToCharArray(0,1)[0],
                        UserID = PrimaryKey.Value
                    };
                    wuic.AddNew(info);
                }
                else
                {
                    info.FName = txtFirstName.Text;
                    info.LName = txtLastName.Text;
                    info.MInitial = string.IsNullOrWhiteSpace(txtMiddleInitial.Text) ? (char?)null : (char)txtMiddleInitial.Text.ToCharArray(0, 1)[0];
                    wuic.Update(info);
                }
            }
        }

        protected void UserWizard_CreatedUser(object sender, EventArgs e)
        {
            MembershipUser user = Membership.GetUser(((CreateUserWizard)sender).UserName);
            if (user != null)
            {
                PrimaryKey = (int)user.ProviderUserKey;
            }
        }


    }
}