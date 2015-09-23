using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Trackr;
using TrackrModels;

namespace Trackr.Modules.UserManagement
{
    public partial class Default : Page
    {
        private int? WebUserID
        {
            get
            {
                int userID;
                
                // try view state first, then query string
                if (ViewState["WebUserID"] as int? != null)
                {
                    return ViewState["WebUserID"] as int?;
                }

                if (int.TryParse((Request.QueryString["wid"] ?? "").ToString(), out userID))
                {
                    return userID;
                }
                else
                {
                    // unable to get user id
                    return null;
                }
            }
        }

        public string Email
        {
            get { return ViewState["Email"] as string ?? ""; }
            set { ViewState["Email"] = value; }
        }
        







        protected void Page_Load(object sender, EventArgs e)
        {
            if (WebUserID.HasValue)
            {
                LoadUserData(WebUserID.Value);
            }
        }

        private void LoadUserData(int userID)
        {
            using (WebUsersController wuc = new WebUsersController())
            {
                var info = wuc.Get(userID);
                Email = info.Email;
            }
        }


        #region step 1, login information
        protected void Step1_Activate(object sender, EventArgs e)
        {

        }

        protected void Step1_Deactivate(object sender, EventArgs e)
        {
            
        }

        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            Email = ((TextBox)sender).Text;
        }
        #endregion



    }
}