using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Payment_Service.Models
{
    public partial class PaymentsDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<Payment> Payments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC07D22DF512");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.PaymentId)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
