namespace CryptocurrencyPaymentAPI.Mappers
{
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Mappers.Utils;
    using CryptocurrencyPaymentAPI.Model.ValueObjects;

    public static class ActionMapper
    {
        public static GetConversionActionDto ToDto(this Conversion entity) =>
            entity is null
            ? new GetConversionActionDto()
            : new GetConversionActionDto()
            {
                ActionName = EnumDescriptionHelper.GetEnumValueAsString(entity.ActionName),
                DateTime = entity.DateTime,
                Success = entity.Success,
                ExpiryDate = entity.ExpiryDate,
                FiatCurrency = entity.FiatCurrency.ToDto(),
                CryptoCurrency = entity.CryptoCurrency.ToDto(),
                Reason = entity.Reason,
                Message = entity.Message,
            };


    }
}
