namespace CryptocurrencyPaymentAuth.Repositories
{
    using CryptocurrencyPaymentAuth.Model;

    public interface IMerchantRepository
    {
        Task<Merchant> Authenticate(string username, string password);
    }
}
