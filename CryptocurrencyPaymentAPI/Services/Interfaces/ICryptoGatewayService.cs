﻿namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;

    public interface ICryptoGatewayService
    {
        PaymentGatewayName GetPaymentGatewayEnum();
        CurrencyConvertedDto? GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction);
        PaymentCreatedDto? CreateTransaction(ConfirmPaymentTransactionDto confirmTransactionDto);
        bool ServiceWorking();
    }
}
