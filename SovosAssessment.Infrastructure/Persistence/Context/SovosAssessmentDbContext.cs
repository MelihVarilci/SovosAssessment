using Microsoft.EntityFrameworkCore;
using SovosAssessment.Domain.Entities;

namespace SovosAssessment.Infrastructure.Persistence.Context
{
    // Summary:
    //     Temel varlıkların tanımlandığı DbContext sınıfı
    public class SovosAssessmentDbContext : DbContext
    {
        public SovosAssessmentDbContext(DbContextOptions<SovosAssessmentDbContext> contextOptions) : base(contextOptions)
        {

        }

        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceLine> InvoiceLines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>(x =>
            {
                x.HasKey(p => p.Id);
                x.HasMany(p => p.InvoiceLines);
            });

            modelBuilder.Entity<InvoiceLine>(x =>
            {
                x.HasKey(p => p.Id);
                x.HasOne(p => p.InvoiceFk);
            });
        }
    }
}
