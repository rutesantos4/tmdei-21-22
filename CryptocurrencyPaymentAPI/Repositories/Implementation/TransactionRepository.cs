namespace CryptocurrencyPaymentAPI.Repositories.Implementation
{
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Repositories;
    using CryptocurrencyPaymentAPI.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;

    internal class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Transaction> GetByDomainIdentifier(string domainIdentifier)
        {
            return await GetQueryable()
                .Include(entity => entity.Details)
                    .ThenInclude(entity => entity.Conversion)
                        .ThenInclude(entity => entity.FiatCurrency)
                .Include(entity => entity.Details)
                    .ThenInclude(entity => entity.Conversion)
                        .ThenInclude(entity => entity.CryptoCurrency)
                .Include(entity => entity.Details)
                    .ThenInclude(entity => entity.Init)
                .Include(entity => entity.Details)
                    .ThenInclude(entity => entity.Debit)
                        .ThenInclude(entity => entity.CurrencyInfo)
                .FirstOrDefaultAsync(e => e.DomainIdentifier == domainIdentifier);
        }
    }
}