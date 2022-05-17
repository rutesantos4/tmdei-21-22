namespace CryptocurrencyPaymentAPI.Mappers
{
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Implementation;
    using CryptocurrencyPaymentAPI.Validations.ValidationMessages;

    public static class NotificationMapper
    {
        public static Transaction BitPayNotificationToEntity(this Transaction transaction, BitPayService.InvoiceResponseData notification)
        {
            if (notification.Status.Equals("confirmed", StringComparison.OrdinalIgnoreCase)
                || notification.Status.Equals("complete", StringComparison.OrdinalIgnoreCase))
            {
                transaction.TransactionState = TransactionState.Transmitted;
                transaction.Details.Debit = new Model.ValueObjects.DebitAction()
                {
                    ActionName = ActionType.Debit,
                    DateTime = DateTime.UtcNow,
                    Success = true,
                    CurrencyInfo = new Model.ValueObjects.CurrencyInfo()
                    {
                        FiatCurrency = transaction.Details.Conversion.FiatCurrency.Currency,
                        CryptoCurrency = transaction.Details.Conversion.CryptoCurrency.Currency,
                    },
                    Message = null,
                    Code = null
                };
            }
            else
            {
                var validationMessage = GetValidationMessage(notification);
                transaction.TransactionState = TransactionState.Failed;
                transaction.Details.Debit = new Model.ValueObjects.DebitAction()
                {
                    ActionName = ActionType.Debit,
                    DateTime = DateTime.UtcNow,
                    Success = false,
                    CurrencyInfo = null,
                    Message = validationMessage.Message,
                    Code = validationMessage.Code.ToString(),
                };
            }
            return transaction;
        }

        private static ValidationMessage GetValidationMessage(BitPayService.InvoiceResponseData notification)
        {
            if (notification.ExceptionStatus.Equals("paidOver", StringComparison.OrdinalIgnoreCase))
            {
                return ErrorCodes.TransactionOverPaid;
            }

            if (notification.ExceptionStatus.Equals("paidPartial", StringComparison.OrdinalIgnoreCase))
            {
                return ErrorCodes.TransactionUnderPaid;
            }

            return ErrorCodes.TransactionExpired;
        }
    }
}
