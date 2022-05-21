namespace CryptocurrencyPaymentConfiguration.Repositories
{
    using CryptocurrencyPaymentConfiguration.Model;

    public interface IConfigurationRepository
    {
        Task<MerchantConfiguration> GetByMerchantId(string merchantId);
    }
}
