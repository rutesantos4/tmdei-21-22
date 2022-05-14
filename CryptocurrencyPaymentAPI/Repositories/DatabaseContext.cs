namespace CryptocurrencyPaymentAPI.Repositories
{
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.ValueObjects;
    using Microsoft.EntityFrameworkCore;

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DatabaseContext()
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConversionAction>();
            modelBuilder.Entity<InitAction>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
