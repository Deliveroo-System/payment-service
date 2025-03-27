using Microsoft.EntityFrameworkCore;
using Payment_Service.Models;



namespace Payment_Service.Models
{
    public partial class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<PaymentPaypalTransaction> PaymentPaypalTransactions { get; set; }
        public DbSet<PaymentCODTransaction> PaymentCodTransactions { get; set; }
        public DbSet<PaymentRefund> PaymentRefunds { get; set; }
        public DbSet<PaymentLog> PaymentLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .Property(p => p.TotalAmount)
                .HasPrecision(18, 2); // Ensures decimal precision

            modelBuilder.Entity<PaymentPaypalTransaction>()
                .Property(p => p.TransactionAmount)
                .HasPrecision(18, 2); // Ensures decimal precision
        }
    }
}


