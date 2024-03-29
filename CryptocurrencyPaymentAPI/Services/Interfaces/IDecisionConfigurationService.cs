﻿namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using System.Threading.Tasks;

    public interface IDecisionConfigurationService
    {
        List<PaymentGatewayName> GetPossiblePaymentGateway(MerchantAuthorizationDto authorizationRequestDto, CreatePaymentTransactionDto createPaymentTransactionDto);
        Task<GetCryptoFromFiatCurrencyDto> GetCryptoFromFiatCurrency(MerchantAuthorizationDto authorizationRequestDto, string currency);
    }
}
