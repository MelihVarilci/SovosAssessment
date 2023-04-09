using SovosAssessment.Domain.Entities.Common.Concrete;
using SovosAssessment.Infrastructure.Persistence.Context;
using SovosAssessment.Infrastructure.Persistence.Repositories;

namespace SovosAssessment.Application.Abstractions.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SovosAssessmentDbContext _context;

        public UnitOfWork(SovosAssessmentDbContext context)
        {
            //Database.SetInitializer<DbContextClass>(null);

            if (context == null)
                throw new ArgumentNullException("dbContext can not be null.");

            _context = context;

            Invoice = new InvoiceRepository(_context);
            InvoiceLine = new InvoiceLineRepository(_context);
        }

        public IInvoiceRepository Invoice { get; private set; }
        public IInvoiceLineRepository InvoiceLine { get; private set; }


        public IGenericRepository<T> GetRepository<T>() where T : BaseEntity
        {
            return new GenericRepository<T>(_context);
        }

        public void SaveChanges()
        {
            try
            {
                // Transaction işlemleri burada ele alınabilir veya Identity Map kurumsal tasarım kalıbı kullanılarak
                // sadece değişen alanları güncellemeyide sağlayabiliriz.
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.SaveChanges();
                        transaction.Commit();
                    }
                    catch
                    {
                        // Burada DbEntityValidationException hatalarını handle edebiliriz.
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO:Logging
            }
        }

        // Burada IUnitOfWork arayüzüne implemente ettiğimiz IDisposable arayüzünün Dispose Patternini implemente ediyoruz.
        private bool disposed = false;
        // Kaynakları boşaltmak için dispose uygulayın
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // Serbest bırakılmış yönetilmeyen Kaynaklar
                if (disposing)
                {
                    // Serbest Bırakılan Yönetilen Kaynaklar
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        // Kaynakları açıkça serbest bırakmak için Dispose'u çağırın
        public void Dispose()
        {
            // Yönetilen kaynakları da temizlemek için dispose yönteminde true değerini geçin ve bir sonraki satırda sonlandırmayı atlamak için GC deyin.
            Dispose(true);
            // Dispose zaten çağrılmışsa, Garbage Collector'e bu örnek üzerinde finalize etmeyi atlamasını söyleyin.
            GC.SuppressFinalize(this);
        }
    }
}

