namespace CryptocurrencyPaymentAPI.Mappers
{
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Model.ValueObjects;

    public static class MoneyMapper
    {
        public static GetMoneyDto ToDto(this Money entity) =>
            entity is null
            ? new GetMoneyDto()
            : new GetMoneyDto
            {
                Currency = entity.Currency,
                Amount = entity.Amount
            };
    }
}
