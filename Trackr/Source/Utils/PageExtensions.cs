using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackrModels;

namespace Trackr.Utils
{
    public static class PageExtensions
    {
        public static WebUser GetCurrentWebUser()
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                throw new Exception("User not authenticated");
            }

            string username = HttpContext.Current.User.Identity.Name;

            WebUser user = HttpContext.Current.Session["CurrentWebUser"] as WebUser;

            if (user != null && user.Email == username)
            {
                return user;
            }
            else
            {
                using (WebUsersController wuc = new WebUsersController())
                {
                    HttpContext.Current.Session["CurrentWebUser"] = wuc.GetWhere(i => i.Email == username).First();
                    return HttpContext.Current.Session["CurrentWebUser"] as WebUser;
                }
            }
        }

    }
}