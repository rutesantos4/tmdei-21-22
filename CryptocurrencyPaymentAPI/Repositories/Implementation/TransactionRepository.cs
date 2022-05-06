namespace CryptocurrencyPaymentAPI.Repositories.Implementation
{
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Repositories;
    using CryptocurrencyPaymentAPI.Repositories.Interfaces;

    internal class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}