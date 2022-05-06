namespace CryptocurrencyPaymentAPI.Repositories.Interfaces
{
    using CryptocurrencyPaymentAPI.Model.Entities;

    public interface IRepository<T> where T : BaseEntity

    {
        Task<T> GetById(long id);
        Task<T> GetByDomainIdentifier(string domainIdentifier);
        Task<List<T>> List();
        Task<T> Add(T entity);
        Task<T> Delete(T entity);
        Task<T> Update(T entity);
    }
}
