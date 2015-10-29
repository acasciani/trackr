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

                if (UserManagement.PrimaryKey.HasValue)
                {
                    return UserManagement.PrimaryKey.Value;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            UserManagement.PrimaryKey = WebUserID;
        }


    }
}