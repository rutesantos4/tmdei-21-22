namespace CryptocurrencyPaymentAuth.Repositories
{
    using CryptocurrencyPaymentAuth.Model;

    public class DatabaseContext
    {
        public List<Merchant> Merchants { get; set; } = new List<Merchant>()
        {
            new Merchant(){ Id = "admin", Password = "admin-authenticated", Username = "admin-authenticated" },
            new Merchant(){ Id = "merchantId-Test", Password = "merchant-tests", Username = "merchant-tests" },
        };
    }
}
