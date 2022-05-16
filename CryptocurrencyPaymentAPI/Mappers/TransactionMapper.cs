namespace CryptocurrencyPaymentAPI.Mappers
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Mappers.Utils;
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Model.ValueObjects;

    public static class TransactionMapper
    {
        public static GetRatesDto ToDtoRates(this Transaction entity) =>
            entity is null
            ? new GetRatesDto()
            : new GetRatesDto()
            {
                Amount = entity.Details.Conversion.FiatCurrency.Amount,
                FiatCurrency = entity.Details.Conversion.FiatCurrency.Currency,
                Rate = new CurrencyRateDto()
                {
                    Amount = entity.Details.Conversion.CryptoCurrency.Amount,
                    Currency = entity.Details.Conversion.CryptoCurrency.Currency,
                    Rate = entity.Details.Conversion.Rate,
                    ExpiryDate = entity.Details.Conversion.ExpiryDate
                },
                TransactionId = entity.DomainIdentifier,
            };

        public static GetInitTransactionDto ToDtoInit(this Transaction entity) =>
            entity is null
            ? new GetInitTransactionDto()
            : new GetInitTransactionDto()
            {
                TransactionId = entity.DomainIdentifier,
                ExpiryDate = entity.Details.Init.ExpiryDate,
                PaymentInfo = entity.Details.Init.PaymentInfo
            };

        public static GetTransactionDto ToDto(this Transaction entity) =>
            entity is null
            ? new GetTransactionDto()
            : new GetTransactionDto()
            {
                TransactionReference = entity.DomainIdentifier,
                MerchantTransactionReference = entity.TransactionReference,
                TransactionState = EnumDescriptionHelper.GetEnumValueAsString(entity.TransactionState),
                TransactionType = EnumDescriptionHelper.GetEnumValueAsString(entity.TransactionType),
                PaymentGateway = EnumDescriptionHelper.GetEnumValueAsString(entity.PaymentGateway),
                Details = entity.Details.ToDto()
            };

        public static Transaction ToEntity(this CreatePaymentTransactionDto dto, CurrencyConvertedDto dtoConvertion, PaymentGatewayName paymentGatewayName) =>
            dto is null
            ? new Transaction()
            : new Transaction()
            {
                DomainIdentifier = Guid.NewGuid().ToString(),
                PaymentGateway = paymentGatewayName,
                Details = new Detail()
                {
                    Conversion = new ConversionAction()
                    {
                        ActionName = ActionType.Convert,
                        DateTime = DateTime.UtcNow,
                        Success = true,
                        FiatCurrency = new Money()
                        {
                            Amount = dto.Amount,
                            Currency = dto.FiatCurrency ?? string.Empty
                        },
                        CryptoCurrency = new Money()
                        {
                            Currency = dtoConvertion?.CurrencyRate?.Currency ?? string.Empty,
                            Amount = dtoConvertion?.CurrencyRate?.Amount ?? 0,
                        },
                        ExpiryDate = DateTime.UtcNow.AddMinutes(10),
                        Rate = dtoConvertion?.CurrencyRate?.Rate ?? 0,
                        Message = null,
                        Code = null
                    },
                    Init = null
                },
                TransactionState = TransactionState.CurrencyConverted,
                PaymentGatewayTransactionId = dtoConvertion?.PaymentGatewayTransactionId ?? string.Empty,
                TransactionType = TransactionType.Payment,
                TransactionReference = dto.TransactionReference ?? string.Empty,
                MerchantId = "TODO"
            };
    }
}
