using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment_Service.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    payment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    payment_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.payment_id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentCodTransactions",
                columns: table => new
                {
                    CodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CollectedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CollectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCodTransactions", x => x.CodId);
                    table.ForeignKey(
                        name: "FK_PaymentCodTransactions_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "payment",
                        principalColumn: "payment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentLogs",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LogMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_PaymentLogs_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "payment",
                        principalColumn: "payment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentPaypalTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayPalTransactionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TransactionStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentPaypalTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_PaymentPaypalTransactions_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "payment",
                        principalColumn: "payment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransactionStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "payment",
                        principalColumn: "payment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRefunds",
                columns: table => new
                {
                    RefundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RefundReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RefundStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRefunds", x => x.RefundId);
                    table.ForeignKey(
                        name: "FK_PaymentRefunds_PaymentTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentRefunds_payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "payment",
                        principalColumn: "payment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCodTransactions_PaymentId",
                table: "PaymentCodTransactions",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLogs_PaymentId",
                table: "PaymentLogs",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentPaypalTransactions_PaymentId",
                table: "PaymentPaypalTransactions",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRefunds_PaymentId",
                table: "PaymentRefunds",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRefunds_TransactionId",
                table: "PaymentRefunds",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentId",
                table: "PaymentTransactions",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentCodTransactions");

            migrationBuilder.DropTable(
                name: "PaymentLogs");

            migrationBuilder.DropTable(
                name: "PaymentPaypalTransactions");

            migrationBuilder.DropTable(
                name: "PaymentRefunds");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "payment");
        }
    }
}
