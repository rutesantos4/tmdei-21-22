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
            if (new string[] { "confirmed", "complete" }.Contains(notification.Status.ToLower()))
            {
                return GetSuccessTransaction(transaction);
            }
            var validationMessage = GetValidationMessage(notification);
            return GetFailedTransaction(transaction, validationMessage);
        }

        public static Transaction CoinbaseNotificationToEntity(this Transaction transaction, CoinbaseService.CoinbaseChargeResponse notification)
        {
            var lastTime = notification.Data.Timeline.OrderBy(x => x.Time).Last();
            if (new string[] { "RESOLVED", "COMPLETED" }.Contains(lastTime.Status.ToUpper()))
            {
                return GetSuccessTransaction(transaction);
            }
            var validationMessage = GetValidationMessage(lastTime);
            return GetFailedTransaction(transaction, validationMessage);
        }

        public static Transaction CoinqvestNotificationToEntity(this Transaction transaction, CoinqvestService.CoinqvestNotification notification)
        {
            if (notification.Data.Checkout.State.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
            {
                return GetSuccessTransaction(transaction);
            }
            var validationMessage = GetValidationMessage(notification);
            return GetFailedTransaction(transaction, validationMessage);
        }

        private static Transaction GetSuccessTransaction(Transaction transaction)
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
            return transaction;
        }

        private static Transaction GetFailedTransaction(Transaction transaction, ValidationMessage validationMessage)
        {
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

        private static ValidationMessage GetValidationMessage(CoinbaseService.Timeline notification)
        {
            if (notification.Context.Equals("OVERPAID", StringComparison.OrdinalIgnoreCase))
            {
                return ErrorCodes.TransactionOverPaid;
            }

            if (notification.Context.Equals("UNDERPAID", StringComparison.OrdinalIgnoreCase))
            {
                return ErrorCodes.TransactionUnderPaid;
            }

            return ErrorCodes.TransactionExpired;
        }

        private static ValidationMessage GetValidationMessage(CoinqvestService.CoinqvestNotification notification)
        {
            if (notification.Data.Checkout.State.Equals("UNRESOLVED_UNDERPAID", StringComparison.OrdinalIgnoreCase))
            {
                return ErrorCodes.TransactionUnderPaid;
            }

            return ErrorCodes.TransactionExpired;
        }

    }
}
