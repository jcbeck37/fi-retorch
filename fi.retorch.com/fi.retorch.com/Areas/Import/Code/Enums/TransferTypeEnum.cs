namespace fi.retorch.com.Areas.Import.Code.Enums
{
    public enum ColumnTypeEnum
    {
        Ignore = -1,
        Transaction = 1,
        TransactionDate = 2,
        Amount = 3,
        PostedDate = 5,
        AccountName = 6,
        AccountBalance = 7,
        CheckNumber = 8,
        Memo = 9,
        Category = 10,
        PositiveNegative = 11
    }

    public static class ColumnTypeEnumExtensions
    {
        public static string ToFriendlyString(this ColumnTypeEnum me)
        {
            switch (me)
            {
                case ColumnTypeEnum.Transaction:
                    return "Transaction";
                case ColumnTypeEnum.Amount:
                    return "Amount";
                case ColumnTypeEnum.TransactionDate:
                    return "Trans. Date";
                case ColumnTypeEnum.PostedDate:
                    return "Posted Date";
                case ColumnTypeEnum.AccountName:
                    return "Account Name";
                case ColumnTypeEnum.AccountBalance:
                    return "Account Balance";
                case ColumnTypeEnum.CheckNumber:
                    return "Check #";
                case ColumnTypeEnum.Memo:
                    return "Memo";
                case ColumnTypeEnum.Category:
                    return "Category";
                case ColumnTypeEnum.PositiveNegative:
                    return "Positive/Negative";
                default:
                    return "<Ignore>";
            }
        }
    }
}