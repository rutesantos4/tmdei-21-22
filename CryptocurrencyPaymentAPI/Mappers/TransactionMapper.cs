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
        public static GetRatesDto ToDto(this Transaction entity, CreatePaymentTransactionDto dtoCreation, CurrencyConvertedDto dtoConvertion) =>
            entity is null
            ? new GetRatesDto()
            : new GetRatesDto()
            {
                Amount = dtoCreation.Amount,
                FiatCurrency = dtoCreation.FiatCurrency,
                Rate = dtoConvertion.CurrencyRate,
                TransactionId = entity.DomainIdentifier
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
                        ExpiryDate = DateTime.UtcNow.AddMinutes(10),// TODO - This is wrong
                        Message = null,
                        Reason = null
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
