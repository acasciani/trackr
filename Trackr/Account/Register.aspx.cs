using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using TrackrModels;
using System.Web.Security;

namespace Trackr.Account
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Email.Text = "test";
            Password.Text = "test";
            MembershipCreateStatus status;
            Membership.CreateUser(Email.Text, Password.Text, Email.Text, "how old is your dog?", "13", true, null, out status);
        }

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            
        }
    }
}