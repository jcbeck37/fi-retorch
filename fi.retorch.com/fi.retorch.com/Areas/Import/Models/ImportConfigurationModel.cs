using System.ComponentModel.DataAnnotations;
using System.Web;

namespace fi.retorch.com.Areas.Import.Models
{
    public class ImportConfigurationModel
    {
        [Display(Name = "Account")]
        public int AccountId { get; set; }

        [Display(Name = "First Line Contains Column Names")]
        public bool HasHeaderRow { get; set; }

        [Display(Name = "File")]
        public HttpPostedFileBase ImportFile { get; set; }
    }
}