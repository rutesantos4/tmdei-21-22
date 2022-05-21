namespace CryptocurrencyPaymentConfiguration.Repositories
{
    using CryptocurrencyPaymentConfiguration.Model;
    using System.Threading.Tasks;

    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly DatabaseContext dbContext;

        public ConfigurationRepository()
        {
            this.dbContext = new DatabaseContext();
        }

        public async Task<MerchantConfiguration> GetByMerchantId(string merchantId)
        {
            return await Task.Run(() =>
                dbContext.MerchantConfigurations.SingleOrDefault(x => x.MerchantId.Equals(merchantId, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
