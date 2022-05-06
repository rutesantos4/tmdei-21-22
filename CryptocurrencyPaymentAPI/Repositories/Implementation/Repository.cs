namespace CryptocurrencyPaymentAPI.Repositories.Implementation
{
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DatabaseContext _dbContext;

        public Repository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        /*
        * Returns all entities of type T that are active or not, in an object that implements interface IQueryable.
        * Prefered method when there is a need to include attributes that are "lazy".
        */
        protected IQueryable<T> GetQueryable()
        {
            return _dbContext.Set<T>().Where(entity => !entity.IsDeleted);
        }

        /*
        * Gets the entity with the given database ID.
        */
        public async virtual Task<T> GetById(long id)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
        }

        /*
        * Gets the entity with the given Domain ID.
        */
        public async virtual Task<T> GetByDomainIdentifier(string domainIdentifier)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.DomainIdentifier == domainIdentifier);
        }

        /*
        * Returns all entities of type T that are active in the form of List<T>. 
        * Method to be overriden when in need to retrieve all categories including additional "lazy" atributtes.
        */
        public async virtual Task<List<T>> List()
        {
            return await this.GetQueryable().AsQueryable().ToListAsync();
        }

        /*
        * Creates the passed entity in the DB.
        */
        public async Task<T> Add(T entity)
        {
            entity.Version = 1;
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        /*
        * "Deactivates" the passed entity. (Soft delete)
        */
        public async virtual Task<T> Delete(T entity)
        {
            entity.IsDeleted = true;
            return await Update(entity);
        }

        /*
        * Updates the passed entity.
        */
        public async Task<T> Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            entity.Version += 1;
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
