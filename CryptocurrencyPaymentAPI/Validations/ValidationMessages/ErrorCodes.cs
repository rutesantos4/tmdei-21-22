namespace CryptocurrencyPaymentAPI.Validations.ValidationMessages
{
    public static class ErrorCodes
    {
        public static readonly ValidationMessage MissingFiatCurrency = new(100, "Missing Fiat Currency.");
        public static readonly ValidationMessage MissingCryptoCurrency = new(101, "Missing Cryptocurrency.");
        public static readonly ValidationMessage InvalidAmount = new(102, "Amount must be bigger than zero.");
        public static readonly ValidationMessage InvalidCryptoCurrency = new(103, "Cryptocurrency is invalid.");

        public static readonly ValidationMessage InvalidTransaction = new(200, "Transaction does not exists.");

        public static readonly ValidationMessage OperationInvalid = new(300, "Transaction was not possible to be processed by payment gateway.");
        public static readonly ValidationMessage ConversionRateExpired = new(301, "Convertion Rate expired. Please perform convertion again.");
        public static readonly ValidationMessage TransactionStateConverted = new(302, "Transaction State is wrong, it should be CurrencyConverted.");

        public static readonly ValidationMessage TransactionStateInitialized = new(400, "Transaction State is wrong, it should be Initialized.");
        public static readonly ValidationMessage TransactionExpired = new(401, "Transaction expired.");
        public static readonly ValidationMessage TransactionOverPaid = new(402, "Transaction was overpaid by the customer.");
        public static readonly ValidationMessage TransactionUnderPaid = new(403, "Transaction was underpaid by the customer.");
    }
}
