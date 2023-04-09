using SovosAssessment.Domain.Entities.Common.Concrete;
using System.Linq.Expressions;

namespace SovosAssessment.Infrastructure.Persistence.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();

        IQueryable<T> GetAll(Expression<Func<T, bool>> expression); // LINQ desteği sunabilmek içinde expression'ları kullanıyoruz.

        Task<T> GetByIdAsync(int id);

        T GetById(int id);

        T Get(Expression<Func<T, bool>> expression);

        Task CreateAsync(T entity);

        void Create(T entity);

        void Update(T entity);

        Task DeleteAsync(int id);

        void Delete(T entity);

        void Delete(int id);

        Task<int> InsertAndGetIdAsync(T entity);

        Task<T> InsertAsync(T entity);

        int InsertAndGetId(T entity);

        T Insert(T entity);

        void AddRange(IEnumerable<T> entities);

        void RemoveRange(IEnumerable<T> entities);
    }
}

