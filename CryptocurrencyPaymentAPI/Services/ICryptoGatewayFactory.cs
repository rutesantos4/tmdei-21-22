namespace CryptocurrencyPaymentAPI.Services
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.Model.Enums;
    using CryptocurrencyPaymentAPI.Services.Interfaces;

    public interface ICryptoGatewayFactory
    {
        List<ICryptoGatewayService> GetCryptoGatewayServices(AuthorizationRequestDto authorizationRequestDto, CreatePaymentTransactionDto createPaymentTransactionDto);
        ICryptoGatewayService GetCryptoGatewayService(PaymentGatewayName paymentGateway);
    }
}
