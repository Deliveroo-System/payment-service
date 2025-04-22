using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment_Service.Migrations
{
    /// <inheritdoc />
    public partial class FixPaymentRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentCodTransactions_payment_PaymentId",
                table: "PaymentCodTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentPaypalTransactions_payment_PaymentId",
                table: "PaymentPaypalTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_payment_PaymentId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_PaymentId",
                table: "PaymentTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentPaypalTransactions",
                table: "PaymentPaypalTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentCodTransactions",
                table: "PaymentCodTransactions");

            migrationBuilder.RenameTable(
                name: "PaymentPaypalTransactions",
                newName: "payment_paypal_transactions");

            migrationBuilder.RenameTable(
                name: "PaymentCodTransactions",
                newName: "payment_cod_transactions");

            migrationBuilder.RenameColumn(
                name: "TransactionStatus",
                table: "payment_paypal_transactions",
                newName: "transaction_status");

            migrationBuilder.RenameColumn(
                name: "TransactionCurrency",
                table: "payment_paypal_transactions",
                newName: "transaction_currency");

            migrationBuilder.RenameColumn(
                name: "TransactionAmount",
                table: "payment_paypal_transactions",
                newName: "transaction_amount");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "payment_paypal_transactions",
                newName: "payment_id");

            migrationBuilder.RenameColumn(
                name: "PayPalTransactionId",
                table: "payment_paypal_transactions",
                newName: "paypal_transaction_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "payment_paypal_transactions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "payment_paypal_transactions",
                newName: "transaction_id");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentPaypalTransactions_PaymentId",
                table: "payment_paypal_transactions",
                newName: "IX_payment_paypal_transactions_payment_id");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "payment_cod_transactions",
                newName: "payment_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "payment_cod_transactions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CollectedBy",
                table: "payment_cod_transactions",
                newName: "collected_by");

            migrationBuilder.RenameColumn(
                name: "CollectedAt",
                table: "payment_cod_transactions",
                newName: "collected_at");

            migrationBuilder.RenameColumn(
                name: "CodStatus",
                table: "payment_cod_transactions",
                newName: "cod_status");

            migrationBuilder.RenameColumn(
                name: "CodId",
                table: "payment_cod_transactions",
                newName: "cod_id");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentCodTransactions_PaymentId",
                table: "payment_cod_transactions",
                newName: "IX_payment_cod_transactions_payment_id");

            migrationBuilder.AlterColumn<string>(
                name: "GatewayTransactionId",
                table: "PaymentTransactions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "order_id",
                table: "payment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId1",
                table: "payment_paypal_transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId1",
                table: "payment_cod_transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_payment_paypal_transactions",
                table: "payment_paypal_transactions",
                column: "transaction_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payment_cod_transactions",
                table: "payment_cod_transactions",
                column: "cod_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_paypal_transactions_PaymentId1",
                table: "payment_paypal_transactions",
                column: "PaymentId1");

            migrationBuilder.CreateIndex(
                name: "IX_payment_cod_transactions_PaymentId1",
                table: "payment_cod_transactions",
                column: "PaymentId1",
                unique: true,
                filter: "[PaymentId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_payment_cod_transactions_payment_PaymentId1",
                table: "payment_cod_transactions",
                column: "PaymentId1",
                principalTable: "payment",
                principalColumn: "payment_id");

            migrationBuilder.AddForeignKey(
                name: "FK_payment_cod_transactions_payment_payment_id",
                table: "payment_cod_transactions",
                column: "payment_id",
                principalTable: "payment",
                principalColumn: "payment_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_paypal_transactions_payment_PaymentId1",
                table: "payment_paypal_transactions",
                column: "PaymentId1",
                principalTable: "payment",
                principalColumn: "payment_id");

            migrationBuilder.AddForeignKey(
                name: "FK_payment_paypal_transactions_payment_payment_id",
                table: "payment_paypal_transactions",
                column: "payment_id",
                principalTable: "payment",
                principalColumn: "payment_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payment_cod_transactions_payment_PaymentId1",
                table: "payment_cod_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_payment_cod_transactions_payment_payment_id",
                table: "payment_cod_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_payment_paypal_transactions_payment_PaymentId1",
                table: "payment_paypal_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_payment_paypal_transactions_payment_payment_id",
                table: "payment_paypal_transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payment_paypal_transactions",
                table: "payment_paypal_transactions");

            migrationBuilder.DropIndex(
                name: "IX_payment_paypal_transactions_PaymentId1",
                table: "payment_paypal_transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payment_cod_transactions",
                table: "payment_cod_transactions");

            migrationBuilder.DropIndex(
                name: "IX_payment_cod_transactions_PaymentId1",
                table: "payment_cod_transactions");

            migrationBuilder.DropColumn(
                name: "PaymentId1",
                table: "payment_paypal_transactions");

            migrationBuilder.DropColumn(
                name: "PaymentId1",
                table: "payment_cod_transactions");

            migrationBuilder.RenameTable(
                name: "payment_paypal_transactions",
                newName: "PaymentPaypalTransactions");

            migrationBuilder.RenameTable(
                name: "payment_cod_transactions",
                newName: "PaymentCodTransactions");

            migrationBuilder.RenameColumn(
                name: "transaction_status",
                table: "PaymentPaypalTransactions",
                newName: "TransactionStatus");

            migrationBuilder.RenameColumn(
                name: "transaction_currency",
                table: "PaymentPaypalTransactions",
                newName: "TransactionCurrency");

            migrationBuilder.RenameColumn(
                name: "transaction_amount",
                table: "PaymentPaypalTransactions",
                newName: "TransactionAmount");

            migrationBuilder.RenameColumn(
                name: "paypal_transaction_id",
                table: "PaymentPaypalTransactions",
                newName: "PayPalTransactionId");

            migrationBuilder.RenameColumn(
                name: "payment_id",
                table: "PaymentPaypalTransactions",
                newName: "PaymentId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "PaymentPaypalTransactions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "transaction_id",
                table: "PaymentPaypalTransactions",
                newName: "TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_payment_paypal_transactions_payment_id",
                table: "PaymentPaypalTransactions",
                newName: "IX_PaymentPaypalTransactions_PaymentId");

            migrationBuilder.RenameColumn(
                name: "payment_id",
                table: "PaymentCodTransactions",
                newName: "PaymentId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "PaymentCodTransactions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "collected_by",
                table: "PaymentCodTransactions",
                newName: "CollectedBy");

            migrationBuilder.RenameColumn(
                name: "collected_at",
                table: "PaymentCodTransactions",
                newName: "CollectedAt");

            migrationBuilder.RenameColumn(
                name: "cod_status",
                table: "PaymentCodTransactions",
                newName: "CodStatus");

            migrationBuilder.RenameColumn(
                name: "cod_id",
                table: "PaymentCodTransactions",
                newName: "CodId");

            migrationBuilder.RenameIndex(
                name: "IX_payment_cod_transactions_payment_id",
                table: "PaymentCodTransactions",
                newName: "IX_PaymentCodTransactions_PaymentId");

            migrationBuilder.AlterColumn<string>(
                name: "GatewayTransactionId",
                table: "PaymentTransactions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "order_id",
                table: "payment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentPaypalTransactions",
                table: "PaymentPaypalTransactions",
                column: "TransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentCodTransactions",
                table: "PaymentCodTransactions",
                column: "CodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentId",
                table: "PaymentTransactions",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentCodTransactions_payment_PaymentId",
                table: "PaymentCodTransactions",
                column: "PaymentId",
                principalTable: "payment",
                principalColumn: "payment_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentPaypalTransactions_payment_PaymentId",
                table: "PaymentPaypalTransactions",
                column: "PaymentId",
                principalTable: "payment",
                principalColumn: "payment_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_payment_PaymentId",
                table: "PaymentTransactions",
                column: "PaymentId",
                principalTable: "payment",
                principalColumn: "payment_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
