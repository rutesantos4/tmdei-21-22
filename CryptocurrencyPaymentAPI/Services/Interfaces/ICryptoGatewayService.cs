﻿namespace CryptocurrencyPaymentAPI.Services.Interfaces
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.Enums;

    public interface ICryptoGatewayService
    {
        PaymentGatewayName GetPaymentGatewayEnum();
        CurrencyConvertedDto? GetCurrencyRates(CreatePaymentTransactionDto createPaymentTransaction);
        Transaction CreateTransaction(ConfirmPaymentTransactionDto transaction, string paymentGatewayTransactionId);
        bool ServiceWorking();
    }
}