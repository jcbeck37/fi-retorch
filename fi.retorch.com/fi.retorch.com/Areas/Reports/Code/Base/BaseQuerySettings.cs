using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fi.retorch.com.Areas.Reports.Code.Base
{
    public class BaseQuerySettings
    {
        public int? PageSize { get; set; }
        public int? Page { get; set; }
        public string Sort { get; set; }
        public string Search { get; set; }
    }
}