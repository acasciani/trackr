using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackrModels;

namespace Trackr.Source.Wizards
{
    public partial class UserManagement : WizardBase<int>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }
        }

        private void Populate_Create()
        {

        }

        private void Populate_Edit()
        {
            using (WebUsersController wuc = new WebUsersController())
            {
                WebUser wu = wuc.GetScopedEntity(CurrentUser.UserID, Permissions.UserManagement.EditUser, PrimaryKey.Value);
                Username.Text = wu.Email;
                FirstName.Text = "";
            }
        }

        
    }
}