using System.ComponentModel.DataAnnotations;

namespace fi.retorch.com.Areas.Import.Models
{
    public class ImportProcessModel
    {
        [Display(Name = "Account")]
        public string AccountName { get; set; }
        public bool IsDebt { get; set; }

        public string CreditText { get; set; }
        public string DebitText { get; set; }

        [Display(Name = "Imported Records")]
        public int ImportedRecords { get; set; }

        [Display(Name = "Interest Records")]
        public int InterestRecords { get; set; }

        [Display(Name = "Skipped Records")]
        public int SkippedRecords { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal CurrentAccountBalance { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal CreditTotal { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal DebitTotal { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal NewAccountBalance { get; set; }

        public ImportProcessModel ()
        {
            ImportedRecords = 0;
            InterestRecords = 0;
            SkippedRecords = 0;

            CurrentAccountBalance = 0;
            CreditTotal = 0;
            DebitTotal = 0;
            NewAccountBalance = 0;
        }
    }
}