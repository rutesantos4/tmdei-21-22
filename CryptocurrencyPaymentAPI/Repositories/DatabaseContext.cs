namespace CryptocurrencyPaymentAPI.Repositories
{
    using CryptocurrencyPaymentAPI.Model.Entities;
    using Microsoft.EntityFrameworkCore;

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DatabaseContext()
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
