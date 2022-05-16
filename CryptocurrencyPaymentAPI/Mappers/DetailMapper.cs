namespace CryptocurrencyPaymentAPI.Mappers
{
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Model.ValueObjects;

    public static class DetailMapper
    {
        public static GetDetailDto ToDto(this Detail entity) =>
            entity is null
            ? new GetDetailDto()
            : new GetDetailDto
            {
                Conversion = entity.Conversion.ToDto(),
                Init = entity.Init.ToDto(),
                Debit = entity.Debit.ToDto()
            };
    }
}
