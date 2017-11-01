using fi.retorch.com.Areas.Import.Code.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace fi.retorch.com.Areas.Import.Models
{
    public class ImportPreviewModel
    {
        [Display(Name = "Account")]
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public bool IsDebt { get; set; }

        public string CreditText { get; set; }
        public string DebitText { get; set; }

        [Display(Name = "First Line Contains Column Names")]
        public bool HasHeaderRow { get; set; }

        [Display(Name = "Reverse Positive/Negative")]
        public bool HasInvertedAmounts { get; set; }

        public List<ImportColumn> ImportColumns { get; set; }
        public List<ImportRecord> PreviewRecords { get; set; }

        [Display(Name = "Total Records")]
        public int TotalRecords { get; set; }
        [Display(Name = "Failed Records")]
        public int FailedRecords { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal CurrentAccountBalance { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal CreditTotal { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal DebitTotal { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal NewAccountBalance { get; set; }
        
        public bool CalculateInterest { get; set; }

        public static List<ImportColumn> GenerateColumnsFromHeaders(string[] headers)
        {
            List<ImportColumn> columns = new List<ImportColumn>();

            int sequence = 0;
            foreach (string value in headers)
            {
                ImportColumn colItem = new ImportColumn();

                switch (value)
                {
                    case "Transaction":
                    case "Transaction Description":
                    case "Description":
                        colItem.ColumnType = ColumnTypeEnum.Transaction;
                        break;
                    case "Amount":
                    case "Net Amount":
                        colItem.ColumnType = ColumnTypeEnum.Amount;
                        break;
                    case "AccountBalance":
                    case "Balance":
                    case "Running Balance":
                        colItem.ColumnType = ColumnTypeEnum.AccountBalance;
                        break;
                    case "Credit":
                    case "CreditAmount":
                        colItem.ColumnType = ColumnTypeEnum.Amount;
                        colItem.IsPositive = true;
                        break;
                    case "Debit":
                    case "DebitAmount":
                        colItem.ColumnType = ColumnTypeEnum.Amount;
                        colItem.IsPositive = false;
                        break;
                    case "Date":
                    case "Trans Date":
                    case "Trade Date":
                        colItem.ColumnType = ColumnTypeEnum.TransactionDate;
                        break;
                    case "Post Date":
                    case "Settlement Date":
                        colItem.ColumnType = ColumnTypeEnum.PostedDate;
                        break;
                    case "Category":
                        colItem.ColumnType = ColumnTypeEnum.Category;
                        break;
                    default:
                        colItem.ColumnType = ColumnTypeEnum.Ignore;
                        break;
                }

                colItem.Sequence = sequence;
                columns.Add(colItem);
                sequence++;
            }

            return columns;
        }

        public static List<ImportColumn> GenerateColumnsFromData(string[] data)
        {
            List<ImportColumn> columns = new List<ImportColumn>();

            int sequence = 0;
            foreach (string value in data)
            {
                ImportColumn colItem = new ImportColumn();
                colItem.ColumnType = ColumnTypeEnum.Ignore;

                //string sampleString;
                decimal sampleAmount;
                DateTime sampleDate;
                int sampleInteger;

                if (decimal.TryParse(value, out sampleAmount))
                {
                    // transaction amount, balance
                    if (!columns.Any(c => c.ColumnType == ColumnTypeEnum.Amount))
                    {
                        colItem.ColumnType = ColumnTypeEnum.Amount;
                    }
                    else if (!columns.Any(c => c.ColumnType == ColumnTypeEnum.AccountBalance))
                    {
                        colItem.ColumnType = ColumnTypeEnum.AccountBalance;
                    }
                }
                else if (DateTime.TryParse(value, out sampleDate))
                {
                    // transaction date, posted date
                    if (!columns.Any(c => c.ColumnType == ColumnTypeEnum.TransactionDate))
                    {
                        colItem.ColumnType = ColumnTypeEnum.TransactionDate;
                    }
                    else if (!columns.Any(c => c.ColumnType == ColumnTypeEnum.PostedDate))
                    {
                        colItem.ColumnType = ColumnTypeEnum.PostedDate;
                    }
                }
                else if (int.TryParse(value, out sampleInteger))
                {
                    // may be Type indicator of some sort
                }
                else if (!string.IsNullOrEmpty(value))
                {
                    if (!columns.Any(c => c.ColumnType == ColumnTypeEnum.Transaction))
                    {
                        colItem.ColumnType = ColumnTypeEnum.Transaction;
                    }
                    else  if (!columns.Any(c => c.ColumnType == ColumnTypeEnum.Category))
                    {
                        colItem.ColumnType = ColumnTypeEnum.Category;
                    }
                }
                
                colItem.Sequence = sequence;
                columns.Add(colItem);
                sequence++;
            }

            return columns;
        }
    }

    public class ImportColumn
    {
        public ColumnTypeEnum ColumnType { get; set; }
        public string ColumnName { get { return ColumnType.ToFriendlyString(); } }
        public bool? IsPositive { get; set; }
        public int Sequence { get; set; }

        public ImportValue GetDataDisplay(string data)
        {
            ImportValue display = new ImportValue();
            display.Column = this;
            display.Data = data;

            switch (ColumnType)
            {
                case ColumnTypeEnum.TransactionDate:
                case ColumnTypeEnum.PostedDate:
                    display.ParseDate();
                    break;
                case ColumnTypeEnum.Amount:
                case ColumnTypeEnum.AccountBalance:
                    display.ParseAmount(IsPositive);
                    break;
                default:
                    display.Display = data;
                    display.IsValidValue = true;
                    break;
            }

            return display;
        }
    }

    public class ImportRecord
    {
        public int Sequence { get; set; }
        public bool? IsInterest { get; set; }
        public List<ImportValue> Values { get; set; }

        public bool HasColumnType(ColumnTypeEnum type)
        {
            return Values.Any(v => v.Column.ColumnType == type && v.IsValidValue);
        }

        public ImportValue GetColumnType(ColumnTypeEnum type)
        {
            return Values.FirstOrDefault(v => v.Column.ColumnType == type && v.IsValidValue);
        }
    }

    public class ImportValue
    {
        public ImportColumn Column { get; set; }
        public bool IsValidValue { get; set; }
        public string Data { get; set; }
        public string Display { get; set; }
        public decimal ValidAmount { get; set; }
        public DateTime ValidDate { get; set; }

        public void ParseAmount(bool? isPositive)
        {
            // determine positive/negative value
            int mult = 1;
            if (isPositive.HasValue) // debit or credit field
            {
                if (!isPositive.Value)
                    mult = -1;
            }
            else
            {
                if (Data.IndexOf('(') >= 0)
                    mult = -1;
            }

            // attempt to parse amount
            decimal amount;
            if (decimal.TryParse(Data, out amount))
            {
                IsValidValue = true;
                ValidAmount = mult * amount;
                Display = ValidAmount.ToString("C");
            }
            else
            {
                IsValidValue = false;
            }
        }

        public void ParseDate()
        {
            DateTime? result = InnerParseDate(Data);

            if (result.HasValue)
            {
                IsValidValue = true;
                ValidDate = result.Value;
                Display = Data;
            }
            else if (Data.Trim().IndexOf(" ") > -1)
            {
                // special case for special dates
                // ex. 03/07/2017  Tue

                Data = Data.Trim();
                int space = Data.IndexOf(" ");
                string part1 = Data.Substring(0, space);
                string part2 = Data.Substring(space);

                if (InnerParseDate(part1).HasValue)
                {
                    IsValidValue = true;
                    ValidDate = InnerParseDate(part1).Value;
                    Display = Data;
                } else if (InnerParseDate(part2).HasValue)
                {
                    IsValidValue = true;
                    ValidDate = InnerParseDate(part2).Value;
                    Display = Data;
                } else
                {
                    IsValidValue = false;
                }
            }
            else
            {
                IsValidValue = false;
            }
        }

        private DateTime? InnerParseDate(string value)
        {
            DateTime result;
            CultureInfo ci = CultureInfo.GetCultureInfo("en-US");
            string[] fmts = ci.DateTimeFormat.GetAllDateTimePatterns();
            //Console.WriteLine(String.Join("\r\n", fmts));
            if (DateTime.TryParseExact(value, fmts, ci,
               DateTimeStyles.AssumeLocal, out result))
            {
                return result;
            }

            return null;
        }
    }
}