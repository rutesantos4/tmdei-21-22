namespace CryptocurrencyPaymentAPI.Mappers
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;
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
                Rate = new CurrencyRateDto()
                {
                    Amount = dtoConvertion?.CurrencyRate?.Amount ?? 0,
                    Currency = dtoConvertion?.CurrencyRate?.Currency ?? string.Empty,
                    Rate = dtoConvertion?.CurrencyRate?.Rate ?? 0,
                },
                TransactionId = entity.DomainIdentifier
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
                    Conversion = new Conversion()
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
                        ExpiryDate = DateTime.UtcNow.AddMinutes(1)
                    }
                },
                TransactionState = TransactionState.CurrencyConverted,
                PaymentGatewayTransactionId = dtoConvertion?.PaymentGatewayTransactionId ?? string.Empty,
                TransactionType = TransactionType.Payment,
                TransactionReference = dto.TransactionReference ?? string.Empty,
                MerchantId = "TODO"
            };
    }
}
