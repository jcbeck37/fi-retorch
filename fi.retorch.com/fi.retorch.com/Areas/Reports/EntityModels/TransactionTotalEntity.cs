using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fi.retorch.com.Areas.Reports.EntityModels
{
    public class TransactionTotalEntity
    {
        public int AccountId { get; set; }
        public decimal Total { get; set; }
    }
}