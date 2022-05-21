namespace CryptocurrencyPaymentAuth.Repositories
{
    using CryptocurrencyPaymentAuth.Model;
    using System.Linq;
    using System.Threading.Tasks;

    public class MerchantRepository : IMerchantRepository
    {
        private readonly DatabaseContext dbContext;

        public MerchantRepository()
        {
            this.dbContext = new DatabaseContext();
        }

        public async Task<Merchant> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => dbContext.Merchants.SingleOrDefault(x => x.Username == username && x.Password == password));

            if (user == null)
                return null;

            user.Password = "";
            return user;
        }
    }
}
