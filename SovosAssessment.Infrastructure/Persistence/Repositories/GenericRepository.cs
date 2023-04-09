using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SovosAssessment.Domain.Entities.Common.Concrete;
using SovosAssessment.Infrastructure.Persistence.Context;
using SovosAssessment.Infrastructure.Persistence.Repositories;
using System.Linq.Expressions;

namespace SovosAssessment.Application.Abstractions.Services
{
    /// <summary>
    /// EntityFramework için hazırlıyor olduğumuz bu repositoriyi daha önceden tasarladığımız generic repositorimiz olan IRepository arayüzünü implemente ederek tasarladık.
    /// Bu şekilde tasarlamamızın ana sebebi ise veritabanına independent(bağımsız) bir durumda kalabilmek. Örneğin MongoDB için ise ilgili provider'ı aracılığı ile MongoDBRepository tasarlayabiliriz.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly SovosAssessmentDbContext context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(SovosAssessmentDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("dbContext can not be null.");

            this.context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            // Üzerinde herhangi bir değişiklik yapmayacağımız işlemlerde AsNoTracking kullanılarak performans attırılır.
            // Entity Framework tarafından sorgu neticesinde elde edilen nesnelerin takip mekanizması sistem tarafından izlenmelerine son verilmesini sağlamakta.
            return _dbSet.AsNoTracking();
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public T Get(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression).SingleOrDefault();
        }

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Create(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            //_dbSet.Attach(entity);
            //context.Entry(entity).State = EntityState.Modified;
            _dbSet.Update(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            _dbSet.Remove(entity);
        }

        public void Delete(T entity)
        {
            // Eğer sizlerde genelde bir kayıtı silmek yerine IsDelete şeklinde bool bir flag alanı tutuyorsanız,
            // Küçük bir refleciton kodu yardımı ile bunuda otomatikleştirebiliriz.
            if (entity.GetType().GetProperty("IsDelete") != null)
            {
                T _entity = entity;

                _entity.GetType().GetProperty("IsDelete").SetValue(_entity, true);

                Update(_entity);
            }
            else
            {
                // Önce entity'nin state'ini kontrol etmeliyiz.
                EntityEntry dbEntityEntry = context.Entry(entity);

                if (dbEntityEntry.State != EntityState.Deleted)
                {
                    dbEntityEntry.State = EntityState.Deleted;
                }
                else
                {
                    _dbSet.Attach(entity);
                    _dbSet.Remove(entity);
                }
            }
        }

        public void Delete(int id)
        {
            var entity = GetById(id);

            if (entity == null) return;
            else
            {
                if (entity.GetType().GetProperty("IsDelete") != null)
                {
                    T _entity = entity;
                    _entity.GetType().GetProperty("IsDelete").SetValue(_entity, true);

                    Update(_entity);
                }
                else
                {
                    Delete(entity);
                }
            }
        }

        public async Task<int> InsertAndGetIdAsync(T entity)
        {
            var insertedEntity = await InsertAsync(entity);
            return insertedEntity.Id;
        }

        public virtual Task<T> InsertAsync(T entity)
        {
            return Task.FromResult(Insert(entity));
        }

        public int InsertAndGetId(T entity)
        {
            return Insert(entity).Id;
        }

        public T Insert(T entity)
        {
            var result = _dbSet.Add(entity);
            context.SaveChanges();
            return result.Entity;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}

