namespace fi.retorch.com.Areas.Dashboard.Code.Enums
{
    public enum TransferTypeEnum
    {
        Expense = 0,
        Income = 1,
        Transfer = 2
    }

    public static class TransferTypeExtensions
    {
        public static string ToFriendlyString(this TransferTypeEnum me)
        {
            switch (me)
            {
                case TransferTypeEnum.Expense:
                    return "Expense";
                case TransferTypeEnum.Income:
                    return "Income";
                case TransferTypeEnum.Transfer:
                default:
                    return "Transfer";
            }
        }
    }
}