using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Trackr.Source.Wizards
{
    public class WizardBase<K> : UserControl where K : struct
    {
        public K? PrimaryKey { get; set; }
        public bool IsNew { get { return !PrimaryKey.HasValue; } }
    }
}