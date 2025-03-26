using Payment_Service.Models;

using Microsoft.EntityFrameworkCore;
using Payment_Service.Models;

namespace Payment_Service.Models
{
    public partial class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
            : base(options)
        {
        }

        // DbSets representing your tables
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public virtual DbSet<PaymentPaypalTransaction> PaymentPaypalTransactions { get; set; }
        public virtual DbSet<PaymentCODTransaction> PaymentCODTransactions { get; set; }
        public virtual DbSet<PaymentRefund> PaymentRefunds { get; set; }
        public virtual DbSet<PaymentLog> PaymentLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<PaymentPaypalTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId).HasName("PK_PaymentPaypalTransaction");

                entity.Property(e => e.TransactionAmount)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.TransactionCurrency)
                    .HasMaxLength(3);

                entity.Property(e => e.TransactionStatus)
                    .HasMaxLength(20);

                entity.Property(e => e.PayPalTransactionId)
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime");

                entity.HasOne(e => e.Payment)
                    .WithMany()
                    .HasForeignKey(e => e.PaymentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

          
            OnModelCreatingPartial(modelBuilder);
        }

        
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
