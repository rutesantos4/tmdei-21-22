namespace CryptocurrencyPaymentAPI.Mappers
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Mappers.Utils;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Model.ValueObjects;

    public static class ActionMapper
    {
        public static GetConversionActionDto ToDto(this ConversionAction entity) =>
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
                Reason = entity.Code,
                Message = entity.Message,
            };

        public static InitAction ToEntity(this PaymentCreatedDto dto) =>
            dto is null
            ? new InitAction()
            : new InitAction()
            {
                ActionName = ActionType.Init,
                DateTime = DateTime.UtcNow,
                Success = true,
                ExpiryDate = dto.ExpiryDate,
                PaymentInfo = GetPaymentInfo(dto),
                Message = null,
                Code = null
            };

        public static GetInitActionDto? ToDto(this InitAction entity) =>
            entity is null
            ? null
            : new GetInitActionDto()
            {
                ActionName = EnumDescriptionHelper.GetEnumValueAsString(entity.ActionName),
                DateTime = entity.DateTime,
                Success = entity.Success,
                ExpiryDate = entity.ExpiryDate,
                PaymentInfo = entity.PaymentInfo,
                Reason = entity.Code,
                Message = entity.Message,
            };

        public static GetDebitActionDto? ToDto(this DebitAction entity) =>
            entity is null
            ? null
            : new GetDebitActionDto()
            {
                ActionName = EnumDescriptionHelper.GetEnumValueAsString(entity.ActionName),
                DateTime = entity.DateTime,
                Success = entity.Success,
                CryptoCurrency = entity.CurrencyInfo.CryptoCurrency,
                FiatCurrency = entity.CurrencyInfo.FiatCurrency,
                Reason = entity.Code,
                Message = entity.Message,
            };

        private static string GetPaymentInfo(PaymentCreatedDto dto)
        {
            return !string.IsNullOrWhiteSpace(dto.PaymentLink) ? dto.PaymentLink : dto.WalletId;
        }

    }
}
