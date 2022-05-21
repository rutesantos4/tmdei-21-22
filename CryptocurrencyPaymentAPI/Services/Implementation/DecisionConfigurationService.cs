namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using System.Collections.Generic;

    public class DecisionConfigurationService : IDecisionConfigurationService
    {

        public List<PaymentGatewayName> GetPossiblePaymentGateway(MerchantAuthorizationDto authorizationRequestDto,
            CreatePaymentTransactionDto createPaymentTransactionDto)
        {
            // TODO - call external service
            return new List<PaymentGatewayName>() { PaymentGatewayName.BitPay };
        }
    }
}
