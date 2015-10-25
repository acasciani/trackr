using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackrModels;
using Trackr.Utils;

namespace Trackr
{
    public class Page : System.Web.UI.Page
    {
        public WebUser CurrentUser
        {
            get
            {
                return PageExtensions.GetCurrentWebUser();
            }
        }
    }
}